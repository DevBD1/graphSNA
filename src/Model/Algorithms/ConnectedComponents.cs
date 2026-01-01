using graphSNA.Model.Foundation;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    public static class ConnectedComponents
    {
        /// <summary>
        /// Finds all disconnected sub-graphs (islands) in the network.
        /// Required by project specifications (PDF 3.2).
        /// </summary>
        /// <param name="graph">The active graph to analyze.</param>
        /// <returns>A list of node lists, where each inner list represents a distinct connected component.</returns>
        public static List<List<Node>> FindComponents(Graph graph)
        {
            var components = new List<List<Node>>();
            var visited = new HashSet<Node>();

            if (graph == null || graph.Nodes.Count == 0)
                return components;

            // Iterate through all nodes to find unvisited ones (potential new islands)
            foreach (var node in graph.Nodes)
            {
                if (!visited.Contains(node))
                {
                    // Start discovering a new component
                    var newComponent = new List<Node>();
                    
                    // Use BFS (Breadth-First Search) to gather all nodes in this cluster
                    Queue<Node> queue = new Queue<Node>();
                    queue.Enqueue(node);
                    visited.Add(node);
                    newComponent.Add(node);

                    while (queue.Count > 0)
                    {
                        var current = queue.Dequeue();

                        // Find neighbors dynamically
                        foreach (var neighbor in GetNeighbors(graph, current))
                        {
                            if (!visited.Contains(neighbor))
                            {
                                visited.Add(neighbor);
                                newComponent.Add(neighbor);
                                queue.Enqueue(neighbor);
                            }
                        }
                    }

                    components.Add(newComponent);
                }
            }

            return components;
        }

        /// <summary>
        /// Helper method to find neighbors of a specific node.
        /// </summary>
        private static IEnumerable<Node> GetNeighbors(Graph graph, Node node)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) yield return edge.Target;
                else if (edge.Target == node) yield return edge.Source;
            }
        }
    }
}