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

            // Tab Page Change Event (For updating Statistics)
            this.tabControl1.SelectedIndexChanged += TabSelectedIndexChanged;

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
                    //Distributes nodes across the screen randomly and uniformly
                    controller.ApplyForceLayout(panel1.Width, panel1.Height);

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
                string pathStr = string.Join(" -> ", result.path.Select(n => n.Name));
                txtCost.Text = $"{result.cost:F2}";
                MessageBox.Show($"Path: {pathStr}\nCost: {result.cost:F2}", "Result");
            }
            else
            {
                txtCost.Text = "None";
                MessageBox.Show("No path found.", "Result");
            }
            panel1.Invalidate();
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

            // Menü Şıkları
            var itemAdd = new ToolStripMenuItem("Yeni Kişi Ekle");
            var itemDelete = new ToolStripMenuItem("Sil");
            var itemEdit = new ToolStripMenuItem("Düzenle (Özellikler)");

            // Tıklama Olayları
            itemAdd.Click += (s, e) => {
                // Basitçe rastgele isimle ekleyelim (İleride InputBox yaparız)
                string name = "User_" + new Random().Next(100, 999);
                controller.AddNode(name, 0.5f, 50, 0, lastRightClickPoint);
                panel1.Invalidate();
            };

            itemDelete.Click += (s, e) => {
                if (selectedNode != null)
                {
                    controller.RemoveNode(selectedNode);
                    selectedNode = null; // Seçimi kaldır
                    panel1.Invalidate();
                }
            };

            itemEdit.Click += (s, e) => {
                if (selectedNode != null)
                {
                    // Detayları gösterip düzenlemeye izin verebiliriz
                    // Şimdilik sadece bilgi gösterelim, istersen buraya güncelleme formunu bağlarız
                    MessageBox.Show($"Düzenlenecek: {selectedNode.Name}\nŞu anlık özellik güncelleme için InputForm yapmamız gerek.", "Bilgi");
                }
            };

            // Menüye Ekleme
            graphContextMenu.Items.Add(itemAdd);
            graphContextMenu.Items.Add(itemDelete);
            graphContextMenu.Items.Add(itemEdit);
        }
    }
}