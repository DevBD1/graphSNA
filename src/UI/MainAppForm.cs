using graphSNA.Model;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Linq;

namespace graphSNA.UI
{
    public partial class MainAppForm : Form
    {
        Graph myGraph;
        Node selectedNode = null;

        private const int NodeRadius = 15;
        private const int NodeSize = NodeRadius * 2;

        public MainAppForm()
        {
            InitializeComponent();
            InitializeGraphData();

            // Bind events manually to ensure single subscription
            panel1.Paint += GraphCanvas_Paint;
            panel1.MouseClick += PnlGraph_MouseClick;

            // Enable DoubleBuffering to prevent rendering flicker
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        private void InitializeGraphData()
        {
            myGraph = new Graph();

            // Initialize with dummy data for testing
            Node u1 = new Node("Ali", 10, 5, 2) { Location = new Point(100, 100) };
            Node u2 = new Node("Veli", 20, 10, 5) { Location = new Point(300, 200) };

            myGraph.AddNode(u1);
            myGraph.AddNode(u2);
            myGraph.AddEdge(u1, u2);
        }

        private void GraphCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (myGraph == null) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Draw Edges
            using (Pen edgePen = new Pen(Color.Gray, 2))
            {
                foreach (Edge edge in myGraph.Edges)
                {
                    Point p1 = new Point(edge.Source.Location.X + NodeRadius, edge.Source.Location.Y + NodeRadius);
                    Point p2 = new Point(edge.Target.Location.X + NodeRadius, edge.Target.Location.Y + NodeRadius);
                    g.DrawLine(edgePen, p1, p2);
                }
            }

            // Draw Nodes
            Font font = new Font("Arial", 10, FontStyle.Bold);

            foreach (Node node in myGraph.Nodes)
            {
                Brush fillBrush = (node == selectedNode) ? Brushes.Yellow : Brushes.LightBlue;
                Pen borderPen = (node == selectedNode) ? new Pen(Color.Red, 3) : Pens.Black;

                g.FillEllipse(fillBrush, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawEllipse(borderPen, node.Location.X, node.Location.Y, NodeSize, NodeSize);
                g.DrawString(node.Name, font, Brushes.Black, node.Location.X, node.Location.Y - 20);
            }
        }

        private void PnlGraph_MouseClick(object sender, MouseEventArgs e)
        {
            if (myGraph == null) return;

            bool nodeFound = false;

            foreach (Node node in myGraph.Nodes)
            {
                if (IsPointInNode(e.Location, node))
                {
                    selectedNode = node;
                    nodeFound = true;
                    ShowNodeDetails(node);
                    break;
                }
            }

            if (!nodeFound)
            {
                selectedNode = null;
                ClearNodeDetails();
            }

            panel1.Invalidate(); // Trigger redraw to update selection state
        }

        private bool IsPointInNode(Point p, Node n)
        {
            int centerX = n.Location.X + NodeRadius;
            int centerY = n.Location.Y + NodeRadius;

            // Euclidean distance check
            double distance = Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2));
            return distance <= NodeRadius;
        }

        private void ShowNodeDetails(Node node)
        {
            string info = $"Name: {node.Name}\nActivity: {node.Activity}\nInteraction: {node.Interaction}";
            MessageBox.Show(info, "Node Info");
        }

        private void ClearNodeDetails()
        {
            // Placeholder for UI cleanup logic
        }

        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        private void findShortestPathButton_Click(object sender, EventArgs e)
        {
            if (myGraph == null || myGraph.Nodes.Count < 2)
            {
                MessageBox.Show("Graph is not ready.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var start = myGraph.Nodes.First();
            var goal = myGraph.Nodes.Last();

            IShortestPathAlgorithm algorithm;

            // Note: 'radioDijsktra' contains a typo in Designer, kept for consistency
            if (radioDijsktra.Checked)
                algorithm = new DijkstraAlgorithm();
            else if (radioAstar.Checked)
                algorithm = new AStarAlgorithm();
            else
            {
                MessageBox.Show("Please select an algorithm.", "Warning");
                return;
            }

            var manager = new ShortestPathManager(algorithm);
            var (path, cost) = manager.Calculate(myGraph, start, goal);

            if (path.Count == 0)
            {
                txtCost.Text = "No Path";
                MessageBox.Show("No path found.", "Result");
            }
            else
            {
                string pathStr = string.Join(" → ", path.Select(n => n.Name));
                txtCost.Text = $"{cost:F2}";
                MessageBox.Show($"Path: {pathStr}\nCost: {cost:F2}", "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}