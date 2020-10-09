using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Runtime;
using UnityEngine;

namespace FRPGraph.Runtime
{
    public class DependencyGraph
    {
        private List<Node> _adjacencyList;

        public static DependencyGraph Create(FrpGraphContainer frpGraphContainer)
        {
            var dictionary = new Dictionary<Guid, Node>();
            foreach (var nodeLinkData in frpGraphContainer.NodeLinks)
            {
                Guid baseGuid, targetGuid;
                try
                {
                    baseGuid = new Guid(nodeLinkData.BaseNodeGuid);
                    targetGuid = new Guid(nodeLinkData.TargetNodeGuid);
                }
                catch (Exception e) when (
                    e is ArgumentNullException
                    || e is FormatException
                    || e is OverflowException)
                {
                    Debug.Log($"Invalid Guid string format.\n{e}");
                    return null;
                }

                var baseNode = dictionary.ContainsKey(baseGuid)
                    ? dictionary[baseGuid]
                    : dictionary[baseGuid] = new Node(baseGuid);
                var targetNode = dictionary.ContainsKey(targetGuid)
                    ? dictionary[targetGuid]
                    : dictionary[targetGuid] = new Node(targetGuid);
                baseNode.OutputLinks.Add(targetNode);
                targetNode.InputLinks.Add(baseNode);
            }
            
            return new DependencyGraph
            {
                _adjacencyList = dictionary.Values.ToList(),
            };
        }

        public List<Guid> TopologicalSort()
        {
            var dict = new Dictionary<Guid, Node>();
            foreach (var node in _adjacencyList)
            {
                var newNode = dict.ContainsKey(node.Guid)
                    ? dict[node.Guid]
                    : dict[node.Guid] = new Node(node.Guid);
                foreach (var inputNode in node.InputLinks)
                {
                    var newInputNode = dict.ContainsKey(inputNode.Guid)
                        ? dict[inputNode.Guid]
                        : dict[inputNode.Guid] = new Node(inputNode.Guid);
                    newNode.InputLinks.Add(newInputNode);
                }
                foreach (var outputNode in node.OutputLinks)
                {
                    var newOutputNode = dict.ContainsKey(outputNode.Guid)
                        ? dict[outputNode.Guid]
                        : dict[outputNode.Guid] = new Node(outputNode.Guid);
                    newNode.OutputLinks.Add(newOutputNode);
                }
            }
            var list = dict.Values.ToList();
            var rootNodes = new Queue<Node>(list.FindAll(node => node.InputLinks.Count == 0));
            var resultList = new List<Guid>();
            while (rootNodes.Count != 0)
            {
                var rootNode = rootNodes.Dequeue();
                resultList.Add(rootNode.Guid);
                var nextNodes = rootNode.OutputLinks.ToList();
                foreach (var nextNode in nextNodes)
                {
                    rootNode.OutputLinks.Remove(nextNode);
                    nextNode.InputLinks.Remove(rootNode);
                    if (nextNode.InputLinks.Count == 0)
                    {
                        rootNodes.Enqueue(nextNode);
                    }
                }
            }
            var isDag = list.All(node => node.InputLinks.Count == 0 && node.OutputLinks.Count == 0);
            if (!isDag)
            {
                throw new RuntimeException("Graph is not DAG");
            }
            
            return resultList;
        }

        private class Node
        {
            public readonly Guid Guid;
            public readonly List<Node> OutputLinks = new List<Node>();
            public readonly List<Node> InputLinks = new List<Node>();

            public Node(Guid guid)
            {
                Guid = guid;
            }
        }
    }
}
