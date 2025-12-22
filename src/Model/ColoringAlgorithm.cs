using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace graphSNA.Model
{
    public static class ColoringAlgorithm
    {
        private static readonly List<Color> Palette = new List<Color>
        {
            Color.Red, Color.Blue, Color.Green, Color.Orange,
            Color.Purple, Color.Brown, Color.Teal, Color.Magenta,
            Color.Gold, Color.Pink, Color.Lime, Color.Navy
        };

        public static int ApplyWelshPowell(Graph graph)
        {
            if (graph == null || graph.Nodes.Count == 0) return 0;

            // Sort nodes by degree (connection count) descending
            var sortedNodes = graph.Nodes.OrderByDescending(n => n.ConnectionCount).ToList();

            foreach (var node in graph.Nodes) node.Color = Color.LightGray;

            int colorIndex = 0;

            while (sortedNodes.Count > 0)
            {
                Color currentColor = Palette[colorIndex % Palette.Count];
                Node highestDegreeNode = sortedNodes[0];
                highestDegreeNode.Color = currentColor;

                var nodesColoredThisRound = new List<Node> { highestDegreeNode };

                for (int i = 1; i < sortedNodes.Count; i++)
                {
                    Node candidate = sortedNodes[i];
                    bool isNeighborToCurrentColor = false;

                    foreach (var colored in nodesColoredThisRound)
                    {
                        if (AreNeighbors(graph, candidate, colored))
                        {
                            isNeighborToCurrentColor = true;
                            break;
                        }
                    }

                    if (!isNeighborToCurrentColor)
                    {
                        candidate.Color = currentColor;
                        nodesColoredThisRound.Add(candidate);
                    }
                }

                foreach (var processed in nodesColoredThisRound)
                {
                    sortedNodes.Remove(processed);
                }

                colorIndex++;
            }

            return colorIndex;
        }

        private static bool AreNeighbors(Graph graph, Node n1, Node n2)
        {
            return graph.Edges.Any(e =>
                (e.Source == n1 && e.Target == n2) ||
                (e.Source == n2 && e.Target == n1));
        }
    }
}