namespace FRPGraph.Runtime.GraphAlgorithms
{
    public class Graph<T>
    {
        
    }
    
    class Vertex<T>
    {
        public long id;
        private T data;
    }

    class Edge<T>
    {
        private bool isDirected = false;
        private Vertex<T> vertex1;
        private Vertex<T> vertex2;
        private int weight;

        Edge(Vertex<T> vertex1, Vertex<T> vertex2, bool isDirected, int weight)
        {
            this.vertex1 = vertex1;
            this.vertex2 = vertex2;
            this.weight = weight;
            this.isDirected = isDirected;
        }

        Vertex<T> GetVertex1() => vertex1;
        Vertex<T> GetVertex2() => vertex2;

        private int getWeight() => weight;

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
