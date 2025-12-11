using System.Diagnostics;
using graphSNA.Model;

namespace graphSNA
{
    public partial class Form1 : Form
    {
        Graph myGraph = new Graph();
        public Form1()
        {
            InitializeComponent();

            //RunBackendTest();

            InitializeGraphData();

            pnlGraph.Paint += PnlGraph_Paint;
        }

        private void RunBackendTest()
        {
            // Create two test Nodes (Users)
            // Name, Activity, Interaction, ConnectionCount
            Node u1 = new Node("Ali", 10, 5, 2);
            Node u2 = new Node("Veli", 20, 10, 5);

            // Add them to the graph
            myGraph.AddNode(u1);
            myGraph.AddNode(u2);

            // Create a connection (Edge)
            myGraph.AddEdge(u1, u2);

            // Print results to the "Output" window in Visual Studio
            Debug.WriteLine("---------------- TEST START ----------------");
            Debug.WriteLine($"Total Nodes: {myGraph.Nodes.Count}"); // Should be 2
            Debug.WriteLine($"Total Edges: {myGraph.Edges.Count}"); // Should be 1
            Debug.WriteLine($"Edge Weight: {myGraph.Edges[0].Weight}"); // Should be 1.0 (default)
            Debug.WriteLine("---------------- TEST END ------------------");

            // Show the result as POP-UP
            string sonucMesaji = $"TEST BAŞARILI!\n\n" +
                                 $"Toplam Düğüm (Nodes): {myGraph.Nodes.Count}\n" +
                                 $"Toplam Kenar (Edges): {myGraph.Edges.Count}\n" +
                                 $"Kenar Ağırlığı: {myGraph.Edges[0].Weight}";

            MessageBox.Show(sonucMesaji, "graphSNA Backend Testi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void InitializeGraphData()
        {
            // Test verileri (Konumları elle veriyoruz şimdilik)
            Node u1 = new Node("Ali", 10, 5, 2);
            u1.Location = new Point(100, 100); // Sol üstte

            Node u2 = new Node("Veli", 20, 10, 5);
            u2.Location = new Point(300, 200); // Biraz aşağıda ve sağda

            myGraph.AddNode(u1);
            myGraph.AddNode(u2);
            myGraph.AddEdge(u1, u2);
        }

        private void PnlGraph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            // Çizim Kalitesini Artır (Kırtıklı kenarları düzeltir - AntiAlias)
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 1. Önce KENARLARI (Çizgileri) Çiz (Düğümlerin altında kalsın diye)
            Pen edgePen = new Pen(Color.Gray, 2); // Gri renk, 2px kalınlık

            foreach (Edge edge in myGraph.Edges)
            {
                // Başlangıç ve Bitiş noktaları arasına çizgi çek
                // Merkezden merkeze çizmek için +15 (yarıçap) ekliyoruz
                Point p1 = new Point(edge.Source.Location.X + 15, edge.Source.Location.Y + 15);
                Point p2 = new Point(edge.Target.Location.X + 15, edge.Target.Location.Y + 15);
                
                g.DrawLine(edgePen, p1, p2);
            }

            // 2. Sonra DÜĞÜMLERİ (Daireleri) Çiz
            Brush nodeBrush = Brushes.LightBlue; // Dairenin içi
            Pen nodeBorder = Pens.Black;         // Dairenin kenarı
            Font font = new Font("Arial", 10, FontStyle.Bold); // İsim fontu
            Brush textBrush = Brushes.Black;     // İsim rengi

            foreach (Node node in myGraph.Nodes)
            {
                // Daireyi çiz (30x30 boyutunda)
                g.FillEllipse(nodeBrush, node.Location.X, node.Location.Y, 30, 30);
                g.DrawEllipse(nodeBorder, node.Location.X, node.Location.Y, 30, 30);

                // İsmi yaz (Dairenin hemen üstüne)
                g.DrawString(node.Name, font, textBrush, node.Location.X, node.Location.Y - 20);
            }
        }
    }
}
