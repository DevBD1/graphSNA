using graphSNA.Model.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace graphSNA.Model.Algorithms
{
    // Breadth-First Search implementation
    public class BFS : IGraphTraversal
    {
        public List<Node> Traverse(Graph graph, Node startNode)
        {
            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();
            var result = new List<Node>();

            if (startNode == null || graph == null) return result;

            queue.Enqueue(startNode);
            visited.Add(startNode);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current);

                // Get neighbors dynamically
                foreach (var neighbor in GetNeighbors(graph, current))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return result;
        }

        // Helper to find connected nodes in undirected graph
        private IEnumerable<Node> GetNeighbors(Graph graph, Node node)
        {
            foreach (var edge in graph.Edges)
            {
                if (edge.Source == node) yield return edge.Target;
                else if (edge.Target == node) yield return edge.Source;
            }
        }
    }
}