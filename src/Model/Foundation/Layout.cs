using System;
using System.Drawing;
using System.Collections.Generic;

namespace graphSNA.Model.Foundation
{
    public class Layout
    {
        // İtme ve Çekme Kuvveti Ayarları
        private double k; // İdeal yay uzunluğu
        private const double RepulsionFactor = 50000.0; // İtme gücü
        private const double AttractionFactor = 0.05;   // Çekme gücü

        // Graf alanı sınırları
        private int AreaWidth;
        private int AreaHeight;

        public void CalculateLayout(Graph graph, int width, int height, int iterations = 100)
        {
            AreaWidth = width;
            AreaHeight = height;

            // Merkez Noktası (Yerçekimi için)
            float centerX = width / 2f;
            float centerY = height / 2f;

            // İdeal yay uzunluğu (Formülü koruyoruz)
            double area = width * height;
            k = Math.Sqrt(area / (graph.Nodes.Count + 1));

            // Başlangıç sıcaklığı (Simulated Annealing)
            double temperature = width / 10.0;

            Dictionary<Node, PointF> displacements = new Dictionary<Node, PointF>();

            for (int i = 0; i < iterations; i++)
            {
                // 1. İTME KUVVETLERİ (Repulsion)
                foreach (var v in graph.Nodes)
                {
                    displacements[v] = PointF.Empty;
                    foreach (var u in graph.Nodes)
                    {
                        if (v == u) continue;
                        double dx = v.Location.X - u.Location.X;
                        double dy = v.Location.Y - u.Location.Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);
                        if (dist < 0.1) dist = 0.1;

                        double force = (k * k) / dist;
                        displacements[v] = new PointF(
                            displacements[v].X + (float)(dx / dist * force),
                            displacements[v].Y + (float)(dy / dist * force)
                        );
                    }
                }

                // 2. ÇEKME KUVVETLERİ (Attraction - Yaylar)
                foreach (var edge in graph.Edges)
                {
                    Node v = edge.Source;
                    Node u = edge.Target;
                    double dx = v.Location.X - u.Location.X;
                    double dy = v.Location.Y - u.Location.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);
                    if (dist < 0.1) dist = 0.1;

                    double force = (dist * dist) / k;
                    float dispX = (float)(dx / dist * force);
                    float dispY = (float)(dy / dist * force);

                    displacements[v] = new PointF(displacements[v].X - dispX, displacements[v].Y - dispY);
                    displacements[u] = new PointF(displacements[u].X + dispX, displacements[u].Y + dispY);
                }

                // --- 3. YERÇEKİMİ (GRAVITY) - YENİ ---
                // Düğümlerin sonsuza dağılmasını engellemek için merkeze hafif çekim
                foreach (var v in graph.Nodes)
                {
                    double dx = centerX - v.Location.X;
                    double dy = centerY - v.Location.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    // Gravity gücü (Düğüm sayısı arttıkça artmalı ki dağılmasın)
                    double gravityForce = 0.05 * k;

                    if (dist > 0)
                    {
                        displacements[v] = new PointF(
                            displacements[v].X + (float)(dx / dist * gravityForce),
                            displacements[v].Y + (float)(dy / dist * gravityForce)
                        );
                    }
                }

                // 4. KONUMLARI GÜNCELLE (Sınırlandırma İptal!)
                foreach (var v in graph.Nodes)
                {
                    PointF disp = displacements[v];
                    double length = Math.Sqrt(disp.X * disp.X + disp.Y * disp.Y);

                    if (length > 0)
                    {
                        // Hareketi sıcaklıkla kısıtla
                        double limitedLength = Math.Min(length, temperature);

                        disp.X = (float)(disp.X / length * limitedLength);
                        disp.Y = (float)(disp.Y / length * limitedLength);

                        // --- DUVARLARI YIKTIK ---
                        // Artık Math.Max/Min yok. Koordinatlar eksiye veya width'in ötesine geçebilir.
                        // Zoom/Pan sistemimiz olduğu için bu sorun değil, aksine daha doğal durur.
                        int newX = (int)(v.Location.X + disp.X);
                        int newY = (int)(v.Location.Y + disp.Y);

                        v.Location = new Point(newX, newY);
                    }
                }

                // Soğutma
                temperature *= 0.95;
            }
        }

        private const int NodeSize = 30; // Kenarlardan taşmaması için
    }
}