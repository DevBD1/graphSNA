using graphSNA.Model;
using System;
using System.Drawing; // SetStyle
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace graphSNA.UI
{
    /// <summary>
    ///  Represents the main application UI form.
    /// </summary>
    public partial class MainAppForm : Form
    {
        // global variables & constants
        GraphController controller;
        Node selectedNode = null;
        private const int NodeRadius = 15;
        private const int NodeSize = 30;

        public MainAppForm()
        {
            InitializeComponent();

            // Controller'ı başlat
            controller = new GraphController();

            // SADECE TEK SATIR İLE TÜM BAĞLANTILARI YAP:
            RegisterEvents(); // <-- Diğer dosyadaki metodu çağırıyoruz!

            // Görsel ayarlar
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        // --- GÖRSEL VE ETKİLEŞİM ---

        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            // Veriyi Controller'dan istiyoruz
            Graph graphToDraw = controller.ActiveGraph;

            // Eğer graf yoksa veya boşsa çizme
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

        // IsPointInNode metodunu buradan SİLDİK. Çünkü artık Controller yapıyor.

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