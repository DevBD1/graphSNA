using graphSNA.Model.Foundation;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Abstract base class for all graph algorithms.
    /// Provides common functionality such as neighbor lookup, input validation, and edge weight retrieval.
    /// Implements Template Method pattern for consistent algorithm execution across BFS, DFS, Dijkstra, and A*.
    /// </summary>
    public abstract class AlgorithmBase
    {
        // Display name of the algorithm (e.g., "BFS", "Dijkstra")
        public abstract string Name { get; }

        // Time complexity notation (e.g., "O(V + E)")
        public abstract string TimeComplexity { get; }

        // Validates that the graph and required nodes are not null
        protected bool ValidateInput(Graph graph, params Node[] requiredNodes)
        {
            if (graph == null || graph.Nodes.Count == 0)
                return false;

            foreach (var node in requiredNodes)
            {
                if (node == null)
                    return false;
            }

            return true;
        }

        // Finds all neighboring nodes connected to the specified node (works for undirected graphs)
        protected IEnumerable<Node> GetNeighbors(Graph graph, Node node)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node)
                    yield return edge.Target;
                else if (edge.Target == node)
                    yield return edge.Source;
            }
        }

        // Gets the weight of the edge between two nodes (returns infinity if no edge exists)
        protected double GetEdgeWeight(Graph graph, Node nodeA, Node nodeB)
        {
            foreach (var edge in graph.Edges)
            {
                bool isMatch = (edge.Source == nodeA && edge.Target == nodeB) ||
                               (edge.Source == nodeB && edge.Target == nodeA);

                if (isMatch)
                    return edge.Weight;
            }

            return double.PositiveInfinity;
        }
    }
}