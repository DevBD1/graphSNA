using graphSNA.Model.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Depth-First Search (DFS) traversal algorithm.
    /// Explores as far as possible along each branch before backtracking.
    /// Uses a Stack (LIFO - Last In First Out) data structure.
    /// Time Complexity: O(V + E) where V = vertices, E = edges.
    /// </summary>
    public class DFS : IGraphTraversal
    {
        public List<Node> Traverse(Graph graph, Node startNode)
        {
            // Track which nodes we've already visited (prevents infinite loops)
            var visited = new HashSet<Node>();
            
            // Stack: Last In First Out - we always process the most recently added node
            // This is what makes DFS go "deep" before going "wide"
            var stack = new Stack<Node>();
            
            // Result list: nodes in the order they were visited
            var result = new List<Node>();

            // Safety check
            if (startNode == null || graph == null) return result;

            // Start with the initial node
            stack.Push(startNode);

            while (stack.Count > 0)
            {
                // Pop: get and remove the top element from stack
                var current = stack.Pop();

                // Skip if already visited (can happen in undirected graphs)
                if (!visited.Contains(current))
                {
                    // Mark as visited and add to result
                    visited.Add(current);
                    result.Add(current);

                    // Get all neighbors and reverse them
                    // Reverse ensures we visit neighbors in natural order (left to right)
                    // because Stack reverses the order again when popping
                    var neighbors = GetNeighbors(graph, current).ToList();
                    neighbors.Reverse();

                    // Add unvisited neighbors to stack
                    foreach (var neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }
            }

            return result;
        }

        // Helper method: finds all nodes connected to the given node
        private IEnumerable<Node> GetNeighbors(Graph graph, Node node)
        {
            // Check each edge to find connections
            foreach (var edge in graph.Edges)
            {
                // Undirected graph: check both directions
                if (edge.Source == node) 
                    yield return edge.Target;  // node -> target
                else if (edge.Target == node) 
                    yield return edge.Source;  // source -> node (reverse direction)
            }
        }
    }
}