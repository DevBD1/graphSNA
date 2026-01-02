using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using graphSNA.Model.Foundation;
using System.Text;
using System.Collections.Generic;

namespace graphSNA.UI
{
    /// <summary>
    ///  Main application window for social network analysis and interactive graph visualization. 
    ///  Coordinates graph rendering, UI interaction management (zoom/pan), 
    ///  dynamic layout orchestration, and visual representation of algorithm results.
    /// </summary>
    public partial class MainAppForm : Form
    {
        /// <summary>
        /// Controller responsible for managing graph data and logic.
        /// </summary>
        private ListBox lstNeighbors;
        GraphController controller;
        Node selectedNode = null;
        Node startNodeForPathFinding = null;
        Node endNodeForPathFinding = null;
        bool isSelectingNodesForPathFinding = false;
        bool isSelectingForTraversal = false;
        private ContextMenuStrip graphContextMenu; // Context menu
        private Point lastRightClickPoint; // Coordinates of the clicked location
        private Point _rightMouseDownLocation; // Starting point of right click: to distinguish between pan and menu events

        private Node edgeSourceNode = null; // Node where the connection starts
        private Point currentMousePos;      // Current mouse position during drawing (For ghost line)
        private bool isDraggingEdge = false; // Are we currently drawing a connection?
        private Edge selectedEdge = null;    // The Edge that was right-clicked

        // --- NODE DRAGGING VARIABLES ---
        private Node draggingNode = null;    // The node being dragged
        private bool isDraggingNode = false; // Are we currently dragging a node?
        private Point dragOffset;            // Offset from node's top-left corner to mouse position
        // -------------------------------

        // --- NODE SEARCH CONTROLS ---
        private ComboBox cmbNodeSearch;
        private bool isUpdatingComboBox = false; // Prevent recursive events
        // ----------------------------

        // --- TRAVERSAL ANIMATION VARIABLES ---
        private System.Windows.Forms.Timer animationTimer;
        private List<Node> animationNodes;
        private int animationCurrentIndex;
        private Node animationHighlightedNode;
        private bool isAnimating = false;
        // -------------------------------------

        // Visual settings
        private const int NodeRadius = 8;
        private const int NodeSize = 16;

        // Log Output Control
        private RichTextBox rtbLogs;

        // --- ZOOM & PAN VARIABLES ---
        private float zoomFactor = 1.0f;
        private float panOffsetX = 0.0f;
        private float panOffsetY = 0.0f;
        private Point panStartPoint;           // Starting point of the pan operation
        private bool isPanning = false;        // Are we currently panning?
        // ----------------------------

        public MainAppForm()
        {
            InitializeComponent();

            // Flicker Fix
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, panel1, new object[] { true });

            InitializeNeighborList();

            controller = new GraphController();

            // --- INITIALIZE ANIMATION TIMER ---
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Interval = 1000; // 1000ms between each node
            animationTimer.Tick += AnimationTimer_Tick;

            // --- DYNAMICALLY ADD SEARCH BOX ---
            CreateSearchBox();

            // --- DYNAMICALLY ADD LOG BOX ---
            CreateLogBox();

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

        private void CreateSearchBox()
        {
            GroupBox grpSearch = new GroupBox();
            grpSearch.Text = "Düğüm Ara";
            grpSearch.Size = new Size(238, 60);
            grpSearch.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            grpSearch.ForeColor = Color.DarkSlateGray;
            grpSearch.Padding = new Padding(5);

            cmbNodeSearch = new ComboBox();
            cmbNodeSearch.Location = new Point(10, 25);
            cmbNodeSearch.Size = new Size(218, 23);
            cmbNodeSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cmbNodeSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbNodeSearch.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbNodeSearch.Sorted = true;
            cmbNodeSearch.Font = new Font("Segoe UI", 9, FontStyle.Regular);

            // Event: When user selects or types a node name
            cmbNodeSearch.SelectedIndexChanged += CmbNodeSearch_SelectedIndexChanged;
            cmbNodeSearch.KeyDown += CmbNodeSearch_KeyDown;

            grpSearch.Controls.Add(cmbNodeSearch);

            // Insert at the beginning of flowLayoutPanel1 (before other GroupBoxes)
            this.flowLayoutPanel1.Controls.Add(grpSearch);
            this.flowLayoutPanel1.Controls.SetChildIndex(grpSearch, 0);
        }

