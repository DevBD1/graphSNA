using graphSNA.Model.Foundation;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    /// <summary>
    /// Common interface for shortest path algorithms.
    /// Enables polymorphism between Dijkstra and A* implementations.
    /// </summary>
    public interface IShortestPathAlgorithm
    {
        (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal);
    }
}