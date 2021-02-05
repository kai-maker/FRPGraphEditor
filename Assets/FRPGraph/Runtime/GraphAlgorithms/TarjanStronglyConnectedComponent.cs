using System;
using System.Collections.Generic;

namespace FRPGraph.Runtime.GraphAlgorithms
{
    /**
     * Original source code from
     * https://github.com/mission-peace/interview/tree/master/src/com/interview/graph
     * by Tushar Roy
     * 
     * Ported original Java source code to C-Sharp
     * by Kouich Matsumura
     */
    
    /**
     * Date 08/16/2015
     * @author Tushar Roy
     *
     * Find strongly connected components of directed graph.
     *
     * Time complexity is O(E + V)
     * Space complexity  is O(V)
     *
     * Reference - https://en.wikipedia.org/wiki/Tarjan%27s_strongly_connected_components_algorithm
     */
    public class TarjanStronglyConnectedComponent
    {
        private Dictionary<Vertex<int>, int> visitedTime;
        private Dictionary<Vertex<int>, int> lowTime;
        private HashSet<Vertex<int>> onStack;
        private Stack<Vertex<int>> stack;
        private HashSet<Vertex<int>> visited;
        private List<HashSet<Vertex<int>>> result;
        private int time;

        public List<HashSet<Vertex<int>>> scc(Graph<int> graph)
        {
            time = 0;
            
            visitedTime = new Dictionary<Vertex<int>, int>();
            
            lowTime = new Dictionary<Vertex<int>, int>();
            
            onStack = new HashSet<Vertex<int>>();
            
            stack = new Stack<Vertex<int>>();
            
            visited = new HashSet<Vertex<int>>();
            
            result = new List<HashSet<Vertex<int>>>();

            foreach (Vertex<int> vertex in graph.GetAllVertex())
            {
                if (visited.Contains(vertex))
                {
                    continue;
                }
                sccUtil(vertex);
            }
            return result;
        }

        private void sccUtil(Vertex<int> vertex)
        {
            visited.Add(vertex);
            visitedTime.Add(vertex, time);
            lowTime.Add(vertex, time);
            time++;
            stack.Push(vertex);
            onStack.Add(vertex);

            foreach (var child in vertex.GetAdjacentVertexes())
            {
                if (!visited.Contains(child))
                {
                    sccUtil(child);

                    lowTime[vertex] = Math.Min(lowTime[vertex], lowTime[child]);
                }
                else if (onStack.Contains(child))
                {
                    lowTime[vertex] = Math.Min(lowTime[vertex], visitedTime[child]);
                }
            }

            if (visitedTime[vertex] == lowTime[vertex])
            {
                HashSet<Vertex<int>> stronglyConnectedComponent = new HashSet<Vertex<int>>();
                Vertex<int> v;
                do
                {
                    v = stack.Pop();
                    onStack.Remove(v);
                    stronglyConnectedComponent.Add(v);
                } while (!vertex.Equals(v));
                result.Add(stronglyConnectedComponent);
            }
        }
        public static void Test(string[] args) {
            Graph<int> graph = new Graph<int>(true);
            graph.addEdge(1,2);
            graph.addEdge(2,3);
            graph.addEdge(3,1);
            graph.addEdge(3,4);
            graph.addEdge(4,5);
            graph.addEdge(5,6);
            graph.addEdge(6,4);
            graph.addEdge(7,6);
            graph.addEdge(7,8);
            graph.addEdge(8,7);

            TarjanStronglyConnectedComponent tarjanStronglyConnectedComponent = new TarjanStronglyConnectedComponent();
            List<HashSet<Vertex<int>>> result = tarjanStronglyConnectedComponent.scc(graph);

            result.ForEach(scc =>
            {
                foreach (var vertex in scc)
                {
                    Console.Write(vertex + " ");
                }
                Console.WriteLine();
            });
        }
    }
}