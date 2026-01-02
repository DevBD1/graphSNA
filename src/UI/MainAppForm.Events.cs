using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using graphSNA.Model.Foundation;

namespace graphSNA.UI
{
    /// <summary>
    ///  Handles Event Wiring and Button & Click Logic
    /// </summary>
    public partial class MainAppForm
    {
        private void RegisterEvents()
        {
            // Standard IO Operations
            this.button1.Click += ImportCSV;
            this.button2.Click += ExportCSV;

            // Canvas interactions
            this.panel1.Paint += Canvas_Paint;
            this.panel1.MouseMove += Canvas_MouseMove;
            this.panel1.MouseClick += Canvas_MouseClick;
            this.panel1.MouseUp += Canvas_MouseUp;
            this.panel1.MouseDown += Canvas_MouseDown;

            // Algorithm triggers
            this.btnFindShortestPath.Click += RunFindShortestPath;
            this.btnTraverse.Click += RunTraverse;
            this.btnColoring.Click += RunColoring;

            // Updating Statistics
            this.tabControl1.SelectedIndexChanged += TabSelectedIndexChanged;
            this.button9.Click += BtnRefreshStats_Click;
            // YENİ: Layout (Yeniden Dizilim) Butonu
            this.button3.Click += BtnApplyLayout_Click;
            this.button4.Click += BtnWeighted_Click;
            // CheckBox değiştiği an ekranı yeniden çiz (Invalidate)
            this.chkShowWeights.CheckedChanged += (s, e) => panel1.Invalidate();
            this.button5.Click += btnEditNode_Click;
            this.button6.Click += btnDeleteNode_Click;

        }

        // --- 1. FILE OPERATIONS ---
        private void ImportCSV(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV Files|*.csv" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    controller.LoadGraph(ofd.FileName);
                    groupBox1.Text = $"File: {Path.GetFileName(ofd.FileName)}";
                    controller.RecalculateAllWeights();
                    //Distributes nodes across the screen randomly and uniformly
                    controller.ApplyForceLayout(panel1.Width, panel1.Height);

                    //// Layout bittikten hemen sonra:
                    // 1. Sanal Genişliği Hesapla
                    int nodeCount = controller.ActiveGraph.Nodes.Count;
                    // Min 800px olsun, yoksa çok küçük graflar ezilmesin.
                    int virtualWidth = Math.Max(800, nodeCount * 20);
                    int virtualHeight = Math.Max(600, nodeCount * 20);

                    // 2. Fizik Motorunu Çalıştır
                    controller.ApplyForceLayout(virtualWidth, virtualHeight);

                    // --- 3. AKILLI ORTALAMA VE SIĞDIRMA (FIX) ---

                    // Grafiğin sanal merkezi (World Coordinates)
                    float graphCenterX = virtualWidth / 2.0f;
                    float graphCenterY = virtualHeight / 2.0f;

                    // Panelin merkezi (Screen Coordinates)
                    float panelCenterX = panel1.Width / 2.0f;
                    float panelCenterY = panel1.Height / 2.0f;

                    // Ekrana sığması için gereken Zoom oranını bul
                    float scaleX = (float)panel1.Width / virtualWidth;
                    float scaleY = (float)panel1.Height / virtualHeight;

                    // En kısıtlı kenara göre zoom yap (Kenarlardan %10 boşluk bırak: 0.9f)
                    zoomFactor = Math.Min(scaleX, scaleY) * 0.9f;

                    // Çok küçük dosyalarda devasa zoom yapmasın (Max Zoom 1.0 olsun)
                    if (zoomFactor > 1.0f) zoomFactor = 1.0f;
                    // Çok da küçülmesin (Min Zoom 0.1)
                    if (zoomFactor < 0.1f) zoomFactor = 0.1f;

                    // Pan (Kaydırma) Değerini Hesapla:
                    // Formül: HedefEkran = (Dünya * Zoom) + Pan
                    // Pan = HedefEkran - (Dünya * Zoom)
                    panOffsetX = panelCenterX - (graphCenterX * zoomFactor);
                    panOffsetY = panelCenterY - (graphCenterY * zoomFactor);

                    panel1.Invalidate();
                    MessageBox.Show(Properties.Resources.Msg_Success);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void ExportCSV(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV Files|*.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                controller.SaveGraph(sfd.FileName);
                MessageBox.Show("File saved successfully.");
            }
        }

