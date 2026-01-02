using graphSNA.Model.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Depth-First Search (DFS) traversal algorithm.
    /// Explores as far as possible along each branch before backtracking.
    /// Uses a Stack (LIFO - Last In First Out) data structure.
    /// </summary>
    public class DFS : AlgorithmBase, IGraphTraversal
    {
        public override string Name => "DFS (Depth-First Search)";
        public override string TimeComplexity => "O(V + E)";

        public List<Node> Traverse(Graph graph, Node startNode)
        {
            var result = new List<Node>();

            if (!ValidateInput(graph, startNode))
                return result;

            var visited = new HashSet<Node>();
            var stack = new Stack<Node>();

            stack.Push(startNode);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!visited.Contains(current))
                {
                    visited.Add(current);
                    result.Add(current);

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
    }
}