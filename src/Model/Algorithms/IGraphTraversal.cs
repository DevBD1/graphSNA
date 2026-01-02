using graphSNA.Model.Foundation;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Common interface for graph traversal algorithms.
    /// Enables polymorphism between BFS and DFS implementations.
    /// </summary>
    public interface IGraphTraversal
    {
        List<Node> Traverse(Graph graph, Node startNode);
    }
}