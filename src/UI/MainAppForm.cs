using graphSNA.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D; /// SetStyle
using System.Windows.Forms;

namespace graphSNA.UI
{
    /// <summary>
    ///  Represents the main application UI form.
    /// </summary>
    public partial class MainAppForm : Form
    {
        // Global variables
        GraphController controller;
        Node selectedNode = null;
        // Constants for node radius
        private const int NodeRadius = 15;
        private const int NodeSize = 30;

        /// <summary>
        ///  <>
        /// </summary>
        public MainAppForm()
        {
            // Call the designer generated code (the default method by designer)
            InitializeComponent();

            // Initialize the Controller
            controller = new GraphController();

            // Call the event registration method from MainAppForm.Events.cs
            RegisterEvents();

            // Visual settings for smoother rendering
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Localization
            UpdateUITexts();
        }

        /// <summary>
        ///  Visualization & Communication
        /// </summary>
        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            // Fetch the data (active graph) from the controller
            Graph graphToDraw = controller.ActiveGraph;

            // Do not draw if graf is null
            if (graphToDraw == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. KENARLARI ÇİZ (Edges)
            using (Pen edgePen = new Pen(Color.WhiteSmoke, 2))
            {
                foreach (Edge edge in graphToDraw.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);
                    g.DrawLine(edgePen, p1, p2);
                }
            }

            // 2. DÜĞÜMLERİ ÇİZ (Nodes)
            Font font = new Font("Arial", 10, FontStyle.Bold);

            foreach (Node node in graphToDraw.Nodes)
            {
                // Varsayılan Renkler
                Brush fillBrush = Brushes.LightBlue;
                Pen borderPen = Pens.Black;

                // Seçili ise Vurgula
                if (node == selectedNode)
                {
                    fillBrush = Brushes.Yellow;
                    borderPen = new Pen(Color.Red, 3);
                }

                // Çizim
                g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);

                // İsim Yaz
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X, node.Location.Y - 20);
            }
        }

        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            // Hesaplama işini Controller'a soruyoruz
            Node clickedNode = controller.FindNodeAtPoint(e.Location, NodeRadius);

            if (clickedNode != null)
            {
                selectedNode = clickedNode;
                ShowNodeDetails(selectedNode);
            }
            else
            {
                selectedNode = null; // Boşluğa tıklarsa seçimi kaldır
                ClearNodeDetails();
            }

            // Ekranı yenile (Renk değişimi için)
            panel1.Invalidate();
        }

        private void ShowNodeDetails(Node node)
        {
            // Şimdilik Popup, ileride sağ paneldeki Label'lar olacak
            string info = $"Ad: {node.Name}\nAktiflik: {node.Activity}\nEtkileşim: {node.Interaction}";
            MessageBox.Show(info, "Düğüm Detayı");
        }

        private void ClearNodeDetails()
        {
            // Sağ panel temizleme kodu buraya gelecek
        }
    }
}