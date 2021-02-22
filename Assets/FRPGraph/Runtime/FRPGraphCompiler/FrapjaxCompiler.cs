using System.Linq;
using System.Text;
using UnityEngine;

namespace FRPGraph.Runtime
{
    public static class FrapjaxCompiler
    {
        private static string ConvertOperatorName(string input)
        {
            switch (input)
            {
                case "Lift2B": return "liftB";
                case "DomInputB": return "";
                //デフォルトでは小文字 + B or Eにする
                default:
                {
                    var copy = input;
                    return copy.ToLower().Remove(input.Length - 1) + input.Last();
                }
            }
        }
        
        public static string Compile(IntermediateRepresentation imr)
        {
            StringBuilder result = new StringBuilder();
            result.Append("export default function generated() {\n");
            foreach (var guid in imr.order)
            {
                var node = imr.table[guid];
                // ignore ConstantB and EndB
                if(node.OperatorType == "ConstantB") continue;
                if (node.OperatorType == "DomInputB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"const {node.Return} = extractValueB(document.querySelector('#{node.CodeText}'));");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "DomInputE")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"const {node.Return} = extractValueE(document.querySelector('#{node.CodeText}'));");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "DomOutputB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"insertDomB( {node.Arguments[0]} , '{node.CodeText}' );");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "DomOutputE")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"insertDomE( {node.Arguments[0]} , '{node.CodeText}' );");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "DomEnableB")
                {
                    StringBuilder stringBuilder = new StringBuilder();

                    stringBuilder.Append($"liftB(a => {{document.querySelector('#{node.CodeText}').disabled = !a}}, {node.Arguments[0]})");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "DebugB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"insertDomB( {node.Arguments[0]} , '{node.CodeText}' );");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "SnapshotE")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"const {node.Return} = snapshotE( {node.Arguments[0]} , {node.Arguments[1]} );");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "FilterE")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"const {node.Return} = filterE( {node.Arguments[0]} , {node.CodeText} );");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"const {node.Return} = ");
                    stringBuilder.Append($"{ConvertOperatorName(node.OperatorType)}");
                    stringBuilder.Append("(");
                    stringBuilder.Append($"{node.CodeText}, ");
                    var delimiter = "";
                    foreach (var argument in node.Arguments)
                    {
                        stringBuilder.Append(delimiter);
                        stringBuilder.Append($"{argument}");
                        delimiter = ", ";
                    }

                    stringBuilder.Append(");");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
            }
            result.Append("}\n");
            return result.ToString();
        }

        static public void Print(IntermediateRepresentation imr)
        {
            var res = Compile(imr);
            Debug.Log(res);
        }
    }
}
