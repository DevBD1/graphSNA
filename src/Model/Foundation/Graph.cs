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
    }
}