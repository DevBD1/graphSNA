using graphSNA.Model.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace graphSNA.Model.Algorithms
{
    // Depth-First Search implementation (Iterative approach)
    public class DFS : IGraphTraversal
    {
        public List<Node> Traverse(Graph graph, Node startNode)
        {
            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();
            var result = new List<Node>();

            if (startNode == null || graph == null) return result;

            stack.Push(startNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);

                    // Get neighbors and reverse to maintain natural order in Stack
                    var neighbors = GetNeighbors(graph, current).ToList();
                    neighbors.Reverse();

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