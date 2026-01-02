using graphSNA.Model.Foundation;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Breadth-First Search (BFS) traversal algorithm.
    /// Explores all neighbors at the current depth before moving to the next level.
    /// Uses a Queue (FIFO - First In First Out) data structure.
    /// </summary>
    public class BFS : AlgorithmBase, IGraphTraversal
    {
        public override string Name => "BFS (Breadth-First Search)";
        public override string TimeComplexity => "O(V + E)";

        public List<Node> Traverse(Graph graph, Node startNode)
        {
            var result = new List<Node>();

            if (!ValidateInput(graph, startNode))
                return result;

            var visited = new HashSet<Node>();
            var queue = new Queue<Node>();

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
    }
}