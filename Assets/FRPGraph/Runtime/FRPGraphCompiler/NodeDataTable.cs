using System;
using System.Collections.Generic;

namespace FRPGraph.Runtime
{
    public static class NodeDataTable
    {
        public static Dictionary<Guid, NodeData> Create(FrpGraphContainer frpGraphContainer)
        {
            var nodeDict = new Dictionary<Guid, NodeData>();
            foreach (var nodeData in frpGraphContainer.FrpNodeData)
            {
                nodeDict[new Guid(nodeData.Guid)] = new NodeData
                {
                    OperatorType = nodeData.OperatorInfo.OperatorName,
                    CodeText = nodeData.CodeText,
                    Return = null,
                    Arguments = new List<string>()
                };
            }
            foreach (var nodeLink in frpGraphContainer.NodeLinks)
            {
                nodeDict[new Guid(nodeLink.BaseNodeGuid)].Return = nodeLink.ConnectionName;
                var targetNodeArguments = nodeDict[new Guid(nodeLink.TargetNodeGuid)].Arguments;
                while(targetNodeArguments.Count < nodeLink.TargetPortNum + 1)
                    targetNodeArguments.Add(null);
                targetNodeArguments[nodeLink.TargetPortNum] = nodeLink.ConnectionName;
            }
            return nodeDict;
        }

        public class NodeData
        {
            public string OperatorType;
            public string CodeText;
            public string Return;
            public List<string> Arguments;
        }
    }
}
