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
                DisplayResult($"Exported: {Path.GetFileName(sfd.FileName)}");
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

                string pathStr = string.Join("->", result.path.Select(n => n.Name)); // Compact arrows
                DisplayResult($"Algorithm: {algoType}\nFrom: {startNodeForPathFinding.Name}\nTo: {endNodeForPathFinding.Name}\nTotal Cost: {result.cost:F2}\nPath: {pathStr}");
            }
            else
            {
                controller.HighlightedPath = null;
                txtCost.Text = "None";
                DisplayResult($"Algorithm: {algoType}\nResult: No path found.");
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
                sb.AppendLine($"Visited Nodes: {resultOrder.Count}");
                sb.AppendLine("Traversal Order:");

                string flow = string.Join("->", resultOrder.Select(n => n.Name));
                sb.AppendLine(flow);

                DisplayResult(sb.ToString());
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

            // --- GENERATE LEGEND ---
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Algorithm: Welsh-Powell Coloring");
            sb.AppendLine("Status: Success");
            sb.AppendLine($"Chromatic Number (Total Colors): {colorCount}");
            sb.AppendLine("");
            sb.AppendLine("[LEGEND - Color Distribution]");

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

                sb.AppendLine($"- {colorName.PadRight(15)}: {group.Count()} Nodes ({exampleNodes})");
            }

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
            MessageBox.Show("List has been updated.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatsTable()
        {
            if (controller.ActiveGraph == null) return;

            var topNodes = controller.GetTopInfluencers(5);

            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Columns
            dataGridView1.Columns.Add("Rank", "Rank");
            dataGridView1.Columns.Add("Name", "Name");
            dataGridView1.Columns.Add("Degree", "Degree");
            dataGridView1.Columns.Add("Score", "Centrality Score");

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
            var itemAdd = new ToolStripMenuItem("Add New Person");
            var itemDeleteNode = new ToolStripMenuItem("Delete Selected Person");
            var itemEditNode = new ToolStripMenuItem("Edit (Properties)");
            var itemDeleteEdge = new ToolStripMenuItem("Delete Connection");

            // --- 1. ADD NEW PERSON ---
            itemAdd.Click += (s, e) => {
                InputNodeForm form = new InputNodeForm("Add New Person");

                if (form.ShowDialog() == DialogResult.OK)
                {
                    // Add via controller (lastRightClickPoint: location where right-clicked)
                    controller.AddNode(form.NodeName, form.Activity, form.Interaction, 0, lastRightClickPoint);
                    panel1.Invalidate();
                }
            };

            // --- 2. DELETE PERSON ---
            itemDeleteNode.Click += (s, e) => {
                if (selectedNode != null)
                {
                    var result = MessageBox.Show(
                        $"Are you sure you want to delete '{selectedNode.Name}' and all associated connections?",
                        "Confirm Deletion",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        controller.RemoveNode(selectedNode);
                        selectedNode = null;
                        ClearNodeInfoPanel(); // Clear the right panel
                        panel1.Invalidate();
                    }
                }
            };

            // --- 3. EDIT ---
            itemEditNode.Click += (s, e) => {
                if (selectedNode != null)
                {
                    InputNodeForm form = new InputNodeForm("Edit Person");

                    // Load existing data into form
                    form.NodeName = selectedNode.Name;
                    form.Activity = selectedNode.Activity;
                    form.Interaction = selectedNode.Interaction;

                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        // Update
                        controller.UpdateNode(selectedNode, form.NodeName, form.Activity, form.Interaction);

                        // Update the right panel immediately
                        UpdateNodeInfoPanel(selectedNode);
                        panel1.Invalidate();
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
        }

        private void BtnApplyLayout_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count == 0)
            {
                MessageBox.Show("Please load or create a graph first.");
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
                MessageBox.Show("Please select the person you want to edit first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Prepare Form
            InputNodeForm form = new InputNodeForm("Edit Person");

            // Fill form with current data
            form.NodeName = selectedNode.Name;
            form.Activity = selectedNode.Activity;
            form.Interaction = selectedNode.Interaction;

            // 3. Show Form and Wait for Result
            if (form.ShowDialog() == DialogResult.OK)
            {
                // Update data
                controller.UpdateNode(selectedNode, form.NodeName, form.Activity, form.Interaction);

                // Recalculate edge weights as values have changed!
                controller.RecalculateAllWeights();

                // Update info in the right panel
                UpdateNodeInfoPanel(selectedNode);

                // Refresh screen
                panel1.Invalidate();
            }
        }

        private void btnDeleteNode_Click(object sender, EventArgs e)
        {
            // 1. Selection Check
            if (selectedNode == null)
            {
                MessageBox.Show("Please select the person you want to delete first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Confirm Deletion
            var result = MessageBox.Show(
                $"Are you sure you want to delete '{selectedNode.Name}' and all associated connections?",
                "Confirm Deletion",
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
    }
}