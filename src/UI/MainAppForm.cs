using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Reflection;
using graphSNA.Model;

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

        // Visual settings
        private const int NodeRadius = 8;
        private const int NodeSize = 16;

        // --- ZOOM & PAN VARIABLES ---
        private float zoomFactor = 1.0f;
        private float panOffsetX = 0.0f;
        private float panOffsetY = 0.0f;
        private bool isPanning = false;
        private Point lastMousePosition;
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
            this.panel1.MouseWheel += GraphCanvas_MouseWheel;
            this.panel1.MouseDown += GraphCanvas_MouseDown;
            this.panel1.MouseMove += GraphCanvas_MouseMove;
            this.panel1.MouseUp += GraphCanvas_MouseUp;
            // Focus panel on enter so wheel works immediately
            this.panel1.MouseEnter += (s, e) => panel1.Focus();

            // Calls localization method (stub at bottom if missing)
            UpdateUITexts();
        }

        // --- ZOOM LOGIC ---
        private void GraphCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) zoomFactor *= 1.1f;
            else zoomFactor /= 1.1f;

            // Clamp zoom
            if (zoomFactor < 0.1f) zoomFactor = 0.1f;
            if (zoomFactor > 5.0f) zoomFactor = 5.0f;

            panel1.Invalidate();
        }

        // --- PANNING LOGIC (DRAG TO MOVE) ---
        private void GraphCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isPanning = true;
                lastMousePosition = e.Location;
                Cursor = Cursors.SizeAll; // Visual feedback
            }
        }

        private void GraphCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isPanning)
            {
                // Calculate how much the mouse moved
                float deltaX = e.X - lastMousePosition.X;
                float deltaY = e.Y - lastMousePosition.Y;

                // Update the pan offset
                panOffsetX += deltaX;
                panOffsetY += deltaY;

                lastMousePosition = e.Location;
                panel1.Invalidate(); // Redraw with new position
            }
        }

        private void GraphCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                isPanning = false;
                Cursor = Cursors.Default;
            }
        }

        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graph graphToDraw = controller.ActiveGraph;
            if (graphToDraw == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Move
            g.TranslateTransform(panOffsetX, panOffsetY);
            // 2. Zoom
            g.ScaleTransform(zoomFactor, zoomFactor);

            // Draw Edges
            using (Pen edgePen = new Pen(Color.FromArgb(200, 220, 220, 220), 1))
            {
                foreach (Edge edge in graphToDraw.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);
                    g.DrawLine(edgePen, p1, p2);
                }
            }

            // Draw Nodes
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
        }

        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            // Don't trigger click logic if we just finished panning
            if (isPanning) return;

            // --- COORDINATE CONVERSION (SCREEN -> WORLD) ---
            // Formula: World = (Screen - PanOffset) / ZoomFactor
            float worldX = (e.X - panOffsetX) / zoomFactor;
            float worldY = (e.Y - panOffsetY) / zoomFactor;
            Point worldPoint = new Point((int)worldX, (int)worldY);

            Node clickedNode = controller.FindNodeAtPoint(worldPoint, NodeRadius);

            if (isSelectingNodesForPathFinding)
            {
                HandleShortestPathSelection(clickedNode);
            }
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

        private void ShowNodeDetails(Node node)
        {
            MessageBox.Show($"Name: {node.Name}\nActivity: {node.Activity}\nInteraction: {node.Interaction}", "Node Details");
        }

        private void ClearNodeDetails() { }
    }
}