        // --- 2. SHORTEST PATH LOGIC ---
        private void RunFindShortestPath(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 2)
            {
                MessageBox.Show("Please load a graph first.", "Warning");
                return;
            }

            isSelectingNodesForPathFinding = true;
            isSelectingForTraversal = false;
            startNodeForPathFinding = null;
            endNodeForPathFinding = null;

            MessageBox.Show("Please select the START node from the graph.", "Step 1");
        }
        private void HandleShortestPathSelection(Node clickedNode)
        {
            if (startNodeForPathFinding == null)
            {
                if (clickedNode != null)
                {
                    startNodeForPathFinding = clickedNode;
                    selectedNode = clickedNode;
                    MessageBox.Show($"Start: {clickedNode.Name}\nNow select the TARGET node.", "Step 2");
                }
            }
            else if (endNodeForPathFinding == null)
            {
                if (clickedNode != null && clickedNode != startNodeForPathFinding)
                {
                    endNodeForPathFinding = clickedNode;
                    isSelectingNodesForPathFinding = false;
                    RunShortestPathAlgorithm();
                }
                else if (clickedNode == startNodeForPathFinding)
                {
                    MessageBox.Show("Start and End nodes cannot be the same!", "Error");
                }
            }
            panel1.Invalidate();
        }
        private void RunShortestPathAlgorithm()
        {
            string algoType = radioAstar.Checked ? "A*" : "Dijkstra";
            var result = controller.CalculateShortestPath(startNodeForPathFinding, endNodeForPathFinding, algoType);

            if (result.path.Count > 0)
            {
                controller.HighlightedPath = result.path; // Store for visualization
                txtCost.Text = $"{result.cost:F2}";
                // MessageBox.Show removed for visualization
            }
            else
            {
                controller.HighlightedPath = null;
                txtCost.Text = "None";
                MessageBox.Show("No path found.", "Result");
            }
            panel1.Invalidate(); // Trigger redraw
        }
        
        // --- 3. TRAVERSAL (BFS/DFS) LOGIC ---
        private void RunTraverse(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 1)
            {
                MessageBox.Show("Please load a graph first.");
                return;
            }

            isSelectingForTraversal = true;
            isSelectingNodesForPathFinding = false;

