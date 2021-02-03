using System.Text;
using UnityEngine;

namespace FRPGraph.Runtime
{
    public static class SodiumCompiler
    {
        private static string ConvertOperatorName(string input)
        {
            switch (input)
            {
                case "LiftB": return "map";
                case "Lift2B": return "lift";
                case "DomInputB": return "";
                //デフォルトでは小文字 + Bにする
                default: return input.ToLower().Remove(input.Length - 1) + "B";
            }
        }
        
        public static string Compile(IntermediateRepresentation imr)
        {
            StringBuilder result = new StringBuilder();
            foreach (var guid in imr.order)
            {
                var node = imr.table[guid];
                // ignore ConstantB and EndB
                if(node.OperatorType == "ConstantB" || node.OperatorType == "EndB") continue;
                if (node.OperatorType == "DomInputB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"final Cell<String> {node.Return};");
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
                else if (node.OperatorType == "InputB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"//InputB [{node.Return}]");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "LabelB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"final var {node.CodeText} = new SLabel({node.Arguments[0]});");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "FieldB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"final var {node.CodeText} = new STextField(\"\", 30);");
                    stringBuilder.AppendLine($"final var {node.Return} = {node.CodeText}.text;");
                    result.Append(stringBuilder);
                }
                else if (node.OperatorType == "DisableableFieldB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine($"final var {node.CodeText} = new STextField(\"\", 30, {node.Arguments[0]});");
                    stringBuilder.AppendLine($"final var {node.Return} = {node.CodeText}.text;");
                    result.Append(stringBuilder);
                }
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"final var {node.Return} = {node.Arguments[0]}.");
                    stringBuilder.Append($"{ConvertOperatorName(node.OperatorType)}");
                    stringBuilder.Append("(");
                    var delimiter = ",";
                    for (var i=1; i<node.Arguments.Count; ++i)
                    {
                        var argument = node.Arguments[i];
                        stringBuilder.Append($"{argument}");
                        stringBuilder.Append(delimiter);
                    }
                    stringBuilder.Append($"{node.CodeText.Replace("=>", "->")}");
                    

                    stringBuilder.Append(");");
                    stringBuilder.Append("\n");
                    result.Append(stringBuilder);
                }
            }
            return result.ToString();
        }

        static public void Print(IntermediateRepresentation imr)
        {
            var res = Compile(imr);
            Debug.Log(res);
        }
    }
}
