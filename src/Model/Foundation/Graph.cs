using System.Collections.Generic;
using System.Linq;

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

        // Adds a new user (node) to the network
        public void AddNode(Node node)
        {
            // Prevent adding duplicate nodes with the same ID
            if (Nodes.Any(n => n.Id == node.Id))
                return;

            Nodes.Add(node);
        }

        // Creates a connection between two nodes
        public void AddEdge(Node source, Node target)
        {
            // Prevent self-loops
            if (source == target)
                return;

            // Check if edge already exists (undirected graph)
            bool exists = Edges.Any(e =>
                (e.Source == source && e.Target == target) ||
                (e.Source == target && e.Target == source));

            if (exists)
                return;

            Edge newEdge = new Edge(source, target);
            Edges.Add(newEdge);
        }

        // Removes a node and all its connected edges
        public void RemoveNode(Node nodeToRemove)
        {
            if (nodeToRemove == null) return;

            // Remove all edges connected to this node (iterate in reverse)
            for (int i = Edges.Count - 1; i >= 0; i--)
            {
                Edge e = Edges[i];
                if (e.Source == nodeToRemove || e.Target == nodeToRemove)
                {
                    Edges.RemoveAt(i);
                }
            }

            // Remove the node itself
            Nodes.Remove(nodeToRemove);
        }

        // Removes the edge between two nodes
        public void RemoveEdge(Node n1, Node n2)
        {
            Edge edgeToRemove = null;

            // Check both directions since the graph is undirected
            foreach (var e in Edges)
            {
                if ((e.Source == n1 && e.Target == n2) || (e.Source == n2 && e.Target == n1))
                {
                    edgeToRemove = e;
                    break;
                }
            }

            if (edgeToRemove != null)
            {
                Edges.Remove(edgeToRemove);
            }
        }

        // Returns adjacency matrix representation of the graph
        public (double[,] Matrix, List<Node> NodeOrder) GetAdjacencyMatrix()
        {
            int n = Nodes.Count;
            var matrix = new double[n, n];
            var nodeOrder = Nodes.ToList();
            var nodeIndex = new Dictionary<Node, int>();

            for (int i = 0; i < n; i++)
                nodeIndex[nodeOrder[i]] = i;

            // Fill matrix with edge weights (symmetric for undirected graph)
            foreach (var edge in Edges)
            {
                int i = nodeIndex[edge.Source];
                int j = nodeIndex[edge.Target];
                matrix[i, j] = edge.Weight;
                matrix[j, i] = edge.Weight;
            }

            return (matrix, nodeOrder);
        }
    }
}