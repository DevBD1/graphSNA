using System;
using System.Drawing;
using System.Collections.Generic;

namespace graphSNA.Model
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

            // İdeal mesafe: Alanın büyüklüğüne ve düğüm sayısına göre belirlenir
            double area = width * height;
            k = Math.Sqrt(area / (graph.Nodes.Count + 1));

            // Hareket Vektörleri (dx, dy)
            Dictionary<Node, PointF> displacements = new Dictionary<Node, PointF>();

            // --- FİZİK DÖNGÜSÜ ---
            for (int i = 0; i < iterations; i++)
            {
                // 1. İTME KUVVETLERİ (Her düğüm diğerini iter)
                foreach (var v in graph.Nodes)
                {
                    displacements[v] = PointF.Empty; // Sıfırla

                    foreach (var u in graph.Nodes)
                    {
                        if (v == u) continue;

                        // Mesafe vektörü
                        double dx = v.Location.X - u.Location.X;
                        double dy = v.Location.Y - u.Location.Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist < 0.1) dist = 0.1; // Sıfıra bölünme hatasını önle

                        // İtme Formülü: Fr = k^2 / dist
                        double force = (k * k) / dist;

                        // Kuvveti uygula
                        float dispX = (float)(dx / dist * force);
                        float dispY = (float)(dy / dist * force);

                        displacements[v] = new PointF(
                            displacements[v].X + dispX,
                            displacements[v].Y + dispY
                        );
                    }
                }

                // 2. ÇEKME KUVVETLERİ (Sadece bağlı olanlar çeker)
                foreach (var edge in graph.Edges)
                {
                    Node v = edge.Source;
                    Node u = edge.Target;

                    double dx = v.Location.X - u.Location.X;
                    double dy = v.Location.Y - u.Location.Y;
                    double dist = Math.Sqrt(dx * dx + dy * dy);

                    if (dist < 0.1) dist = 0.1;

                    // Çekme Formülü: Fa = dist^2 / k
                    double force = (dist * dist) / k;

                    float dispX = (float)(dx / dist * force);
                    float dispY = (float)(dy / dist * force);

                    // Kaynak düğüm hedefe çekilir (Eksi)
                    displacements[v] = new PointF(
                        displacements[v].X - dispX,
                        displacements[v].Y - dispY
                    );

                    // Hedef düğüm kaynağa çekilir (Artı)
                    displacements[u] = new PointF(
                        displacements[u].X + dispX,
                        displacements[u].Y + dispY
                    );
                }

                // 3. KONUMLARI GÜNCELLE
                foreach (var v in graph.Nodes)
                {
                    PointF disp = displacements[v];

                    // Hareketi uygula (Sıcaklık/Azalma faktörü eklenebilir ama şimdilik direkt uyguluyoruz)
                    // Hız sınırlaması (Çok uzağa fırlamasınlar)
                    double length = Math.Sqrt(disp.X * disp.X + disp.Y * disp.Y);
                    if (length > 100) // Maksimum hız
                    {
                        disp.X = (float)(disp.X / length * 100);
                        disp.Y = (float)(disp.Y / length * 100);
                    }

                    int newX = (int)(v.Location.X + disp.X);
                    int newY = (int)(v.Location.Y + disp.Y);

                    // Sınırların dışına çıkmayı engelle
                    newX = Math.Max(NodeSize, Math.Min(AreaWidth - NodeSize, newX));
                    newY = Math.Max(NodeSize, Math.Min(AreaHeight - NodeSize, newY));

                    v.Location = new Point(newX, newY);
                }
            }
        }

        private const int NodeSize = 30; // Kenarlardan taşmaması için
    }
}