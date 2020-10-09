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
            var order = depGraph.TopologicalSort();
            var table = NodeDataTable.Create(graphContainer);

            return new IntermediateRepresentation{order = order, table = table};
        }

        public void Print()
        {
            foreach (var guid in order)
            {
                var node = table[guid];
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append($"{node.Return} = ");
                stringBuilder.Append($"{node.OperatorType}");
                stringBuilder.Append("(");
                var delimiter = "";
                foreach (var argument in node.Arguments)
                {
                    stringBuilder.Append(delimiter);
                    stringBuilder.Append($"{argument}");
                    delimiter = ", ";
                }
                stringBuilder.Append(") {");
                stringBuilder.Append($"{node.CodeText}");
                stringBuilder.Append("}");
                Debug.Log(stringBuilder.ToString());
            }
        }
    }
}
