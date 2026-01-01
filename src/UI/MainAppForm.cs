using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using graphSNA.Model.Foundation;

namespace graphSNA.UI
{
    public partial class MainAppForm : Form
    {
        // Global Variables
        GraphController controller;
        Node selectedNode = null;
        Node startNodeForPathFinding = null;
        Node endNodeForPathFinding = null;
        bool isSelectingNodesForPathFinding = false;
        bool isSelectingForTraversal = false;
        private ContextMenuStrip graphContextMenu; // Sağ tık menüsü
        private Point lastRightClickPoint; // Tıklanan yerin koordinatı
        private Point _rightMouseDownLocation; // Sağ tıkın başladığı yer: sag tik ile pan ve menu olaylarini ayirmak icin

        private Node edgeSourceNode = null; // Bağlantının başladığı düğüm
        private Point currentMousePos;      // Çizim yaparken farenin anlık yeri (Hayalet çizgi için)
        private bool isDraggingEdge = false; // Şu an kablo çekiyor muyuz?
        private Edge selectedEdge = null;    // Sağ tıklanan Edge

        // Visual settings
        private const int NodeRadius = 8;
        private const int NodeSize = 16;

        // --- ZOOM & PAN VARIABLES ---
        private float zoomFactor = 1.0f;
        private float panOffsetX = 0.0f;
        private float panOffsetY = 0.0f;
        private Point panStartPoint;           // Pan işleminin başladığı nokta
        private bool isPanning = false;        // Şu an kaydırma yapıyor muyuz?
        // ----------------------------

        public MainAppForm()
        {
            InitializeComponent();

            // Flicker Fix
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });

            controller = new GraphController();

            RegisterEvents();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // --- WIRE UP MOUSE EVENTS FOR PAN & ZOOM ---
            this.panel1.MouseWheel += Canvas_MouseWheel;
            this.panel1.MouseDown += Canvas_MouseDown;
            this.panel1.MouseMove += Canvas_MouseMove;
            this.panel1.MouseUp += Canvas_MouseUp;
            // Focus panel on enter so wheel works immediately
            this.panel1.MouseEnter += (s, e) => panel1.Focus();

            // Calls localization method (stub at bottom if missing)
            UpdateUITexts();

