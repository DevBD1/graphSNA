using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using graphSNA.Model.Foundation;

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

            controller = new GraphController();

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
            if (!isPanning && !isDraggingEdge)
            {
                // Coordinate conversion
                float worldX = (e.X - panOffsetX) / zoomFactor;
                float worldY = (e.Y - panOffsetY) / zoomFactor;
                Point worldPoint = new Point((int)worldX, (int)worldY);

                // Is there anything underneath?
                // Using wide tolerance (15f) for Edge here as well
                bool isOverNode = controller.FindNodeAtPoint(worldPoint, NodeRadius) != null;
                bool isOverEdge = controller.FindEdgeAtPoint(worldPoint, 15f) != null;

                if (isOverNode || isOverEdge)
                    panel1.Cursor = Cursors.Hand; // Hand cursor for clickable items 👆
                else
                    panel1.Cursor = Cursors.Default;
            }
            if (isDraggingEdge)
            {
                currentMousePos = e.Location; // Mouse screen coordinates
                panel1.Invalidate(); // Redraw the line
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
            // Do NOT open the menu if panning was just performed or CTRL is pressed!
            if (isPanning || isDraggingEdge || Control.ModifierKeys == Keys.Control) return;

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
                    edgeMenu.Items.Add("Delete Connection").Click += (s, args) => {
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
                        MessageBox.Show($"Connection established: {edgeSourceNode.Name} <-> {targetNode.Name}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("This connection already exists!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Cleanup
                isDraggingEdge = false;
                edgeSourceNode = null;
                panel1.Invalidate();
                return;
            }
            if (isPanning)
            {
                isPanning = false;
                panel1.Cursor = Cursors.Default; // Reset cursor to default
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
                currentMousePos = e.Location; // Starting point
                return;
            }
            // 1. INITIATE PAN (CTRL + RIGHT/LEFT CLICK)
            if ((e.Button == MouseButtons.Right || e.Button == MouseButtons.Left) && Control.ModifierKeys == Keys.Control)
            {
                isPanning = true;
                panStartPoint = e.Location; // First point of mouse press
                panel1.Cursor = Cursors.SizeAll; // Visual feedback
            }

            // 2. PREPARE RIGHT CLICK MENU (RIGHT CLICK ONLY)
            else if (e.Button == MouseButtons.Right)
            {
                // If CTRL is not pressed, this is a menu request.
                // Save the location (to be used in MouseClick)
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

            // Enable Delete and Edit buttons
            // btnDeleteNode.Enabled = true; (If buttons exist)
            // btnEditNode.Enabled = true;
        }

        // Clears the panel (when clicking on empty space)
        private void ClearNodeInfoPanel()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
        }
    }
}