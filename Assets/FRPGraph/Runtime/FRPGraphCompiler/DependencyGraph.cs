using System;
using System.Collections.Generic;
using System.Linq;
using FRPGraph.Runtime.GraphAlgorithms;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace FRPGraph.Runtime
{
    public class DependencyGraph
    {
        private List<Node<Guid>> _adjacencyList;
        private Dictionary<Edge, int> _edgeToPort;

        public static DependencyGraph Create(FrpGraphContainer frpGraphContainer)
        {
            var dictionary = new Dictionary<Guid, Node<Guid>>();
            var edgeToPort = new Dictionary<Edge, int>();
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
                edgeToPort.Add(new Edge(baseGuid, targetGuid), nodeLinkData.BasePortNum);
            }

            return new DependencyGraph
            {
                _adjacencyList = dictionary.Values.ToList(),
                _edgeToPort = edgeToPort
            };
        }

        private static Dictionary<Guid, Node<Guid>> AdjacencyListToDict(List<Node<Guid>> _adjacencyList)
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
            return dict;
        }

        public List<Guid> TopologicalSort()
        {
            var dict = AdjacencyListToDict(_adjacencyList);
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

        public class EdgePortPair
        {
            private Edge _edge;
            private int _port;
            public Edge Edge => _edge;
            public int Port => _port;

            public EdgePortPair(Edge edge, int port)
            {
                _edge = edge;
                _port = port;
            }
        }

        public List<EdgePortPair> CutToAcyclicGraph()
        {
            var guid2LongMap = new Dictionary<Guid, long>();
            var long2GuidMap = new Dictionary<long, Guid>();
            
            foreach (var (guid, index) in _adjacencyList.Select((node, index) => (node.Id, index)))
            {
                guid2LongMap.Add(guid, index);
                long2GuidMap.Add(index, guid);
            }
            
            AllCyclesInDirectedGraphJohnson johnson = new AllCyclesInDirectedGraphJohnson();
            Graph<int> graph = new Graph<int>(true);

            foreach (var node in _adjacencyList)
            {
                foreach (var toNode in node.OutputLinks)
                {
                    graph.addEdge(guid2LongMap[node.Id], guid2LongMap[toNode.Id]);
                }
                
            }
            List<List<Vertex<int>>> allCycles = johnson.SimpleCycles(graph);
            var allCyclesGuid = new List<List<Guid>>();
            foreach (var cycle in allCycles)
            {
                var guidCycle = cycle.Select(vertex => long2GuidMap[vertex.GetId()]).ToList();
                allCyclesGuid.Add(guidCycle);
            }
            /*
             * 変更箇所
             */
            //var cutEdge = new Edge(allCyclesGuid.Last()[0], allCyclesGuid.Last()[1]);
            
            //Cut
            var cutEdges = ResolveAllCycle(allCyclesGuid);

            var edgePortPair = new List<EdgePortPair>();
            foreach (var cutEdge in cutEdges)
            {
                RemoveEdge(cutEdge.Vertex1, cutEdge.Vertex2);
                int port = _edgeToPort[cutEdge];
                edgePortPair.Add(new EdgePortPair(cutEdge, port));
            }
            return edgePortPair;
        }

        private static List<Edge> ResolveAllCycle(List<List<Guid>> cycles)
        {
            var edgeMultiplicities = new Dictionary<Edge, Tuple<HashSet<int>, Edge>>();
            var remainingCycle = new HashSet<int>();
            var cutEdges = new List<Edge>();
            foreach (var (cycle, index) in cycles.Select((list, index) => (list, index)))
            {
                var cycleStr = cycle.Select(val => val.ToString()).Aggregate((acc, val) => $"{acc} -> {val}");
                Debug.Log(cycleStr);
                for (int i = 0; i < cycle.Count - 1; ++i)
                {
                    var edge = new Edge(cycle[i], cycle[i+1]);
                    var multiplicity = edgeMultiplicities.ContainsKey(edge)
                        ? edgeMultiplicities[edge]
                        : edgeMultiplicities[edge] = new Tuple<HashSet<int>, Edge>(new HashSet<int>(), edge);
                    multiplicity.Item1.Add(index);
                }
                remainingCycle.Add(index);
            }

            while (remainingCycle.Count != 0)
            {
                var mostMultipleEdge = edgeMultiplicities.Values.OrderByDescending(x => x.Item1.Count).First();
                cutEdges.Add(mostMultipleEdge.Item2);
                Debug.Log($"Cut {mostMultipleEdge.Item2.Vertex1} -> {mostMultipleEdge.Item2.Vertex2}");
                var cycleIndices = mostMultipleEdge.Item1.ToList();
                foreach (var cycleIndex in cycleIndices)
                {
                    var cycle = cycles[cycleIndex];
                    for (int i = 0; i < cycle.Count - 1; ++i)
                    {
                        var edge = new Edge(cycle[i], cycle[i+1]);
                        var multiplicity = edgeMultiplicities[edge];
                        multiplicity.Item1.Remove(cycleIndex);
                    }
                    if (remainingCycle.Contains(cycleIndex))
                        remainingCycle.Remove(cycleIndex);
                }
            }
            return cutEdges;
        }

        private void RemoveEdge(Guid vertex1, Guid vertex2)
        {
            var dict = AdjacencyListToDict(_adjacencyList);
            var node1 = dict.ContainsKey(vertex1)
                ? dict[vertex1]
                : dict[vertex1] = new Node<Guid>(vertex1);
            var node2 = dict.ContainsKey(vertex2)
                ? dict[vertex2]
                : dict[vertex2] = new Node<Guid>(vertex2);
            node1.OutputLinks.Remove(node2);
            node2.InputLinks.Remove(node1);
            _adjacencyList = dict.Values.ToList();
        }

        private void AddEdge(Guid vertex1, Guid vertex2)
        {
            var dict = AdjacencyListToDict(_adjacencyList);
            var node1 = dict.ContainsKey(vertex1)
                ? dict[vertex1]
                : dict[vertex1] = new Node<Guid>(vertex1);
            var node2 = dict.ContainsKey(vertex2)
                ? dict[vertex2]
                : dict[vertex2] = new Node<Guid>(vertex2);
            node1.OutputLinks.Add(node2);
            node2.InputLinks.Add(node1);
            _adjacencyList = dict.Values.ToList();
        }

        public class Edge
        {
            private Guid _vertex1;

            public Guid Vertex1 => _vertex1;

            public Guid Vertex2 => _vertex2;

            private Guid _vertex2;
            public Edge(Guid vertex1, Guid vertex2)
            {
                _vertex1 = vertex1;
                _vertex2 = vertex2;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (obj.GetType() != GetType()) return false;
                var target = (Edge) obj;
                return target._vertex1 == _vertex1 && target._vertex2 == _vertex2;
            }

            public override int GetHashCode()
            {
                return _vertex1.GetHashCode() + _vertex2.GetHashCode();
            }

            public override string ToString()
            {
                return $"[{_vertex1.ToString()} -> {_vertex2.ToString()}]";
            }
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
