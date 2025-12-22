using graphSNA.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace graphSNA.UI
{
    /// <summary>
    ///  Represents the main application UI form.
    /// </summary>
    public partial class MainAppForm : Form
    {
        // Global Variables
        GraphController controller;
        Node selectedNode = null;

        // Pathfinding selection variables
        Node startNodeForPathFinding = null;
        Node endNodeForPathFinding = null;
        bool isSelectingNodesForPathFinding = false;

        // Constants for node rendering
        private const int NodeRadius = 15;
        private const int NodeSize = 30;

        public MainAppForm()
        {
            InitializeComponent();

            // Initialize the Controller
            controller = new GraphController();

            // Register events
            RegisterEvents();

            // Visual settings for smoother rendering
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();

            // Localization
            UpdateUITexts();
        }

        /// <summary>
        ///  Visualization & Rendering
        /// </summary>
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
                Brush fillBrush = Brushes.LightBlue;
                Pen borderPen = Pens.Black;

                // --- COLORING LOGIC ---

                // Case 1: Start Node for pathfinding (Green)
                if (node == startNodeForPathFinding)
                {
                    fillBrush = Brushes.LightGreen;
                    borderPen = new Pen(Color.DarkGreen, 3);
                }
                // Case 2: End Node for pathfinding (Red)
                else if (node == endNodeForPathFinding)
                {
                    fillBrush = Brushes.LightCoral;
                    borderPen = new Pen(Color.DarkRed, 3);
                }
                // Case 3: Standard selection (Yellow)
                else if (node == selectedNode)
                {
                    fillBrush = Brushes.Yellow;
                    borderPen = new Pen(Color.Red, 3);
                }

                g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X, node.Location.Y - 20);
            }
        }

        /// <summary>
        /// Mouse Click Event Handling
        /// </summary>
        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            // Is there a node at the clicked point?
            Node clickedNode = controller.FindNodeAtPoint(e.Location, NodeRadius);

            // --- SCENARIO A: Pathfinding Selection Mode ---
            if (isSelectingNodesForPathFinding)
            {
                if (startNodeForPathFinding == null)
                {
                    // If start node is not set, set the clicked node as start
                    if (clickedNode != null)
                    {
                        startNodeForPathFinding = clickedNode;
                        selectedNode = clickedNode; // Keep visually selected
                        MessageBox.Show($"Start Node Selected: {clickedNode.Name}\nPlease click on the TARGET node.", "Step 1 Complete");
                    }
                }
                else if (endNodeForPathFinding == null)
                {
                    // Start is set, now selecting target
                    if (clickedNode != null && clickedNode != startNodeForPathFinding)
                    {
                        endNodeForPathFinding = clickedNode;

                        // Both nodes selected! Exit mode and calculate.
                        isSelectingNodesForPathFinding = false;

                        RunShortestPathAlgorithm();
                    }
                    else if (clickedNode == startNodeForPathFinding)
                    {
                        MessageBox.Show("Start and End nodes cannot be the same!", "Error");
                    }
                }

                // Redraw to update colors (Green/Red)
                panel1.Invalidate();
            }
            // --- SCENARIO B: Normal Mode (Show details) ---
            else
            {
                if (clickedNode != null)
                {
                    selectedNode = clickedNode;
                    ShowNodeDetails(selectedNode);
                }
                else
                {
                    selectedNode = null; // Deselect if clicked on empty space
                    ClearNodeDetails();
                }
                panel1.Invalidate();
            }
        }

        private void ShowNodeDetails(Node node)
        {
            // Temporary Popup, will be replaced by UI panels later
            string info = $"Name: {node.Name}\nActivity: {node.Activity}\nInteraction: {node.Interaction}";
            MessageBox.Show(info, "Node Details");
        }

        private void ClearNodeDetails()
        {
            // Placeholder for right panel cleanup logic
        }

        private void btnFindShortestPath_Click(object sender, EventArgs e)
        {
            // Check if graph is valid
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 2)
            {
                MessageBox.Show("Please import a graph with at least 2 nodes first.", "Warning");
                return;
            }

            // Enable selection mode
            isSelectingNodesForPathFinding = true;
            startNodeForPathFinding = null;
            endNodeForPathFinding = null;

            // Inform the user
            MessageBox.Show("Please click on the START node on the graph.", "Selection Mode Started");
        }

        /// <summary>
        /// Helper method to execute the algorithm after selection is complete
        /// </summary>
        private void RunShortestPathAlgorithm()
        {
            string algoType = radioAstar.Checked ? "A*" : "Dijkstra";

            // Delegate calculation to Controller
            var result = controller.CalculateShortestPath(startNodeForPathFinding, endNodeForPathFinding, algoType);

            // Display Result
            if (result.path.Count > 0)
            {
                string pathStr = string.Join(" -> ", result.path.Select(n => n.Name));
                txtCost.Text = $"{result.cost:F2}";
                MessageBox.Show($"Path: {pathStr}\nCost: {result.cost:F2}", "Shortest Path Found");
            }
            else
            {
                txtCost.Text = "None";
                MessageBox.Show("No path found between these two nodes.", "Result");
            }

            // Clear colors (Optional: keep them to show the path endpoints)
            startNodeForPathFinding = null;
            endNodeForPathFinding = null;
            panel1.Invalidate();
        }
    }
}