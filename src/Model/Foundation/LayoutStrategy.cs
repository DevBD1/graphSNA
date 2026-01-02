using System;
using System.Drawing;

namespace graphSNA.Model.Foundation
{
    /// <summary>
    /// Provides simple mathematical layout strategies for initial node positioning.
    /// Used as fallback before force-directed layout is applied.
    /// </summary>
    public static class LayoutStrategy
    {
        // Arranges nodes in a circle around a center point
        public static void ApplyCircularLayout(Graph graph, int centerX, int centerY, int radius)
        {
            int count = graph.Nodes.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                // Calculate angle for each node (evenly distributed around circle)
                double angle = 2.0 * Math.PI * i / count;

                // Convert polar coordinates (angle, radius) to cartesian (x, y)
                int x = (int)(centerX + radius * Math.Cos(angle));
                int y = (int)(centerY + radius * Math.Sin(angle));

                graph.Nodes[i].Location = new Point(x, y);
            }
        }
    }
}