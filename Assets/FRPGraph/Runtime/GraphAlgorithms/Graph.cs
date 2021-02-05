using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
    public class Graph<T>
    {
        private List<Edge<T>> allEdges;
        private Dictionary<long, Vertex<T>> allVertex;
        private bool isDirected = false;

        public Graph(bool isDirected)
        {
            allEdges = new List<Edge<T>>();
            allVertex = new Dictionary<long, Vertex<T>>();
            this.isDirected = isDirected;
        }

        public void addEdge(long id1, long id2)
        {
            addEdge(id1, id2, 0);
        }

        public void addEdge(long id1, long id2, int weight)
        {
            Vertex<T> vertex1 = null;
            if (allVertex.ContainsKey(id1))
            {
                vertex1 = allVertex[id1];
            }
            else
            {
                vertex1 = new Vertex<T>(id1);
                allVertex.Add(id1, vertex1);
            }
            Vertex<T> vertex2 = null;
            if (allVertex.ContainsKey(id2))
            {
                vertex2 = allVertex[id2];
            }
            else
            {
                vertex2 = new Vertex<T>(id2);
                allVertex.Add(id2, vertex2);
            }
            
            var edge = new Edge<T>(vertex1, vertex2, isDirected, weight);
            allEdges.Add(edge);
            vertex1.addAdjacentVertex(edge, vertex2);
            if (!isDirected)
            {
                vertex2.addAdjacentVertex(edge, vertex1);
            }
        }

        public List<Edge<T>> GetAllEdges() => allEdges;
        
        public Vertex<T> GetVertex(long id) => allVertex[id];

        public IEnumerable<Vertex<T>> GetAllVertex()
        {
            return allVertex.Values;
        }
        
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            foreach(var edge in GetAllEdges())
            {
                buffer.Append(edge.GetVertex1() + " " + edge.GetVertex2() + " " + edge.GetWeight());
                buffer.Append("\n");
            }
            return buffer.ToString();
        }
    }
    
    public class Vertex<T>
    {
        public long id;
        private T data;
        private List<Edge<T>> edges = new List<Edge<T>>();
        private List<Vertex<T>> adjacentVertex = new List<Vertex<T>>();

        public Vertex(long id)
        {
            this.id = id;
        }

        public long GetId() => id;

        public void SetData(T data)
        {
            this.data = data;
        }

        public T GetData() => data;

        public void addAdjacentVertex(Edge<T> e, Vertex<T> v)
        {
            edges.Add(e);
            adjacentVertex.Add(v);
        }

        public override string ToString() => id.ToString();

        public List<Vertex<T>> GetAdjacentVertexes() => adjacentVertex;
        public List<Edge<T>> GetEdges() => edges;

        public int GetDegree() => edges.Count;

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + (int) (id ^ (id >> 32));
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            var other = (Vertex<T>) obj;
            return id == other.id;
        }
    }

    public class Edge<T>
    {
        private bool isDirected = false;
        private Vertex<T> vertex1;
        private Vertex<T> vertex2;
        private int weight;

        public Edge(Vertex<T> vertex1, Vertex<T> vertex2, bool isDirected, int weight)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.weight = weight;
            this.isDirected = isDirected;
        }

        public Vertex<T> GetVertex1() => vertex1;
        public Vertex<T> GetVertex2() => vertex2;

        public int GetWeight() => weight;

        public bool IsDirected() => isDirected;

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + ((vertex1 == null) ? 0 : vertex1.GetHashCode());
            result = prime * result + ((vertex2 == null) ? 0 : vertex2.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;
            var other = (Edge<T>) obj;
            if (vertex1 == null) {
                if (other.vertex1 != null)
                    return false;
            } else if (!vertex1.Equals(other.vertex1))
                return false;
            if (vertex2 == null) {
                if (other.vertex2 != null)
                    return false;
            } else if (!vertex2.Equals(other.vertex2))
                return false;
            return true;
        }

        public override string ToString()
        {
            return "Edge [isDirected=" + isDirected + ", vertex1=" + vertex1
                   + ", vertex2=" + vertex2 + ", weight=" + weight + "]";
        }
    }
}
