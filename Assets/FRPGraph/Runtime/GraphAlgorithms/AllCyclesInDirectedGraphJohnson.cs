using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FRPGraph.Runtime.GraphAlgorithms
{
    public class AllCyclesInDirectedGraphJohnson
    {
        private HashSet<Vertex<int>> blockedSet;
        private Dictionary<Vertex<int>, HashSet<Vertex<int>>> blockedMap;
        private Stack<Vertex<int>> stack;
        private List<List<Vertex<int>>> allCycles;

        public List<List<Vertex<int>>> SimpleCycles(Graph<int> graph)
        {
            blockedSet = new HashSet<Vertex<int>>();
            blockedMap = new Dictionary<Vertex<int>, HashSet<Vertex<int>>>();
            stack = new Stack<Vertex<int>>();
            allCycles = new List<List<Vertex<int>>>();
            long startIndex = 1;
            TarjanStronglyConnectedComponent tarjan = new TarjanStronglyConnectedComponent();
            while (startIndex <= graph.GetAllVertex().Count())
            {
                Graph<int> subGraph = createSubGraph(startIndex, graph);
                List<HashSet<Vertex<int>>> sccs = tarjan.scc(subGraph);

                Vertex<int> maybeLeastVertex = leastIndexSCC(sccs, subGraph);
                if (maybeLeastVertex != null)
                {
                    Vertex<int> leastVertex = maybeLeastVertex;
                    blockedSet.Clear();
                    blockedMap.Clear();
                    findCyclesInSCG(leastVertex, leastVertex);
                    startIndex = leastVertex.GetId() + 1;
                }
                else
                {
                    break;
                }
            }

            return allCycles;
        }

        private Vertex<int> leastIndexSCC(List<HashSet<Vertex<int>>> sccs, Graph<int> subGraph)
        {
            long min = int.MaxValue;
            Vertex<int> minVertex = null;
            HashSet<Vertex<int>> minScc = null;
            foreach (var scc in sccs)
            {
                if (scc.Count() == 1) continue;
                foreach (var vertex in scc)
                {
                    if (vertex.GetId() < min)
                    {
                        min = vertex.GetId();
                        minVertex = vertex;
                        minScc = scc;
                    }
                }
            }

            if (minVertex == null)
            {
                return null;
            }

            Graph<int> graphScc = new Graph<int>(true);
            foreach (var edge in subGraph.GetAllEdges())
            {
                if (minScc.Contains(edge.GetVertex1()) && minScc.Contains(edge.GetVertex2()))
                {
                    graphScc.addEdge(edge.GetVertex1().GetId(), edge.GetVertex2().GetId());
                }
            }

            return graphScc.GetVertex(minVertex.GetId());
        }

        private void unblock(Vertex<int> u)
        {
            blockedSet.Remove(u);
            if (blockedMap.ContainsKey(u))
            {
                foreach (var v in blockedMap[u])
                {
                    if (blockedSet.Contains(v)) unblock(v);
                }

                blockedMap.Remove(u);
            }
        }

        private bool findCyclesInSCG(Vertex<int> startVertex, Vertex<int> currentVertex)
        {
            bool foundCycle = false;
            stack.Push(currentVertex);
            blockedSet.Add(currentVertex);

            foreach (var e in currentVertex.GetEdges())
            {
                Vertex<int> neighbor = e.GetVertex2();

                if (neighbor == startVertex)
                {
                    List<Vertex<int>> cycle = new List<Vertex<int>>();
                    stack.Push(startVertex);
                    cycle.AddRange(stack);
                    cycle.Reverse();
                    stack.Pop();
                    allCycles.Add(cycle);
                    foundCycle = true;
                }
                else if (!blockedSet.Contains(neighbor))
                {
                    bool gotCycle = findCyclesInSCG(startVertex, neighbor);
                    foundCycle = foundCycle || gotCycle;
                }
            }

            if (foundCycle)
            {
                unblock(currentVertex);
            }
            else
            {
                foreach (var e in currentVertex.GetEdges())
                {
                    Vertex<int> w = e.GetVertex2();
                    HashSet<Vertex<int>> bSet = getBSet(w);
                    bSet.Add(currentVertex);
                }
            }

            stack.Pop();
            return foundCycle;
        }

        private HashSet<Vertex<int>> getBSet(Vertex<int> v)
        {
            if(!blockedMap.ContainsKey(v))
                    blockedMap.Add(v, new HashSet<Vertex<int>>());
            return blockedMap[v];
        }

        private Graph<int> createSubGraph(long startVertex, Graph<int> graph)
        {
            Graph<int> subGraph = new Graph<int>(true);
            foreach (var edge in graph.GetAllEdges())
            {
                if (edge.GetVertex1().GetId() >= startVertex && edge.GetVertex2().GetId() >= startVertex)
                {
                    subGraph.addEdge(edge.GetVertex1().GetId(), edge.GetVertex2().GetId());
                }
            }

            return subGraph;
        }

        public static void Test()
        {
            AllCyclesInDirectedGraphJohnson johnson = new AllCyclesInDirectedGraphJohnson();
            Graph<int> graph = new Graph<int>(true);
            graph.addEdge(1, 2);
            graph.addEdge(2, 3);
            graph.addEdge(3, 4);
            graph.addEdge(4, 5);
            graph.addEdge(5, 1);
            graph.addEdge(2, 6);
            graph.addEdge(6, 7);
            graph.addEdge(7, 8);
            graph.addEdge(8, 1);
            List<List<Vertex<int>>> allCycles = johnson.SimpleCycles(graph);
            foreach (var cycle in allCycles)
            {
                var str = cycle.Select(vertex => vertex.GetId().ToString())
                    .Aggregate((acc, val) => $"{acc} -> {val}");
                Debug.Log(str);
            }
        }
    }
}
