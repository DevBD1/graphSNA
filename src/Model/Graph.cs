using System.Collections.Generic;

namespace graphSNA.Model
{
    ///
    /// Manages the entire graph structure, including all nodes and edges.
    /// Acts as the central data source for the application.
    ///
    public class Graph
    {
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }

        public Graph()
        {
            Nodes = new List<Node>();
            Edges = new List<Edge>();
        }

        ///
        /// Adds a new user (node) to the network.
        ///
        public void AddNode(Node node)
        {
            // TODO: Check if a node with the same name already exists
            Nodes.Add(node);
        }

        ///
        /// Creates a connection between two nodes.
        ///
        public void AddEdge(Node source, Node target)
        {
            // TODO: Check for self-loops (source == target)
            // TODO: Check if the edge already exists
            Edge newEdge = new Edge(source, target);
            Edges.Add(newEdge);
        }
    }
}