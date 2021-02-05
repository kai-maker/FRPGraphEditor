using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FRPGraph.Runtime
{
    public class DependencyGraph
    {
        private List<Node<Guid>> _adjacencyList;

        public static DependencyGraph Create(FrpGraphContainer frpGraphContainer)
        {
            var dictionary = new Dictionary<Guid, Node<Guid>>();
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
                    Debug.Log($"Invalid Id string format.\n{e}");
                    return null;
                }

                var baseNode = dictionary.ContainsKey(baseGuid)
                    ? dictionary[baseGuid]
                    : dictionary[baseGuid] = new Node<Guid>(baseGuid);
                var targetNode = dictionary.ContainsKey(targetGuid)
                    ? dictionary[targetGuid]
                    : dictionary[targetGuid] = new Node<Guid>(targetGuid);
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
            var dict = new Dictionary<Guid, Node<Guid>>();
            foreach (var node in _adjacencyList)
            {
                var newNode = dict.ContainsKey(node.Id)
                    ? dict[node.Id]
                    : dict[node.Id] = new Node<Guid>(node.Id);
                foreach (var inputNode in node.InputLinks)
                {
                    var newInputNode = dict.ContainsKey(inputNode.Id)
                        ? dict[inputNode.Id]
                        : dict[inputNode.Id] = new Node<Guid>(inputNode.Id);
                    newNode.InputLinks.Add(newInputNode);
                }
                foreach (var outputNode in node.OutputLinks)
                {
                    var newOutputNode = dict.ContainsKey(outputNode.Id)
                        ? dict[outputNode.Id]
                        : dict[outputNode.Id] = new Node<Guid>(outputNode.Id);
                    newNode.OutputLinks.Add(newOutputNode);
                }
            }
            var list = dict.Values.ToList();
            var rootNodes = new Queue<Node<Guid>>(list.FindAll(node => node.InputLinks.Count == 0));
            var resultList = new List<Guid>();
            while (rootNodes.Count != 0)
            {
                var rootNode = rootNodes.Dequeue();
                resultList.Add(rootNode.Id);
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
                throw new Exception("Graph is not DAG");
            }
            
            return resultList;
        }

        public void CutToAcyclicGraph()
        {
            var dict = new Dictionary<Guid, Node<Guid>>();
            foreach (var node in _adjacencyList)
            {
                var newNode = dict.ContainsKey(node.Id)
                    ? dict[node.Id]
                    : dict[node.Id] = new Node<Guid>(node.Id);
                foreach (var inputNode in node.InputLinks)
                {
                    var newInputNode = dict.ContainsKey(inputNode.Id)
                        ? dict[inputNode.Id]
                        : dict[inputNode.Id] = new Node<Guid>(inputNode.Id);
                    newNode.InputLinks.Add(newInputNode);
                }
                foreach (var outputNode in node.OutputLinks)
                {
                    var newOutputNode = dict.ContainsKey(outputNode.Id)
                        ? dict[outputNode.Id]
                        : dict[outputNode.Id] = new Node<Guid>(outputNode.Id);
                    newNode.OutputLinks.Add(newOutputNode);
                }
            }
            var list = dict.Values.ToList();
        }

        private class Node<T>
        {
            public readonly T Id;
            public readonly List<Node<T>> OutputLinks = new List<Node<T>>();
            public readonly List<Node<T>> InputLinks = new List<Node<T>>();

            public Node(T id)
            {
                Id = id;
            }
        }
    }
}
