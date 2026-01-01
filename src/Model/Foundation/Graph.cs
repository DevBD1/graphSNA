using System.Collections.Generic;

namespace graphSNA.Model.Foundation
{
    /// <summary>
    /// Manages the entire graph structure, including all nodes and edges.
    /// Acts as the central data source for the application.
    /// </summary>
    public class Graph
    {
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }

        public Graph()
        {
            Nodes = new List<Node>();
            Edges = new List<Edge>();
        }

        /// <summary>
        /// Adds a new user (node) to the network.
        /// </summary>
        public void AddNode(Node node)
        {
            // TODO: Check if a node with the same name already exists
            Nodes.Add(node);
        }

        /// <summary>
        /// Creates a connection between two nodes.
        /// </summary>
        public void AddEdge(Node source, Node target)
        {
            // TODO: Check for self-loops (source == target)
            // TODO: Check if the edge already exists
            Edge newEdge = new Edge(source, target);
            Edges.Add(newEdge);
        }

        /// <summary>
        /// Bir düğümü ve ona bağlı tüm kenarları siler.
        /// </summary>
        public void RemoveNode(Node nodeToRemove)
        {
            if (nodeToRemove == null) return;

            // 1. Önce bu düğüme bağlı tüm kenarları temizle
            // (Tersten döngü kuruyoruz ki liste bozulmasın)
            for (int i = Edges.Count - 1; i >= 0; i--)
            {
                Edge e = Edges[i];
                if (e.Source == nodeToRemove || e.Target == nodeToRemove)
                {
                    Edges.RemoveAt(i);
                }
            }

            // 2. Düğümü listeden çıkar
            Nodes.Remove(nodeToRemove);
        }

        /// <summary>
        /// İki düğüm arasındaki bağlantıyı siler.
        /// </summary>
        public void RemoveEdge(Node n1, Node n2)
        {
            Edge edgeToRemove = Edges.FirstOrDefault(e =>
                (e.Source == n1 && e.Target == n2) ||
                (e.Source == n2 && e.Target == n1));

            if (edgeToRemove != null)
            {
                Edges.Remove(edgeToRemove);
            }
        }
    }
}