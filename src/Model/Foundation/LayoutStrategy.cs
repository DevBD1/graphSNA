using System;
using System.Drawing;

namespace graphSNA.Model.Foundation
{
    // Sorumluluk: Düğümleri ekrana matematiksel olarak yerleştirmek.
    public static class LayoutStrategy
    {
        public static void ApplyCircularLayout(Graph graph, int centerX, int centerY, int radius)
        {
            int count = graph.Nodes.Count;
            if (count == 0) return;

            for (int i = 0; i < count; i++)
            {
                // Trigonometri hesabı
                double angle = 2.0 * Math.PI * i / count;

                int x = (int)(centerX + radius * Math.Cos(angle));
                int y = (int)(centerY + radius * Math.Sin(angle));

                graph.Nodes[i].Location = new Point(x, y);
            }
        }
    }
}