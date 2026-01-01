using graphSNA.Model.Foundation;
using System.Collections.Generic;

namespace graphSNA.Model.Algorithms
{
    public interface IShortestPathAlgorithm
    {
        (List<Node> path, double totalCost) FindPath(Graph graph, Node start, Node goal);
    }
}