            InitializeContextMenu();
        }

        // --- ZOOM LOGIC ---
        private void Canvas_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) zoomFactor *= 1.1f;
            else zoomFactor /= 1.1f;

            // Clamp zoom
            if (zoomFactor < 0.1f) zoomFactor = 0.1f;
            if (zoomFactor > 5.0f) zoomFactor = 5.0f;

            panel1.Invalidate();
        }
        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            Graph graphToDraw = controller.ActiveGraph;
            if (graphToDraw == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Move (Pan)
            g.TranslateTransform(panOffsetX, panOffsetY);
            // 2. Zoom
            g.ScaleTransform(zoomFactor, zoomFactor);

            // ========================================================================
            // 1. DRAW EDGES (KENARLARI ÇİZ)
            // ========================================================================

            // Sayıları yazmak için font (Küçük ve okunaklı)
            using (Font weightFont = new Font("Arial", 7, FontStyle.Regular))
            {
                foreach (Edge edge in graphToDraw.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);

                    // A. KALINLIK: Weight'e göre (1px - 5px arası)
                    float thickness = 1.0f + (float)(edge.Weight * 4.0f);
                    if (thickness > 8) thickness = 8; // Çok aşırı kalınlaşmasın

                    // B. RENK: Standart Gri (Röntgen modunu kapattık)
                    Color edgeColor = Color.FromArgb(180, 160, 160, 160);

                    // Çizgiyi Çiz
                    using (Pen dynamicPen = new Pen(edgeColor, thickness))
                    {
                        dynamicPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        dynamicPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        g.DrawLine(dynamicPen, p1, p2);
                    }

                    // =========================================================
                    // C. SAYI YAZDIRMA (GÜNCELLENEN KISIM) 🛠️
                    // =========================================================
                    // Sadece kutucuk işaretliyse sayıları yaz!
                    if (chkShowWeights.Checked)
                    {
                        // Çizginin tam orta noktasını bul
                        float midX = (p1.X + p2.X) / 2;
                        float midY = (p1.Y + p2.Y) / 2;

                        string weightText = edge.Weight.ToString("0.00");
                        SizeF textSize = g.MeasureString(weightText, weightFont);

                        RectangleF textRect = new RectangleF(
                            midX - (textSize.Width / 2),
                            midY - (textSize.Height / 2),
                            textSize.Width,
                            textSize.Height);

                        g.FillRectangle(Brushes.WhiteSmoke, textRect);
                        g.DrawString(weightText, weightFont, Brushes.Black, textRect.Location);
                    }
                }
            }

            // ========================================================================
            // 2. DRAW NODES (DÜĞÜMLERİ ÇİZ - AYNI KALDI)
            // ========================================================================
            Font font = new Font("Arial", 8, FontStyle.Regular);

            foreach (Node node in graphToDraw.Nodes)
            {
                Color finalColor = node.Color;
                Pen borderPen = Pens.Black;

                if (node == startNodeForPathFinding)
                {
                    finalColor = Color.LightGreen;
                    borderPen = new Pen(Color.DarkGreen, 2);
                }
                else if (node == endNodeForPathFinding)
                {
                    finalColor = Color.LightCoral;
                    borderPen = new Pen(Color.DarkRed, 2);
                }
                else if (node == selectedNode)
                {
                    finalColor = Color.Yellow;
                    borderPen = new Pen(Color.Red, 2);
                }

                using (Brush fillBrush = new SolidBrush(finalColor))
                {
                    g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                }

                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X - 5, node.Location.Y - 15);
            }

            // ========================================================================
            // 3. GHOST LINE (HAYALET ÇİZGİ - AYNI KALDI)
            // ========================================================================
            if (isDraggingEdge && edgeSourceNode != null)
            {
                // 1. Farenin Ekran Konumunu -> Dünya Konumuna (World Coords) Çevir
                float mouseWorldX = (currentMousePos.X - panOffsetX) / zoomFactor;
                float mouseWorldY = (currentMousePos.Y - panOffsetY) / zoomFactor;

                PointF targetPoint = new PointF(mouseWorldX, mouseWorldY);

                // 2. Kaynak Düğümün Merkezini Bul
                PointF sourcePoint = new PointF(
                    edgeSourceNode.Location.X + NodeRadius,
                    edgeSourceNode.Location.Y + NodeRadius
                );

                // 3. Çizgiyi Çiz
                using (Pen ghostPen = new Pen(Color.Red, 2))
                {
                    ghostPen.DashStyle = DashStyle.Dot;
                    g.DrawLine(ghostPen, sourcePoint, targetPoint);
                }
            }
        }
        // Koordinat Dönüşüm Yardımcısı (Eğer yoksa ekleyin, varsa kullanın)
        private Point ApplyTransform(Point worldPoint)
        {
            return new Point(
                (int)(worldPoint.X * zoomFactor + panOffsetX),
                (int)(worldPoint.Y * zoomFactor + panOffsetY)
            );
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isPanning && !isDraggingEdge)
            {
                // Koordinat çevirimi
                float worldX = (e.X - panOffsetX) / zoomFactor;
                float worldY = (e.Y - panOffsetY) / zoomFactor;
                Point worldPoint = new Point((int)worldX, (int)worldY);

                // Altında bir şey var mı?
                // Edge için geniş toleransı (15f) burada da kullanıyoruz
                bool isOverNode = controller.FindNodeAtPoint(worldPoint, NodeRadius) != null;
                bool isOverEdge = controller.FindEdgeAtPoint(worldPoint, 15f) != null;

                if (isOverNode || isOverEdge)
                    panel1.Cursor = Cursors.Hand; // Tıklanabilir el işareti 👆
                else
                    panel1.Cursor = Cursors.Default;
            }
            if (isDraggingEdge)
            {
                currentMousePos = e.Location; // Farenin ekran koordinatı
                panel1.Invalidate(); // Çizgiyi tekrar çiz
                return;
            }
            if (isPanning)
            {
                // Calculate how much the mouse moved
                float deltaX = e.X - panStartPoint.X;
                float deltaY = e.Y - panStartPoint.Y;

                // Update the pan offset
                panOffsetX += deltaX;
                panOffsetY += deltaY;

                panStartPoint = e.Location;
                panel1.Invalidate(); // Redraw with new position
            }
        }
        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            // Eğer az önce Pan yaptıysak veya CTRL basılıysa MENÜ AÇMA!
            if (isPanning || isDraggingEdge || Control.ModifierKeys == Keys.Control) return;

            // --- KOORDİNAT HESAPLARI ---
            float worldX = (e.X - panOffsetX) / zoomFactor;
            float worldY = (e.Y - panOffsetY) / zoomFactor;
            Point worldPoint = new Point((int)worldX, (int)worldY);

            Node clickedNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);
            Edge clickedEdge = controller.FindEdgeAtPoint(worldPoint);

            // --- SAĞ TIK (Context Menu) ---
            if (e.Button == MouseButtons.Right)
            {

                lastRightClickPoint = worldPoint; // Yeni düğüm eklenecekse buraya eklensin
                // Sağ tık da bir seçimdir ama "Görsel Seçim" yapmasın, sadece menüyü hazırlasın
                // İstersen sağ tıkla seçimi kaldırmak için: selectedNode = clickedNode; satırını silebilirsin.
                // Ama genelde sağ tıklanan öğe üzerinde işlem yapılır:
                selectedNode = clickedNode;

                // 1. DÜĞÜM MENÜSÜ
                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    // Menüyü ayarla (Sadece Node işlemleri)
                    graphContextMenu.Items[0].Visible = false; // Ekle
                    graphContextMenu.Items[1].Visible = true;  // Node Sil
                    graphContextMenu.Items[2].Visible = true;  // Düzenle
                                                               // Edge Silme butonu varsa gizle
                    if (graphContextMenu.Items.Count > 3) graphContextMenu.Items[3].Visible = false;

                    graphContextMenu.Show(panel1, e.Location);
                }
                // 2. EDGE MENÜSÜ (Yeni!)
                else if (clickedEdge != null)
                {
                    selectedEdge = clickedEdge;
                    // Menüye "Edge Sil" şıkkı eklemeliyiz.
                    // (Aşağıda menüye dinamik ekleme yapacağız)

                    ContextMenuStrip edgeMenu = new ContextMenuStrip();
                    edgeMenu.Items.Add("Bağlantıyı Sil").Click += (s, args) => {
                        controller.RemoveEdge(selectedEdge.Source, selectedEdge.Target);
                        panel1.Invalidate();
                    };
                    edgeMenu.Show(panel1, e.Location);
                }
                // 3. BOŞLUK (Node Ekle)
                else
                {
                    graphContextMenu.Items[0].Visible = true;
                    graphContextMenu.Items[1].Visible = false;
                    graphContextMenu.Items[2].Visible = false;
                    if (graphContextMenu.Items.Count > 3) graphContextMenu.Items[3].Visible = false;

                    lastRightClickPoint = worldPoint;
                    graphContextMenu.Show(panel1, e.Location);
                }
                return;
            }

            // =================================================================
            // 🖱️ SOL TIKLAMA (Sadece Seçim ve Panel Güncelleme)
            // =================================================================
            if (e.Button == MouseButtons.Left)
            {
                // 1. Yol Bulma Modu mu?
                if (isSelectingNodesForPathFinding)
                {
                    HandleShortestPathSelection(clickedNode);
                }
                // 2. Gezinti (Traversal) Modu mu?
                else if (isSelectingForTraversal)
                {
                    if (clickedNode != null)
                    {
                        selectedNode = clickedNode;
                        panel1.Invalidate();
                        RunTraversalAlgorithm(clickedNode);
                        isSelectingForTraversal = false;
                    }
                }
                // 3. Normal Seçim Modu
                else
                {
                    selectedNode = clickedNode; // Tıklananı seç (veya boşluğu)

                    if (selectedNode != null)
                    {
                        // ARTIK MESSAGEBOX YOK! Paneli güncelle.
                        UpdateNodeInfoPanel(selectedNode);
                    }
                    else
                    {
                        // Boşluğa tıklandı -> Paneli temizle
                        ClearNodeInfoPanel();
                    }

                    panel1.Invalidate(); // Seçim rengini (Sarı) güncellemek için çiz
                }
            }
        }
        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDraggingEdge)
            {
                // Bırakılan yerde düğüm var mı?
                float worldX = (e.X - panOffsetX) / zoomFactor;
                float worldY = (e.Y - panOffsetY) / zoomFactor;
                Point worldPoint = new Point((int)worldX, (int)worldY);
                Node targetNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);

                // Geçerli bir hedef mi?
                if (targetNode != null && targetNode != edgeSourceNode)
                {
                    // Controller'a sor: "Ekleyebildin mi?"
                    bool success = controller.AddEdge(edgeSourceNode, targetNode);

                    if (success)
                    {
                        MessageBox.Show($"Bağlantı kuruldu: {edgeSourceNode.Name} <-> {targetNode.Name}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Bu bağlantı zaten mevcut!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Temizlik
                isDraggingEdge = false;
                edgeSourceNode = null;
                panel1.Invalidate();
                return;
            }
            if (isPanning)
            {
                isPanning = false;
                panel1.Cursor = Cursors.Default; // İmleci normale döndür
            }
        }
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            // Koordinat Çevirimi
            float worldX = (e.X - panOffsetX) / zoomFactor;
            float worldY = (e.Y - panOffsetY) / zoomFactor;
            Point worldPoint = new Point((int)worldX, (int)worldY);
            Node clickedNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);

            // --- EDGE BAŞLATMA ---
            // Şart: Sol Tık + SHIFT basılı + Bir düğüme tıklandı
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift && clickedNode != null)
            {
                isDraggingEdge = true;
                edgeSourceNode = clickedNode;
                currentMousePos = e.Location; // Başlangıç noktası
                return;
            }
            // 1. PAN BAŞLATMA (CTRL + SAĞ TIK)
            if ((e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) && Control.ModifierKeys == Keys.Control)
            {
                isPanning = true;
                panStartPoint = e.Location; // Farenin ilk bastığı yer
                panel1.Cursor = Cursors.SizeAll; // Visual feedback
            }

            // 2. SAĞ TIK MENÜ HAZIRLIĞI (SADECE SAĞ TIK)
            else if (e.Button == MouseButtons.Right)
            {
                // CTRL basılı değilse, bu bir menü açma isteğidir.
                // Konumu kaydedelim (MouseClick'te kullanacağız)
                _rightMouseDownLocation = e.Location;
            }
        }
        private void ShowNodeDetails(Node node)
        {
            MessageBox.Show($"Name: {node.Name}\nActivity: {node.Activity}\nInteraction: {node.Interaction}", "Node Details");
        }
        private void ClearNodeDetails() { }
        // Seçili düğümün bilgilerini sağ panele yazar
        private void UpdateNodeInfoPanel(Node node)
        {
            // TextBox isimlerini kendi projenizdekilerle eşleştirin
            textBox1.Text = node.Name;
            textBox2.Text = node.Activity.ToString();
            textBox3.Text = node.Interaction.ToString();

            // Sil ve Düzenle butonlarını aktif et
            // btnDeleteNode.Enabled = true; (Eğer butonlarınız varsa)
            // btnEditNode.Enabled = true;
        }

        // Paneli temizler (Boşluğa tıklayınca)
        private void ClearNodeInfoPanel()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }
    }
}