        private void CmbNodeSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isUpdatingComboBox || cmbNodeSearch.SelectedItem == null) return;

            string selectedName = cmbNodeSearch.SelectedItem.ToString();
            Node foundNode = controller.ActiveGraph?.Nodes.FirstOrDefault(n => n.Name == selectedName);

            if (foundNode != null)
            {
                SelectAndFocusNode(foundNode);
            }
        }

        private void CmbNodeSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string searchText = cmbNodeSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchText)) return;

                // Find node by name (case-insensitive partial match)
                Node foundNode = controller.ActiveGraph?.Nodes
                    .FirstOrDefault(n => n.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase));

                if (foundNode != null)
                {
                    SelectAndFocusNode(foundNode);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else
                {
                    MessageBox.Show($"'{searchText}' ile eşleşen düğüm bulunamadı.", 
                        "Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void SelectAndFocusNode(Node node)
        {
            // 1. Select the node
            selectedNode = node;
            UpdateNodeInfoPanel(node);

            // 2. Center the view on the node
            float nodeCenterX = node.Location.X + NodeRadius;
            float nodeCenterY = node.Location.Y + NodeRadius;

            float panelCenterX = panel1.Width / 2.0f;
            float panelCenterY = panel1.Height / 2.0f;

            // Calculate pan offset to center the node
            panOffsetX = panelCenterX - (nodeCenterX * zoomFactor);
            panOffsetY = panelCenterY - (nodeCenterY * zoomFactor);

            // 3. Redraw
            panel1.Invalidate();
        }

        /// <summary>
        /// Refreshes the node search ComboBox with current graph nodes.
        /// Call this after loading a graph or adding/removing nodes.
        /// </summary>
        public void RefreshNodeSearchList()
        {
            if (controller.ActiveGraph == null) return;

            isUpdatingComboBox = true;

            cmbNodeSearch.Items.Clear();
            foreach (var node in controller.ActiveGraph.Nodes.OrderBy(n => n.Name))
            {
                cmbNodeSearch.Items.Add(node.Name);
            }

            // Update AutoComplete
            var autoComplete = new AutoCompleteStringCollection();
            autoComplete.AddRange(controller.ActiveGraph.Nodes.Select(n => n.Name).ToArray());
            cmbNodeSearch.AutoCompleteCustomSource = autoComplete;

            isUpdatingComboBox = false;
        }

        public void DisplayResult(string message)
        {
            if (rtbLogs.InvokeRequired)
            {
                rtbLogs.Invoke(new Action<string>(DisplayResult), message);
                return;
            }

            rtbLogs.Clear(); // Always clear previous result for a clean look
            rtbLogs.SelectionFont = new Font("Consolas", 9, FontStyle.Bold);
            rtbLogs.SelectionColor = Color.DarkBlue;
            rtbLogs.AppendText("--- Latest Analysis Result ---\n\n");

            rtbLogs.SelectionFont = new Font("Consolas", 9, FontStyle.Regular);
            rtbLogs.SelectionColor = Color.Black;
            rtbLogs.AppendText(message);
        }

        private void CreateLogBox()
        {
            // Create a new tab for Results
            TabPage tabResults = new TabPage();
            tabResults.Text = "Sonuçlar";
            tabResults.BackColor = SystemColors.ActiveCaption;
            tabResults.Padding = new Padding(5);

            rtbLogs = new RichTextBox();
            rtbLogs.Dock = DockStyle.Fill;
            rtbLogs.ReadOnly = true;
            rtbLogs.BackColor = Color.White;
            rtbLogs.BorderStyle = BorderStyle.None;
            rtbLogs.Font = new Font("Consolas", 10F, FontStyle.Regular);
            rtbLogs.WordWrap = true;
            rtbLogs.ScrollBars = RichTextBoxScrollBars.Vertical;

            tabResults.Controls.Add(rtbLogs);

            // Add the new tab to existing TabControl
            this.tabControl1.Controls.Add(tabResults);
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
        /// <summary>
        /// Renders the graph elements onto the canvas, including nodes, edges, and paths.
        /// </summary>
        /// <param name="e">Paint event arguments containing the graphics object.</param>
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
            // 1. DRAW EDGES
            // ========================================================================

            // Font for writing numbers (Small and legible)
            using (Font weightFont = new Font("Arial", 7, FontStyle.Regular))
            {
                foreach (Edge edge in graphToDraw.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);

                    // A. THICKNESS: Based on weight (Between 1px - 8px)
                    float thickness = 1.0f + (float)(edge.Weight * 4.0f);
                    if (thickness > 8) thickness = 8; // Prevent excessive thickness

                    // B. COLOR: Standard Gray (X-ray mode disabled)
                    Color edgeColor = Color.FromArgb(180, 160, 160, 160);

                    // Draw the line
                    using (Pen dynamicPen = new Pen(edgeColor, thickness))
                    {
                        dynamicPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                        dynamicPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                        g.DrawLine(dynamicPen, p1, p2);
                    }

                    // =========================================================
                    // C. PRINTING NUMBERS (UPDATED SECTION)
                    // =========================================================
                    // Print numbers only if the checkbox is checked!
                    if (chkShowWeights.Checked)
                    {
                        // Find the midpoint of the line
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

                // =========================================================
                // 1.5. HIGHLIGHT PATH
                // =========================================================
                if (controller.HighlightedPath != null && controller.HighlightedPath.Count > 1)
                {
                    using (Pen pathPen = new Pen(Color.LimeGreen, 4)) // Thick Green
                    {
                        pathPen.StartCap = LineCap.Round;
                        pathPen.EndCap = LineCap.Round;

                        for (int i = 0; i < controller.HighlightedPath.Count - 1; i++)
                        {
                            Node n1 = controller.HighlightedPath[i];
                            Node n2 = controller.HighlightedPath[i + 1];

                            Point p1 = new Point(n1.Location.X + NodeRadius, n1.Location.Y + NodeRadius);
                            Point p2 = new Point(n2.Location.X + NodeRadius, n2.Location.Y + NodeRadius);

                            g.DrawLine(pathPen, p1, p2);
                        }
                    }
                }
            }

            // ========================================================================
            // 2. DRAW NODES (REMAINS THE SAME)
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
                else if (node == draggingNode)
                {
                    finalColor = Color.Orange;
                    borderPen = new Pen(Color.DarkOrange, 2);
                }
                // Animation: Currently visiting node
                else if (node == animationHighlightedNode)
                {
                    finalColor = Color.Red;
                    borderPen = new Pen(Color.DarkRed, 3);
                }
                // Animation: Already visited nodes
                else if (isAnimating && animationNodes != null && 
                         animationCurrentIndex > 0 &&
                         animationNodes.Take(animationCurrentIndex - 1).Contains(node))
                {
                    finalColor = Color.LimeGreen;
                    borderPen = new Pen(Color.DarkGreen, 2);
                }

                using (Brush fillBrush = new SolidBrush(finalColor))
                {
                    g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                }

                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X - 5, node.Location.Y - 15);
            }

            // ========================================================================
            // 3. GHOST LINE (REMAINS THE SAME)
            // ========================================================================
            if (isDraggingEdge && edgeSourceNode != null)
            {
                // 1. Convert Mouse Screen Position -> World Coordinates
                float mouseWorldX = (currentMousePos.X - panOffsetX) / zoomFactor;
                float mouseWorldY = (currentMousePos.Y - panOffsetY) / zoomFactor;

                PointF targetPoint = new PointF(mouseWorldX, mouseWorldY);

                // 2. Find the Center of the Source Node
                PointF sourcePoint = new PointF(
                    edgeSourceNode.Location.X + NodeRadius,
                    edgeSourceNode.Location.Y + NodeRadius
                );

                // 3. Draw the Line
                using (Pen ghostPen = new Pen(Color.Red, 2))
                {
                    ghostPen.DashStyle = DashStyle.Dot;
                    g.DrawLine(ghostPen, sourcePoint, targetPoint);
                }
            }
        }
        // Coordinate Transformation Helper (Add if missing, use if exists)
        private Point ApplyTransform(Point worldPoint)
        {
            return new Point(
                (int)(worldPoint.X * zoomFactor + panOffsetX),
                (int)(worldPoint.Y * zoomFactor + panOffsetY)
            );
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            // Coordinate conversion (used by multiple features)
            float worldX = (e.X - panOffsetX) / zoomFactor;
            float worldY = (e.Y - panOffsetY) / zoomFactor;
            Point worldPoint = new Point((int)worldX, (int)worldY);

            // --- NODE DRAGGING ---
            if (isDraggingNode && draggingNode != null)
            {
                // Update node position (subtract offset for accurate placement)
                draggingNode.Location = new Point(
                    (int)worldX - dragOffset.X,
                    (int)worldY - dragOffset.Y
                );
                panel1.Invalidate();
                return;
            }

            // --- EDGE DRAGGING ---
            if (isDraggingEdge)
            {
                currentMousePos = e.Location;
                panel1.Invalidate();
                return;
            }

            // --- PANNING ---
            if (isPanning)
            {
                float deltaX = e.X - panStartPoint.X;
                float deltaY = e.Y - panStartPoint.Y;

                panOffsetX += deltaX;
                panOffsetY += deltaY;

                panStartPoint = e.Location;
                panel1.Invalidate();
                return;
            }

            // --- CURSOR UPDATE (when not dragging anything) ---
            bool isOverNode = controller.FindNodeAtPoint(worldPoint, NodeRadius) != null;
            bool isOverEdge = controller.FindEdgeAtPoint(worldPoint, 15f) != null;

            if (isOverNode)
                panel1.Cursor = Cursors.Hand;
            else if (isOverEdge)
                panel1.Cursor = Cursors.Hand;
            else
                panel1.Cursor = Cursors.Default;
        }
        private void Canvas_MouseClick(object sender, MouseEventArgs e)
        {
            // Do NOT open the menu if any dragging was performed or CTRL is pressed!
            if (isPanning || isDraggingEdge || isDraggingNode || Control.ModifierKeys == Keys.Control) return;

            // --- COORDINATE CALCULATIONS ---
            float worldX = (e.X - panOffsetX) / zoomFactor;
            float worldY = (e.Y - panOffsetY) / zoomFactor;
            Point worldPoint = new Point((int)worldX, (int)worldY);

            Node clickedNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);
            Edge clickedEdge = controller.FindEdgeAtPoint(worldPoint);

            // --- RIGHT CLICK (Context Menu) ---
            if (e.Button == MouseButtons.Right)
            {

                lastRightClickPoint = worldPoint; // If a new node is to be added, add it here
                // Right click is also a selection but it shouldn't perform "Visual Selection", just prepare the menu
                // If you want to clear selection on right click, you can remove: selectedNode = clickedNode;
                // However, actions are usually performed on the right-clicked item:
                selectedNode = clickedNode;

                // 1. NODE MENU
                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    // Configure menu (Only Node operations)
                    graphContextMenu.Items[0].Visible = false; // Add
                    graphContextMenu.Items[1].Visible = true;  // Delete Node
                    graphContextMenu.Items[2].Visible = true;  // Edit
                                                               // Hide Delete Edge button if it exists
                    if (graphContextMenu.Items.Count > 3) graphContextMenu.Items[3].Visible = false;

                    graphContextMenu.Show(panel1, e.Location);
                }
                // 2. EDGE MENU (New!)
                else if (clickedEdge != null)
                {
                    selectedEdge = clickedEdge;
                    // We should add "Delete Edge" option to the menu.
                    // (We will add it dynamically below)

                    ContextMenuStrip edgeMenu = new ContextMenuStrip();
                    edgeMenu.Items.Add("Bağlantıyı Sil").Click += (s, args) => {
                        controller.RemoveEdge(selectedEdge.Source, selectedEdge.Target);
                        panel1.Invalidate();
                    };
                    edgeMenu.Show(panel1, e.Location);
                }
                // 3. EMPTY SPACE (Add Node)
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
            // LEFT CLICK (Selection and Panel Update Only)
            // =================================================================
            if (e.Button == MouseButtons.Left)
            {
                // 1. Is Path Finding Mode active?
                if (isSelectingNodesForPathFinding)
                {
                    HandleShortestPathSelection(clickedNode);
                }
                // 2. Is Traversal Mode active?
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
                // 3. Normal Selection Mode
                else
                {
                    selectedNode = clickedNode; // Select the clicked item (or empty space)

                    if (selectedNode != null)
                    {
                        // NO MORE MESSAGEBOX! Update the panel.
                        UpdateNodeInfoPanel(selectedNode);
                    }
                    else
                    {
                        // Clicked on empty space -> Clear the panel
                        ClearNodeInfoPanel();
                    }

                    panel1.Invalidate(); // Redraw to update selection color (Yellow)
                }
            }
        }
        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            // --- NODE DRAGGING END ---
            if (isDraggingNode)
            {
                isDraggingNode = false;
                draggingNode = null;
                panel1.Cursor = Cursors.Default;
                panel1.Invalidate();
                return;
            }

            // --- EDGE DRAGGING END ---
            if (isDraggingEdge)
            {
                // Is there a node at the drop location?
                float worldX = (e.X - panOffsetX) / zoomFactor;
                float worldY = (e.Y - panOffsetY) / zoomFactor;
                Point worldPoint = new Point((int)worldX, (int)worldY);
                Node targetNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);

                // Is it a valid target?
                if (targetNode != null && targetNode != edgeSourceNode)
                {
                    // Ask Controller: "Were you able to add it?"
                    bool success = controller.AddEdge(edgeSourceNode, targetNode);

                    if (success)
                    {
                        MessageBox.Show($"Bağlantı oluşturuldu: {edgeSourceNode.Name} <-> {targetNode.Name}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Bu bağlantı zaten mevcut!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Cleanup
                isDraggingEdge = false;
                edgeSourceNode = null;
                panel1.Invalidate();
                return;
            }

            // --- PANNING END ---
            if (isPanning)
            {
                isPanning = false;
                panel1.Cursor = Cursors.Default;
            }
        }
        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            // Coordinate Conversion
            float worldX = (e.X - panOffsetX) / zoomFactor;
            float worldY = (e.Y - panOffsetY) / zoomFactor;
            Point worldPoint = new Point((int)worldX, (int)worldY);
            Node clickedNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);

            // --- INITIATE EDGE DRAWING ---
            // Condition: Left Click + SHIFT pressed + A node is clicked
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Shift && clickedNode != null)
            {
                isDraggingEdge = true;
                edgeSourceNode = clickedNode;
                currentMousePos = e.Location;
                return;
            }

            // --- INITIATE NODE DRAGGING ---
            // Condition: Left Click + ALT pressed + A node is clicked
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Alt && clickedNode != null)
            {
                isDraggingNode = true;
                draggingNode = clickedNode;
                // Calculate offset from mouse to node's top-left corner
                dragOffset = new Point(
                    (int)worldX - clickedNode.Location.X,
                    (int)worldY - clickedNode.Location.Y
                );
                panel1.Cursor = Cursors.SizeAll;
                return;
            }

            // --- INITIATE PAN (CTRL + RIGHT/LEFT CLICK) ---
            if ((e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) && Control.ModifierKeys == Keys.Control)
            {
                isPanning = true;
                panStartPoint = e.Location;
                panel1.Cursor = Cursors.SizeAll;
                return;
            }

            // --- PREPARE RIGHT CLICK MENU (RIGHT CLICK ONLY) ---
            if (e.Button == MouseButtons.Right)
            {
                _rightMouseDownLocation = e.Location;
            }
        }
        private void ShowNodeDetails(Node node)
        {
            MessageBox.Show($"Name: {node.Name}\nActivity: {node.Activity}\nInteraction: {node.Interaction}", "Node Details");
        }
        private void ClearNodeDetails() { }
        // Writes selected node information to the right panel
        private void UpdateNodeInfoPanel(Node node)
        {
            // Map TextBox names to those in your project
            textBox1.Text = node.Name;
            textBox2.Text = node.Activity.ToString();
            textBox3.Text = node.Interaction.ToString();

            // 1. Clear previous list
            lstNeighbors.Items.Clear();

            // 2. Get data from controller
            var neighbors = controller.GetNeighborsInfo(node);

            // 3. Add to ListBox
            foreach (var item in neighbors)
            {
                lstNeighbors.Items.Add(item);
            }

            // Update ComboBox selection (without triggering event)
            isUpdatingComboBox = true;
            cmbNodeSearch.SelectedItem = node.Name;
            isUpdatingComboBox = false;
        }

        // Clears the panel (when clicking on empty space)
        private void ClearNodeInfoPanel()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";

            // Clear neighbor list
            if (lstNeighbors != null) lstNeighbors.Items.Clear();

            // Clear ComboBox selection
            isUpdatingComboBox = true;
            cmbNodeSearch.SelectedIndex = -1;
            cmbNodeSearch.Text = "";
            isUpdatingComboBox = false;
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (animationNodes == null || animationCurrentIndex >= animationNodes.Count)
            {
                // Animation complete
                StopTraversalAnimation();
                return;
            }

            // Highlight current node
            animationHighlightedNode = animationNodes[animationCurrentIndex];
            animationCurrentIndex++;

            // Update progress in result panel
            UpdateAnimationProgress();

            panel1.Invalidate();
        }

        private void UpdateAnimationProgress()
        {
            if (animationHighlightedNode == null) return;

            string algo = radioDFS.Checked ? "DFS" : "BFS";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[ANIMASYON DEVAM EDIYOR]");
            sb.AppendLine($"Algoritma: {algo}");
            sb.AppendLine($"Adim: {animationCurrentIndex} / {animationNodes.Count}");
            sb.AppendLine();
            sb.AppendLine("Ziyaret Sirasi:");

            for (int i = 0; i < animationCurrentIndex; i++)
            {
                string marker = (i == animationCurrentIndex - 1) ? ">> " : "   ";
                sb.AppendLine($"{marker}{i + 1}. {animationNodes[i].Name}");
            }

            DisplayResult(sb.ToString());
        }

        private void StopTraversalAnimation()
        {
            animationTimer.Stop();
            isAnimating = false;
            animationHighlightedNode = null;

            // Show final result
            if (animationNodes != null && animationNodes.Count > 0)
            {
                string algo = radioDFS.Checked ? "DFS" : "BFS";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("[ANIMASYON TAMAMLANDI]");
                sb.AppendLine();
                sb.AppendLine($"Algoritma: {algo}");
                sb.AppendLine($"Ziyaret Edilen Dugum Sayisi: {animationNodes.Count}");
                sb.AppendLine();
                sb.AppendLine("Gezinme Sirasi:");
                sb.AppendLine(string.Join(" -> ", animationNodes.Select(n => n.Name)));

                DisplayResult(sb.ToString());
            }

            animationNodes = null;
            panel1.Invalidate();
        }

        private void InitializeNeighborList()
        {
            // Create the ListBox
            lstNeighbors = new ListBox();
            lstNeighbors.Location = new Point(15, 250); // Position below Interaction textbox
            lstNeighbors.Size = new Size(480, 120);     // Width matches other controls
            lstNeighbors.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            lstNeighbors.BackColor = Color.WhiteSmoke;

            // Add a Label for it
            Label lblNeighbors = new Label();
            lblNeighbors.Text = "Komşular & Maliyetler:";
            lblNeighbors.AutoSize = true;
            lblNeighbors.Location = new Point(15, 225); // Just above the listbox
            lblNeighbors.Font = new Font("Segoe UI", 9F, FontStyle.Bold);

            // Add to GroupBox4 (The Node Details panel)
            this.groupBox4.Controls.Add(lblNeighbors);
            this.groupBox4.Controls.Add(lstNeighbors);

            // Increase GroupBox height to fit the new list
            this.groupBox4.Height += 140;
        }
    }
}