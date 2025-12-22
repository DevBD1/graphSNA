using System.Collections.Generic;

namespace graphSNA.Model
{
    // Common interface for graph traversal algorithms (Polymorphism requirement)
    public interface IGraphTraversal
    {
        List<Node> Traverse(Graph graph, Node startNode);
    }
}