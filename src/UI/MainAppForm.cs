using graphSNA.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D; // SmoothingMode için gerekli
using System.Windows.Forms;

namespace graphSNA.UI
{
    public partial class MainAppForm : Form
    {
        // global degiskenler
        Graph myGraph;
        Node selectedNode = null;

        // sabitler
        private const int NodeRadius = 15;
        private const int NodeSize = NodeRadius*2;

        public MainAppForm()
        {
            InitializeComponent();

            // 1. Önce veriyi hazırla (Graph nesnesi burada oluşuyor)
            InitializeGraphData();

            // 2. Olayları SADECE BURADA bağla (Tekrarı önlemek için)
            panel1.Paint += GraphCanvas_Paint;
            panel1.MouseClick += PnlGraph_MouseClick;

            // DoubleBuffered: Titremeyi (flickering) önler, çizimi yumuşatır
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        private void InitializeGraphData()
        {
            // KRİTİK DÜZELTME: Nesneyi oluşturmayı unutmayın!
            myGraph = new Graph();

            // Test verileri
            Node u1 = new Node("Ali", 10, 5, 2);
            u1.Location = new Point(100, 100);

            Node u2 = new Node("Veli", 20, 10, 5);
            u2.Location = new Point(300, 200);

            myGraph.AddNode(u1);
            myGraph.AddNode(u2);
            myGraph.AddEdge(u1, u2);

            // Buradaki event bağlama kodları silindi (Constructor'a taşındı).
        }

        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Null kontrolü: Eğer graf henüz oluşmadıysa çizim yapma
            if (myGraph == null) return;

            // 1. KENARLARI ÇİZ
            using (Pen edgePen = new Pen(Color.Gray, 2))
            {
                foreach (Edge edge in myGraph.Edges)
                {
                    // Merkezden merkeze çizmek için +NodeRadius ekliyoruz
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);

                    g.DrawLine(edgePen, p1, p2);
                }
            }

            // 2. DÜĞÜMLERİ ÇİZ
            Font font = new Font("Arial", 10, FontStyle.Bold);

            foreach (Node node in myGraph.Nodes)
            {
                // Varsayılan Renkler
                Brush fillBrush = Brushes.LightBlue;
                Pen borderPen = Pens.Black;

                // SEÇİLİ OLANI VURGULA
                if (node == selectedNode)
                {
                    fillBrush = Brushes.Yellow;
                    borderPen = new Pen(Color.Red, 3);
                }

                // Sabitleri kullanarak çizim
                g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);

                // İsmi yaz
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X, node.Location.Y - 20);
            }
        }

        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            if (myGraph == null) return;

            Point mousePt = e.Location;
            bool nodeFound = false;

            foreach (Node node in myGraph.Nodes)
            {
                if (IsPointInNode(mousePt, node))
                {
                    selectedNode = node;
                    nodeFound = true;
                    ShowNodeDetails(node);
                    break;
                }
            }

            // Boşluğa tıklandıysa seçimi kaldır
            if (!nodeFound)
            {
                selectedNode = null;
                ClearNodeDetails();
            }

            panel1.Invalidate(); // Yeniden çiz
        }

        private bool IsPointInNode(Point p, Node n)
        {
            // Sabit yarıçapı kullanıyoruz
            int centerX = n.Location.X + NodeRadius;
            int centerY = n.Location.Y + NodeRadius;

            // Pisagor
            double distance = Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2));

            return distance <= NodeRadius;
        }

        private void ShowNodeDetails(Node node)
        {
            // İleride burayı sağ paneldeki Label'lara bağlayacağız
            // Şimdilik MessageBox kalabilir
            string info = $"Ad: {node.Name}\nAktiflik: {node.Activity}\nEtkileşim: {node.Interaction}";
            MessageBox.Show(info, "Düğüm Bilgisi");
        }

        private void ClearNodeDetails()
        {
            // Sağ paneldeki label'ları temizlemek için burayı kullanacağız
        }
    }
}