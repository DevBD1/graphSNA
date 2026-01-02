using System;
using System.Windows.Forms;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using graphSNA.Model.Foundation;
using graphSNA.Model.Algorithms;

namespace graphSNA.UI
{
    /// <summary>
    ///  Manages UI events, algorithm triggers (Shortest Path, Traversal, etc.), 
    ///  file operations, and visualization updates for the main application form.
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
            this.btnConnectedComponents.Click += RunConnectedComponents;

            // Updating Statistics
            this.tabControl1.SelectedIndexChanged += TabSelectedIndexChanged;
            this.button9.Click += BtnRefreshStats_Click;

            // Layout (Re-arrangement) Button
            this.button3.Click += BtnApplyLayout_Click;
            this.button4.Click += BtnWeighted_Click;

            // Redraw (Invalidate) screen immediately when CheckBox changes
            this.chkShowWeights.CheckedChanged += (s, e) => panel1.Invalidate();
            this.button5.Click += btnEditNode_Click;
            this.button6.Click += btnDeleteNode_Click;
        }

        // --- HELPER: Format elapsed time for display ---
        private static string FormatElapsedTime(Stopwatch sw)
        {
            double ms = sw.Elapsed.TotalMilliseconds;
            if (ms < 1)
                return $"{sw.Elapsed.TotalMicroseconds:F0} µs";
            if (ms < 1000)
                return $"{ms:F2} ms";
            return $"{sw.Elapsed.TotalSeconds:F3} s";
        }

        // --- 1. FILE OPERATIONS ---
        private void ImportCSV(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "CSV Dosyaları|*.csv" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    controller.LoadGraph(ofd.FileName);
                    groupBox1.Text = $"Dosya: {Path.GetFileName(ofd.FileName)}";
                    controller.RecalculateAllWeights();

                    // Refresh the node search list
                    RefreshNodeSearchList();

                    // Distributes nodes across the screen randomly and uniformly
                    controller.ApplyForceLayout(panel1.Width, panel1.Height);

                    // Immediately after layout finishes:
                    // 1. Calculate Virtual Width
                    int nodeCount = controller.ActiveGraph.Nodes.Count;
                    // Min 800px to prevent small graphs from being crushed.
                    int virtualWidth = Math.Max(800, nodeCount * 20);
                    int virtualHeight = Math.Max(600, nodeCount * 20);

                    // 2. Run Physics Engine
                    controller.ApplyForceLayout(virtualWidth, virtualHeight);

                    // --- 3. SMART CENTERING AND FITTING (FIX) ---

                    // Virtual center of the graph (World Coordinates)
                    float graphCenterX = virtualWidth / 2.0f;
                    float graphCenterY = virtualHeight / 2.0f;

                    // Center of the panel (Screen Coordinates)
                    float panelCenterX = panel1.Width / 2.0f;
                    float panelCenterY = panel1.Height / 2.0f;

                    // Find the Zoom ratio required to fit on screen
                    float scaleX = (float)panel1.Width / virtualWidth;
                    float scaleY = (float)panel1.Height / virtualHeight;

                    // Zoom based on the most constrained edge (Leave 10% margin: 0.9f)
                    zoomFactor = Math.Min(scaleX, scaleY) * 0.9f;

                    // Prevent massive zoom in very small files (Max Zoom 1.0)
                    if (zoomFactor > 1.0f) zoomFactor = 1.0f;
                    // Prevent too much shrinking (Min Zoom 0.1)
                    if (zoomFactor < 0.1f) zoomFactor = 0.1f;

                    // Calculate Pan (Offset) Value:
                    // Formula: TargetScreen = (World * Zoom) + Pan
                    // Pan = TargetScreen - (World * Zoom)
                    panOffsetX = panelCenterX - (graphCenterX * zoomFactor);
                    panOffsetY = panelCenterY - (graphCenterY * zoomFactor);

                    panel1.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportCSV(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV Dosyaları|*.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                controller.SaveGraph(sfd.FileName);
                DisplayResult($"Dışa Aktarıldı: {Path.GetFileName(sfd.FileName)}");
            }
        }

        // --- 2. SHORTEST PATH LOGIC ---
        private void RunFindShortestPath(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 2)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.", "Uyarı");
                return;
            }

            isSelectingNodesForPathFinding = true;
            isSelectingForTraversal = false;
            startNodeForPathFinding = null;
            endNodeForPathFinding = null;

            MessageBox.Show("Lütfen graftan BAŞLANGIÇ düğümünü seçin.", "Adım 1");
        }

        private void HandleShortestPathSelection(Node clickedNode)
        {
            if (startNodeForPathFinding == null)
            {
                if (clickedNode != null)
                {
                    startNodeForPathFinding = clickedNode;
                    selectedNode = clickedNode;
                    MessageBox.Show($"Başlangıç: {clickedNode.Name}\nŞimdi HEDEF düğümünü seçin.", "Adım 2");
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
                    MessageBox.Show("Başlangıç ve hedef düğüm aynı olamaz!", "Hata");
                }
            }
            panel1.Invalidate();
        }

        private void RunShortestPathAlgorithm()
        {
            string algoType = radioAstar.Checked ? "A*" : "Dijkstra";
            
            // Start performance measurement
            var stopwatch = Stopwatch.StartNew();
            var result = controller.CalculateShortestPath(startNodeForPathFinding, endNodeForPathFinding, algoType);
            stopwatch.Stop();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Algoritma: {algoType}");
            sb.AppendLine($"Başlangıç: {startNodeForPathFinding.Name}");
            sb.AppendLine($"Hedef: {endNodeForPathFinding.Name}");

            if (result.path.Count > 0)
            {
                controller.HighlightedPath = result.path;
                txtCost.Text = $"{result.cost:F2}";

                string pathStr = string.Join(" → ", result.path.Select(n => n.Name));
                sb.AppendLine($"Toplam Maliyet: {result.cost:F2}");
                sb.AppendLine($"Yol: {pathStr}");
            }
            else
            {
                controller.HighlightedPath = null;
                txtCost.Text = "Yok";
                sb.AppendLine("Sonuç: Yol bulunamadı.");
                MessageBox.Show("Yol bulunamadı.", "Sonuç");
            }

            // Append performance metrics
            sb.AppendLine();
            sb.AppendLine("───────────────────────────");
            sb.AppendLine("[PERFORMANS METRİKLERİ]");
            sb.AppendLine($"İşlem Süresi: {FormatElapsedTime(stopwatch)}");
            sb.AppendLine($"Düğüm Sayısı: {controller.ActiveGraph.Nodes.Count}");
            sb.AppendLine($"Kenar Sayısı: {controller.ActiveGraph.Edges.Count}");

            DisplayResult(sb.ToString());
            panel1.Invalidate();
        }

        // --- 3. TRAVERSAL (BFS/DFS) LOGIC ---
        private void RunTraverse(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 1)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.");
                return;
            }

            isSelectingForTraversal = true;
            isSelectingNodesForPathFinding = false;

            MessageBox.Show("Gezinmeye başlamak için bir BAŞLANGIÇ düğümüne tıklayın.", "Gezinme Modu");
        }

        private void RunTraversalAlgorithm(Node startNode)
        {
            string algo = radioDFS.Checked ? "DFS" : "BFS";
            
            // Get traversal order
            List<Node> resultOrder = controller.TraverseGraph(startNode, algo);

            if (resultOrder.Count > 0)
            {
                // Start animation
                animationNodes = resultOrder;
                animationCurrentIndex = 0;
                animationHighlightedNode = null;
                isAnimating = true;

                // Show starting message
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"[{algo} ANIMASYONU BASLATILIYOR]");
                sb.AppendLine();
                sb.AppendLine($"Baslangic Dugumu: {startNode.Name}");
                sb.AppendLine($"Toplam Dugum: {resultOrder.Count}");
                sb.AppendLine();
                sb.AppendLine("Her 0.4 saniyede bir dugum ziyaret edilecek.");
                DisplayResult(sb.ToString());

                // Start timer
                animationTimer.Start();
            }
        }

        // --- 4. WELSH-POWELL COLORING ---
        private void RunColoring(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.", "Uyarı");
                return;
            }

            // Start performance measurement
            var stopwatch = Stopwatch.StartNew();
            int colorCount = controller.ColorGraph();
            stopwatch.Stop();
            
            panel1.Invalidate();

            // --- GENERATE LEGEND ---
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Algoritma: Welsh-Powell Renklendirme");
            sb.AppendLine("Durum: Başarılı");
            sb.AppendLine($"Kromatik Sayı (Toplam Renk): {colorCount}");
            sb.AppendLine("");
            sb.AppendLine("[LEJANT - Renk Dağılımı]");

            // Group nodes by color to count usage
            var colorGroups = controller.ActiveGraph.Nodes
                .GroupBy(n => n.Color)
                .OrderByDescending(g => g.Count())
                .ToList();

            foreach (var group in colorGroups)
            {
                // Try to get a readable name, otherwise use RGB
                string colorName = group.Key.IsKnownColor ? group.Key.Name : $"#{group.Key.R:X2}{group.Key.G:X2}{group.Key.B:X2}";
                
                // Optional: List top 3 nodes in this color group for context
                string exampleNodes = string.Join(", ", group.Take(3).Select(n => n.Name));
                if (group.Count() > 3) exampleNodes += ", ...";

                sb.AppendLine($"  • {colorName.PadRight(12)}: {group.Count()} Düğüm ({exampleNodes})");
            }

            // Append performance metrics
            sb.AppendLine();
            sb.AppendLine("───────────────────────────");
            sb.AppendLine("[PERFORMANS METRİKLERİ]");
            sb.AppendLine($"İşlem Süresi: {FormatElapsedTime(stopwatch)}");
            sb.AppendLine($"Düğüm Sayısı: {controller.ActiveGraph.Nodes.Count}");
            sb.AppendLine($"Kenar Sayısı: {controller.ActiveGraph.Edges.Count}");

            DisplayResult(sb.ToString());
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
            dataGridView1.Columns.Add("Score", "Merkezilik Puanı");

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

            // --- CREATE MENU ITEMS ---
            var itemAdd = new ToolStripMenuItem("Yeni Kişi Ekle");
            var itemDeleteNode = new ToolStripMenuItem("Seçili Kişiyi Sil");
            var itemEditNode = new ToolStripMenuItem("Düzenle (Özellikler)");
            var itemDeleteEdge = new ToolStripMenuItem("Bağlantıyı Sil");

            // --- 1. ADD NEW PERSON ---
            itemAdd.Click += (s, e) => {
                InputNodeForm form = new InputNodeForm("Yeni Kişi Ekle");

                if (form.ShowDialog() == DialogResult.OK)
                {
                    var newNode = controller.AddNode(form.NodeName, form.Activity, form.Interaction, 0, lastRightClickPoint);
                    
                    if (newNode == null)
                    {
                        MessageBox.Show($"'{form.NodeName}' isimli bir kişi zaten mevcut!", 
                            "Yinelenen İsim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    
                    RefreshNodeSearchList(); // Update search list
                    panel1.Invalidate();
                }
            };

            // --- 2. DELETE PERSON ---
            itemDeleteNode.Click += (s, e) => {
                if (selectedNode != null)
                {
                    var result = MessageBox.Show(
                        $"'{selectedNode.Name}' kişisini ve tüm bağlantılarını silmek istediğinizden emin misiniz?",
                        "Silme Onayı",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        controller.RemoveNode(selectedNode);
                        selectedNode = null;
                        ClearNodeInfoPanel();
                        RefreshNodeSearchList(); // Update search list
                        panel1.Invalidate();
                    }
                }
            };

            // --- 3. EDIT ---
            itemEditNode.Click += (s, e) => {
                if (selectedNode != null)
                {
                    InputNodeForm form = new InputNodeForm("Kişiyi Düzenle");

                    // Load existing data into form
                    form.NodeName = selectedNode.Name;
                    form.Activity = selectedNode.Activity;
                    form.Interaction = selectedNode.Interaction;

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // Update (throws exception if duplicate)
                            controller.UpdateNode(selectedNode, form.NodeName, form.Activity, form.Interaction);

                            // Update the right panel immediately
                            UpdateNodeInfoPanel(selectedNode);
                            panel1.Invalidate();
                        }
                        catch (InvalidOperationException)
                        {
                            MessageBox.Show($"'{form.NodeName}' isimli bir kişi zaten mevcut!", 
                                "Yinelenen İsim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            };

            // --- 4. DELETE CONNECTION (EDGE) ---
            itemDeleteEdge.Click += (s, e) => {
                if (selectedEdge != null)
                {
                    // Call the RemoveEdge method in the controller
                    controller.RemoveEdge(selectedEdge.Source, selectedEdge.Target);
                    selectedEdge = null;
                    panel1.Invalidate();
                }
            };

            // --- MENU ITEM ORDER ---
            // Index 0: Add
            // Index 1: Delete Node
            // Index 2: Edit
            // Index 3: Delete Edge
            graphContextMenu.Items.Add(itemAdd);
            graphContextMenu.Items.Add(itemDeleteNode);
            graphContextMenu.Items.Add(itemEditNode);
            graphContextMenu.Items.Add(new ToolStripSeparator()); // Separator line
            graphContextMenu.Items.Add(itemDeleteEdge);

            var itemShowMatrix = new ToolStripMenuItem("Komşuluk Matrisini Göster");
            itemShowMatrix.Click += (s, e) => {
                string matrix = controller.GetAdjacencyMatrixAsString();
                DisplayResult("[KOMŞULUK MATRİSİ]\n\n" + matrix);
            };

            graphContextMenu.Items.Add(new ToolStripSeparator());
            graphContextMenu.Items.Add(itemShowMatrix);

            // 2. NEW List Item (Adjacency List)
            var itemShowList = new ToolStripMenuItem("Komşuluk Listesini Göster");
            itemShowList.Click += (s, e) => {
                string list = controller.GetAdjacencyListAsString();
                DisplayResult("[KOMŞULUK LİSTESİ - DETAYLI]\n\n" + list);
            };
            graphContextMenu.Items.Add(itemShowList);
        }

        private void BtnApplyLayout_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin veya oluşturun.");
                return;
            }

            // Set cursor to wait mode
            Cursor = Cursors.WaitCursor;

            // 1. Run physics engine (searches for best position for 100 iterations)
            // Providing panel dimensions to prevent nodes from going out of bounds.
            int nodeCount = controller.ActiveGraph.Nodes.Count;
            int virtualWidth = Math.Max(panel1.Width, nodeCount * 20);  // 20px area per node
            int virtualHeight = Math.Max(panel1.Height, nodeCount * 20);

            controller.ApplyForceLayout(virtualWidth, virtualHeight);

            // 2. Redraw screen based on new coordinates
            panel1.Invalidate();

            // Reset cursor
            Cursor = Cursors.Default;
        }

        private void BtnWeighted_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null) return;

            Cursor = Cursors.WaitCursor;

            // 1. FIRST: Recalculate all weights mathematically
            controller.RecalculateAllWeights();

            // 2. SECOND: Run physics engine based on these weights
            int nodeCount = controller.ActiveGraph.Nodes.Count;
            int virtualWidth = Math.Max(800, nodeCount * 50);
            int virtualHeight = Math.Max(600, nodeCount * 50);

            controller.ApplyForceLayout(virtualWidth, virtualHeight);

            // 3. FINALLY: Redraw the screen
            panel1.Invalidate();

            Cursor = Cursors.Default;
        }

        private void btnEditNode_Click(object sender, EventArgs e)
        {
            // 1. Safety Check: Is a node selected?
            if (selectedNode == null)
            {
                MessageBox.Show("Lütfen önce düzenlemek istediğiniz kişiyi seçin.", "Seçim Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Prepare Form
            InputNodeForm form = new InputNodeForm("Kişiyi Düzenle");

            // Fill form with current data
            form.NodeName = selectedNode.Name;
            form.Activity = selectedNode.Activity;
            form.Interaction = selectedNode.Interaction;

            // 3. Show Form and Wait for Result
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Update data (throws exception if duplicate)
                    controller.UpdateNode(selectedNode, form.NodeName, form.Activity, form.Interaction);

                    // Recalculate edge weights as values have changed!
                    controller.RecalculateAllWeights();

                    // Update info in the right panel
                    UpdateNodeInfoPanel(selectedNode);

                    // Refresh screen
                    panel1.Invalidate();
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show($"'{form.NodeName}' isimli bir kişi zaten mevcut!", 
                        "Yinelenen İsim", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            // 1. Selection Check
            if (selectedNode == null)
            {
                MessageBox.Show("Lütfen önce silmek istediğiniz kişiyi seçin.", "Seçim Yok", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Confirm Deletion
            var result = MessageBox.Show(
                $"'{selectedNode.Name}' kişisini ve tüm bağlantılarını silmek istediğinizden emin misiniz?",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // 3. Execution
            if (result == DialogResult.Yes)
            {
                controller.RemoveNode(selectedNode);

                // Refresh weights as neighbor counts change when someone is deleted
                controller.RecalculateAllWeights();

                selectedNode = null;
                ClearNodeInfoPanel(); // Clear right panel texts
                panel1.Invalidate();  // Redraw graph
            }
        }

        private void RunConnectedComponents(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.", "Uyarı");
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var components = ConnectedComponents.FindComponents(controller.ActiveGraph);
            stopwatch.Stop();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[BAĞLANTI ANALİZİ]");
            sb.AppendLine($"Toplam Bileşen Sayısı: {components.Count}");
            sb.AppendLine();

            if (components.Count == 1)
            {
                sb.AppendLine("Graf BAĞLI: TÜM düğümler birbirine ulaşabilir.");
            }
            else
            {
                sb.AppendLine("Graf PARÇALI: Ayrik topluluklar mevcut!");
            }
            sb.AppendLine();

            int index = 1;
            foreach (var component in components.OrderByDescending(c => c.Count))
            {
                string status = component.Count == 1 ? " [IZOLE]" : "";
                string nodes = string.Join(", ", component.Select(n => n.Name));
                sb.AppendLine($"Bilesen {index} ({component.Count} dugum){status}:");
                sb.AppendLine($"  {nodes}");
                sb.AppendLine();
                index++;
            }

            // Performance metrics
            sb.AppendLine("----------------------------");
            sb.AppendLine("[PERFORMANS METRIKLERI]");
            sb.AppendLine($"Islem Suresi: {FormatElapsedTime(stopwatch)}");
            sb.AppendLine($"Dugum Sayisi: {controller.ActiveGraph.Nodes.Count}");
            sb.AppendLine($"Kenar Sayisi: {controller.ActiveGraph.Edges.Count}");

            DisplayResult(sb.ToString());
        }
    }
}