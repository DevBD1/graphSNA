using graphSNA.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace graphSNA.UI
{
    /// <summary>
    ///  Represents the main application UI form.
    ///  (Core Logic: Variables, Constructor, Paint, MouseClick)
    /// </summary>
    public partial class MainAppForm : Form
    {
        // --- GLOBAL DEĞİŞKENLER ---
        GraphController controller;
        Node selectedNode = null;

        // Pathfinding Seçim Değişkenleri
        Node startNodeForPathFinding = null;
        Node endNodeForPathFinding = null;
        bool isSelectingNodesForPathFinding = false;

        // Traversal (BFS/DFS) Seçim Değişkenleri (YENİ)
        bool isSelectingForTraversal = false;

        // Görsel Ayarlar
        private const int NodeRadius = 15;
        private const int NodeSize = 30;

        public MainAppForm()
        {
            InitializeComponent();
            controller = new GraphController();

            // Burak'ın yapısındaki Events dosyasına gider
            RegisterEvents();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Burak'ın yapısındaki Localization dosyasına gider
            UpdateUITexts();
        }

        // --- ÇİZİM İŞLEMLERİ (PAINT) ---
        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graph graphToDraw = controller.ActiveGraph;
            if (graphToDraw == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Kenarları Çiz
            using (Pen edgePen = new Pen(Color.WhiteSmoke, 2))
            {
                foreach (Edge edge in graphToDraw.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);
                    g.DrawLine(edgePen, p1, p2);
                }
            }

            // 2. Düğümleri Çiz
            Font font = new Font("Arial", 10, FontStyle.Bold);

            foreach (Node node in graphToDraw.Nodes)
            {
                Brush fillBrush = Brushes.LightBlue;
                Pen borderPen = Pens.Black;

                // --- BOYAMA MANTIĞI ---
                if (node == startNodeForPathFinding) // Yol Başlangıcı (Yeşil)
                {
                    fillBrush = Brushes.LightGreen;
                    borderPen = new Pen(Color.DarkGreen, 3);
                }
                else if (node == endNodeForPathFinding) // Yol Bitişi (Kırmızı)
                {
                    fillBrush = Brushes.LightCoral;
                    borderPen = new Pen(Color.DarkRed, 3);
                }
                else if (node == selectedNode) // Normal Seçim (Sarı)
                {
                    fillBrush = Brushes.Yellow;
                    borderPen = new Pen(Color.Red, 3);
                }

                g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X, node.Location.Y - 20);
            }
        }

        // --- MOUSE TIKLAMA MANTIĞI ---
        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            Node clickedNode = controller.FindNodeAtPoint(e.Location, NodeRadius);

            // SENARYO 1: En Kısa Yol Seçimi
            if (isSelectingNodesForPathFinding)
            {
                HandleShortestPathSelection(clickedNode);
            }
            // SENARYO 2: BFS/DFS Tarama Seçimi (YENİ)
            else if (isSelectingForTraversal)
            {
                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    panel1.Invalidate();

                    // Algoritmayı Çalıştır (Events dosyasındaki metoda gidecek)
                    RunTraversalAlgorithm(clickedNode);

                    isSelectingForTraversal = false; // Modu kapat
                }
            }
            // SENARYO 3: Normal Seçim
            else
            {
                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    ShowNodeDetails(selectedNode);
                }
                else
                {
                    selectedNode = null;
                    ClearNodeDetails();
                }
                panel1.Invalidate();
            }
        }

        // Helper: Node Detay Gösterimi
        private void ShowNodeDetails(Node node)
        {
            MessageBox.Show($"Ad: {node.Name}\nAktiflik: {node.Activity}\nEtkileşim: {node.Interaction}", "Düğüm Detayı");
        }
        private void ClearNodeDetails() { }
    }
}