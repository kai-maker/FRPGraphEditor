using System;
using System.Collections.Generic;
using System.Text;
using FRPGraph.Editor.Nodes;
using UnityEngine;

namespace FRPGraph.Runtime
{
    public class IntermediateRepresentation
    {
        public List<Guid> order;
        public Dictionary<Guid, NodeDataTable.NodeData> table;
        
        public static IntermediateRepresentation Create(FrpGraphContainer graphContainer)
        {
            var depGraph = DependencyGraph.Create(graphContainer);
            var cutEdge = depGraph.CutToAcyclicGraph();
            var table = NodeDataTable.Create(graphContainer);
            var order = depGraph.TopologicalSort();

            return new IntermediateRepresentation{order = order, table = table};
        }

        private static string ConvertOperatorName(string input)
        {
            switch (input)
            {
                case "Lift2B": return "liftB";
                case "DomInputB": return "";
                //デフォルトでは小文字 + Bにする
                default: return input.ToLower().Remove(input.Length - 1) + "B";
            }
        }

        public void Print()
        {
            StringBuilder result = new StringBuilder();
            foreach (var guid in order)
            {
                var node = table[guid];
                // ignore ConstantB and EndB
                if(node.OperatorType == "ConstantB" || node.OperatorType == "EndB") continue;
                if (node.OperatorType == "DomInputB")
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append($"const {node.Return} = extractValueB(document.querySelector('#{node.CodeText}'))");
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
            Debug.Log(result.ToString());
        }
    }
}