            MessageBox.Show("Click on a START node to begin traversal.", "Traversal Mode");
        }
        private void RunTraversalAlgorithm(Node startNode)
        {
            string algo = radioDFS.Checked ? "DFS" : "BFS";
            List<Node> resultOrder = controller.TraverseGraph(startNode, algo);

            if (resultOrder.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Algorithm: {algo}");
                sb.AppendLine($"Start Node: {startNode.Name}");
                sb.AppendLine($"Visited: {resultOrder.Count} nodes");
                sb.AppendLine("--------------------------------");
                string flow = string.Join(" -> ", resultOrder.Select(n => n.Name));
                sb.AppendLine(flow);
                MessageBox.Show(sb.ToString(), "Traversal Result");
            }
        }
        
        // --- 4. WELSH-POWELL COLORING ---
        private void RunColoring(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Please load a graph first.", "Warning");
                return;
            }

            int colorCount = controller.ColorGraph();
            panel1.Invalidate();

            MessageBox.Show($"Graph colored successfully!\nChromatic Number: {colorCount}", "Completed");
        }

        // --- 5. STATISTICS (DEGREE CENTRALITY) ---
        private void TabSelectedIndexChanged(object sender, EventArgs e)
        {
            // Update table only when 'Stats' tab is active
            if (tabControl1.SelectedIndex == 1)
            {
                UpdateStatsTable();
            }
        }
        private void BtnRefreshStats_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null) return;
            UpdateStatsTable();
            MessageBox.Show("Liste güncellendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void UpdateStatsTable()
        {
            if (controller.ActiveGraph == null) return;

            var topNodes = controller.GetTopInfluencers(5);

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Columns
            dataGridView1.Columns.Add("Rank", "Sıra");
            dataGridView1.Columns.Add("Name", "İsim");
            dataGridView1.Columns.Add("Degree", "Derece");
            dataGridView1.Columns.Add("Score", "Merkezilik Skoru");

            int rank = 1;
            foreach (var node in topNodes)
            {
                double score = node.ConnectionCount * node.Interaction;
                dataGridView1.Rows.Add(rank++, node.Name, node.ConnectionCount, score.ToString("F2"));
            }

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // --- 6. CONTEXT MENU (RIGHT-CLICK) ---
        private void InitializeContextMenu()
        {
            graphContextMenu = new ContextMenuStrip();

            // --- MENÜ ÖĞELERİNİ OLUŞTUR ---
            var itemAdd = new ToolStripMenuItem("Yeni Kişi Ekle");
            var itemDeleteNode = new ToolStripMenuItem("Seçili Kişiyi Sil");
            var itemEditNode = new ToolStripMenuItem("Düzenle (Özellikler)");
            var itemDeleteEdge = new ToolStripMenuItem("Bağlantıyı Sil"); // Yeni!

            // --- 1. YENİ KİŞİ EKLE ---
            itemAdd.Click += (s, e) => {
                // Yeni form yapımız (Komşusuz, sade)
                InputNodeForm form = new InputNodeForm("Yeni Kişi Ekle");

                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Controller üzerinden ekle (lastRightClickPoint: sağ tıklanan yer)
                    controller.AddNode(form.NodeName, form.Activity, form.Interaction, 0, lastRightClickPoint);
                    panel1.Invalidate();
                }
            };

            // --- 2. KİŞİYİ SİL ---
            itemDeleteNode.Click += (s, e) => {
                if (selectedNode != null)
                {
                    var result = MessageBox.Show(
                        $"'{selectedNode.Name}' kişisini ve bağlantılarını silmek istiyor musunuz?",
                        "Silme Onayı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        controller.RemoveNode(selectedNode);
                        selectedNode = null;
                        ClearNodeInfoPanel(); // Sağ paneli temizle
                        panel1.Invalidate();
                    }
                }
            };

            // --- 3. DÜZENLE ---
            itemEditNode.Click += (s, e) => {
                if (selectedNode != null)
                {
                    InputNodeForm form = new InputNodeForm("Kişiyi Düzenle");

                    // Mevcut verileri forma yükle
                    form.NodeName = selectedNode.Name;
                    form.Activity = selectedNode.Activity;
                    form.Interaction = selectedNode.Interaction;

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Güncelle
                        controller.UpdateNode(selectedNode, form.NodeName, form.Activity, form.Interaction);

                        // Sağ paneli de anlık güncelle
                        UpdateNodeInfoPanel(selectedNode);
                        panel1.Invalidate();
                    }
                }
            };

            // --- 4. BAĞLANTIYI (EDGE) SİL ---
            itemDeleteEdge.Click += (s, e) => {
                if (selectedEdge != null)
                {
                    // Controller'da yazdığımız RemoveEdge metodunu çağır
                    controller.RemoveEdge(selectedEdge.Source, selectedEdge.Target);
                    selectedEdge = null;
                    panel1.Invalidate();
                }
            };

            // --- MENÜYE EKLEME SIRASI ---
            // İndex 0: Ekle
            // İndex 1: Node Sil
            // İndex 2: Düzenle
            // İndex 3: Edge Sil
            graphContextMenu.Items.Add(itemAdd);
            graphContextMenu.Items.Add(itemDeleteNode);
            graphContextMenu.Items.Add(itemEditNode);
            graphContextMenu.Items.Add(new ToolStripSeparator()); // Araya çizgi
            graphContextMenu.Items.Add(itemDeleteEdge);
        }
        
        private void BtnApplyLayout_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Önce bir graf yükleyin veya oluşturun.");
                return;
            }

            // İmleci bekleme moduna al (Hesaplama sürerse diye)
            Cursor = Cursors.WaitCursor;

            // 1. Fizik motorunu çalıştır (100 iterasyon boyunca en iyi konumu arar)
            // Panel boyutlarını veriyoruz ki dışarı taşmasınlar.
            // Düğüm sayısına göre alan belirle (Min 1000px, her düğüm için ekstra alan)
            int nodeCount = controller.ActiveGraph.Nodes.Count;
            int virtualWidth = Math.Max(panel1.Width, nodeCount * 20);  // Düğüm başına 50px alan
            int virtualHeight = Math.Max(panel1.Height, nodeCount * 20);

            controller.ApplyForceLayout(virtualWidth, virtualHeight);

            // 2. Yeni koordinatlara göre ekranı tekrar çiz
            panel1.Invalidate();

            // İmleci düzelt
            Cursor = Cursors.Default;
        }
        private void BtnWeighted_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null) return;

            Cursor = Cursors.WaitCursor;

            // 1. ÖNCE: Tüm ağırlıkları matematiksel olarak hesapla
            controller.RecalculateAllWeights();

            // 2. SONRA: Bu ağırlıklara göre fizik motorunu çalıştır
            // (Layout.cs artık yeni Weight değerlerini kullanacak)
            // Alan boyutunu hesapla (önceki adımda yaptığımız 'akıllı alan' mantığı)
            int nodeCount = controller.ActiveGraph.Nodes.Count;
            int virtualWidth = Math.Max(800, nodeCount * 50);
            int virtualHeight = Math.Max(600, nodeCount * 50);

            controller.ApplyForceLayout(virtualWidth, virtualHeight);

            // 3. EN SON: Ekranı ortala ve boya
            // (Buraya 'Akıllı Zoom/Pan' kod bloğunu ekleyebilirsin veya basitçe Invalidate)

            // Basitçe yeniden çizelim (Zoom ayarları korunsun istiyorsan):
            panel1.Invalidate();

            Cursor = Cursors.Default;
        }
        private void btnEditNode_Click(object sender, EventArgs e)
        {
            // 1. Güvenlik Kontrolü: Seçili düğüm var mı?
            if (selectedNode == null)
            {
                MessageBox.Show("Lütfen önce düzenlemek istediğiniz kişiyi seçin.", "Seçim Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Formu Hazırla
            InputNodeForm form = new InputNodeForm("Kişiyi Düzenle");

            // Mevcut verileri forma doldur (Kullanıcı sıfırdan yazmasın)
            form.NodeName = selectedNode.Name;
            form.Activity = selectedNode.Activity;
            form.Interaction = selectedNode.Interaction;

            // 3. Formu Göster ve Sonucu Bekle
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Verileri güncelle
                controller.UpdateNode(selectedNode, form.NodeName, form.Activity, form.Interaction);

                // Değerler değiştiği için kenar ağırlıklarını (Weight) tekrar hesapla!
                controller.RecalculateAllWeights();

                // Sağ paneldeki bilgileri güncelle
                UpdateNodeInfoPanel(selectedNode);

                // Ekranı yenile (Kalınlıklar ve renkler değişsin)
                panel1.Invalidate();
            }
        }
        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            // 1. Seçim Kontrolü
            if (selectedNode == null)
            {
                MessageBox.Show("Lütfen silmek istediğiniz kişiyi seçin.", "Seçim Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Onay İste
            var result = MessageBox.Show(
                $"'{selectedNode.Name}' kişisini ve tüm bağlantılarını silmek istediğinize emin misiniz?",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // 3. Silme İşlemi
            if (result == DialogResult.Yes)
            {
                controller.RemoveNode(selectedNode);

                // Bir kişi silinince komşuların bağlantı sayısı değişir, ağırlıkları yenilemek iyidir.
                controller.RecalculateAllWeights();

                selectedNode = null;
                ClearNodeInfoPanel(); // Sağ paneldeki yazıları temizle
                panel1.Invalidate();  // Grafiği tekrar çiz
            }
        }
    }
}