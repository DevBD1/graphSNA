using graphSNA.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace graphSNA.UI
{
    // Represents the main application UI form.
    // Core Logic: Variables, Constructor, Paint, MouseClick
    public partial class MainAppForm : Form
    {
        // Global Variables
        GraphController controller;
        Node selectedNode = null;

        // Pathfinding selection variables
        Node startNodeForPathFinding = null;
        Node endNodeForPathFinding = null;
        bool isSelectingNodesForPathFinding = false;

        // Traversal selection variables
        bool isSelectingForTraversal = false;

        // Visual settings
        private const int NodeRadius = 15;
        private const int NodeSize = 30;

        public MainAppForm()
        {
            InitializeComponent();
            controller = new GraphController();

            // Wires up events defined in MainAppForm.Events.cs
            RegisterEvents();

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Updates UI texts based on localization settings
            UpdateUITexts();
        }

        // Handles the painting of the graph nodes and edges
        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graph graphToDraw = controller.ActiveGraph;
            if (graphToDraw == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // 1. Draw Edges
            using (Pen edgePen = new Pen(Color.WhiteSmoke, 2))
            {
                foreach (Edge edge in graphToDraw.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);
                    g.DrawLine(edgePen, p1, p2);
                }
            }

            // 2. Draw Nodes
            Font font = new Font("Arial", 10, FontStyle.Bold);

            foreach (Node node in graphToDraw.Nodes)
            {
                // --- COLOR SELECTION LOGIC (THE FIX) ---
                // Default: Use the node's own color (assigned by Welsh-Powell or default)
                Color finalColor = node.Color;
                Pen borderPen = Pens.Black;

                // Override color for specific interaction states
                if (node == startNodeForPathFinding) // Start Node (Green)
                {
                    finalColor = Color.LightGreen;
                    borderPen = new Pen(Color.DarkGreen, 3);
                }
                else if (node == endNodeForPathFinding) // End Node (Red)
                {
                    finalColor = Color.LightCoral;
                    borderPen = new Pen(Color.DarkRed, 3);
                }
                else if (node == selectedNode) // Selected Node (Yellow)
                {
                    finalColor = Color.Yellow;
                    borderPen = new Pen(Color.Red, 3);
                }

                // --- DRAWING ---
                // Create a brush with the dynamic 'finalColor'
                using (Brush fillBrush = new SolidBrush(finalColor))
                {
                    g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                }

                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X, node.Location.Y - 20);
            }
        }

        // Handles mouse clicks on the graph panel
        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            Node clickedNode = controller.FindNodeAtPoint(e.Location, NodeRadius);

            // Scenario 1: Pathfinding Selection
            if (isSelectingNodesForPathFinding)
            {
                HandleShortestPathSelection(clickedNode);
            }
            // Scenario 2: Traversal Selection (BFS/DFS)
            else if (isSelectingForTraversal)
            {
                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    panel1.Invalidate();

                    // Execute Algorithm (Defined in Events.cs)
                    RunTraversalAlgorithm(clickedNode);

                    isSelectingForTraversal = false;
                }
            }
            // Scenario 3: Normal Selection
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

        // Helper: Display node details
        private void ShowNodeDetails(Node node)
        {
            MessageBox.Show($"Name: {node.Name}\nActivity: {node.Activity}\nInteraction: {node.Interaction}", "Node Details");
        }
        private void ClearNodeDetails() { }
    }
}