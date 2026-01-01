using System;
using System.Drawing;
using System.Collections.Generic;

namespace graphSNA.Model.Foundation
{
    public class Layout
    {
        // İtme ve Çekme Kuvveti Ayarları
        private double k; // İdeal yay uzunluğu
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
            // Eski: k = Math.Sqrt(area / (graph.Nodes.Count + 1));
            // Yeni: Sonuna * 0.6 ekledik. Bu, grafiği %40 daha dar bir alana sıkıştırır.
            k = Math.Sqrt(area / (graph.Nodes.Count + 1)) * 0.6;

            // Başlangıç sıcaklığı (Simulated Annealing)
            double temperature = width / 10.0;

            Dictionary<Node, PointF> displacements = new Dictionary<Node, PointF>();

            for (int i = 0; i < iterations; i++)
            {
                // 1. İTME KUVVETLERİ (Repulsion)
                foreach (var v in graph.Nodes)
                {
                    displacements[v] = PointF.Empty; // reset
                    foreach (var u in graph.Nodes)
                    {
                        if (v == u) continue;
                        double dx = v.Location.X - u.Location.X;
                        double dy = v.Location.Y - u.Location.Y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);
                        if (dist < 0.1) dist = 0.1; // 0'a bölünme hatasını önle
                        // İtme Formülü: Fr = k^2 / dist
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

                    // --- FORMÜL DEĞİŞİKLİĞİ ---
                    // Weight (0 ile 1 arası).
                    // Karesini alıyoruz ki (Weight^2), yüksek ağırlıklar ÇOK daha etkili olsun.
                    // Çarpanı 20.0 yaptık. Weight=1 olanlar 21 kat güçlü çekecek!
                    double weightFactor = 1.0 + (edge.Weight * edge.Weight * 20.0);

                    // Normal kuvveti bu faktörle çarpıyoruz
                    double force = ((dist * dist) / k) * weightFactor;

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
                    // Eski: double gravityForce = 0.05 * k;
                    // Yeni: 0.20 yaptık. (Merkeze çekim gücü 4 kat arttı, dağılmayı önler)
                    double gravityForce = 0.20 * k;

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
                        // Hareketi sıcaklıkla sınırla (Titremeyi önler)
                        double limitedLength = Math.Min(length, temperature);
                        
                        disp.X = (float)(disp.X / length * limitedLength);
                        disp.Y = (float)(disp.Y / length * limitedLength);

                        // Yeni koordinatları ata (Sınırlandırma/Clamp YOK - Özgürce dağılsınlar)
                        int newX = (int)(v.Location.X + disp.X);
                        int newY = (int)(v.Location.Y + disp.Y);

                        v.Location = new Point(newX, newY);
                    }
                }

                // Soğutma
                temperature *= 0.95;
            }
        }

        // Düğüm boyutu (Referans için, hesaplamada kritik değil)
        private const int NodeSize = 30;
    }
}