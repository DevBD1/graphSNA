using graphSNA.Model.Algorithms;
using graphSNA.Model.Foundation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphSNA.UI
{
    /// <summary>
    /// The bridge between Form and Data (Graph).
    /// </summary>
    public class GraphController
    {
        // We store the data here, Form cannot access it directly (Encapsulation)
        public Graph ActiveGraph { get; private set; }
        
        // Stores the nodes of the calculated shortest path for visualization
        public List<Node> HighlightedPath { get; set; } = new List<Node>();

        public GraphController()
        {
            ActiveGraph = new Graph();
        }

        // CRUD Operations for Nodes
        public Node AddNode(string name, float act, float inter, long conn, Point loc)
        {
            if (ActiveGraph == null) ActiveGraph = new Graph();

            // Check for duplicate name (case-insensitive)
            if (ActiveGraph.Nodes.Any(n => n.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                return null; // Duplicate name found, return null to indicate failure
            }

            // 1. New ID Generation Logic (Auto-Increment)
            int newId = 1; // Start from 1 if there are no nodes

            if (ActiveGraph.Nodes.Count > 0)
            {
                // Try to parse existing IDs to integers, default to 0 if fails.
                // This allows finding the maximum numeric ID (e.g., 101, 102).
                int maxId = ActiveGraph.Nodes
                    .Select(n => int.TryParse(n.Id, out int val) ? val : 0)
                    .Max();

                newId = maxId + 1;
            }

            // 2. Create Node with the new ID
            // Node constructor parameter order: (Id, Name, Activity, Interaction)
            Node newNode = new Node(newId.ToString(), name, act, inter);

            newNode.ConnectionCount = conn;
            newNode.Location = loc;

            ActiveGraph.AddNode(newNode);
            return newNode; // Return the added node (may be needed elsewhere)
        }
        public void RemoveNode(Node node)
        {
            if (node == null) return;

            // Get all neighbors to update their connection counts before the edges are removed
            var neighbors = ActiveGraph.Edges
                .Where(e => e.Source == node || e.Target == node)
                .Select(e => e.Source == node ? e.Target : e.Source)
                .ToList();

            foreach (var neighbor in neighbors)
            {
                neighbor.ConnectionCount--;
            }

            ActiveGraph.RemoveNode(node);
        }
        public void UpdateNode(Node node, string newName, float act, float inter)
        {
            // Check for duplicate name (exclude current node, case-insensitive)
            bool duplicateExists = ActiveGraph.Nodes.Any(n => 
                n != node && n.Name.Equals(newName, StringComparison.OrdinalIgnoreCase));

            if (duplicateExists)
            {
                throw new InvalidOperationException($"A node with name '{newName}' already exists.");
            }

            node.Name = newName;
            node.Activity = act;
            node.Interaction = inter;

            // Recalculate weights for all connected edges since properties changed
            foreach (var edge in ActiveGraph.Edges)
            {
                if (edge.Source == node || edge.Target == node)
                {
                    edge.CalculateWeight();
                }
            }
        }

        // Returns all disconnected sub-graphs (islands) in the network
        public List<List<Node>> GetConnectedComponents()
        {
            return ConnectedComponents.FindComponents(ActiveGraph);
        }

        public void RemoveEdge(Node n1, Node n2)
        {
            bool exists = ActiveGraph.Edges.Any(e =>
                (e.Source == n1 && e.Target == n2) ||
                (e.Source == n2 && e.Target == n1));

            if (exists)
            {
                ActiveGraph.RemoveEdge(n1, n2);
                n1.ConnectionCount--;
                n2.ConnectionCount--;

                // Recalculate weights for all edges connected to n1 and n2
                foreach (var edge in ActiveGraph.Edges)
                {
                    if (edge.Source == n1 || edge.Target == n1 || edge.Source == n2 || edge.Target == n2)
                    {
                        edge.CalculateWeight();
                    }
                }
            }
        }
        public bool AddEdge(Node source, Node target)
        {
            // 1. Prevent self-loops
            if (source == target) return false;

            // 2. Check if edge already exists (Undirected graph logic)
            bool exists = ActiveGraph.Edges.Any(e =>
                (e.Source == source && e.Target == target) ||
                (e.Source == target && e.Target == source));

            // 3. Add if it doesn't exist and return true
            if (!exists)
            {
                // Update connection counts first so the new edge gets correct weight
                source.ConnectionCount++;
                target.ConnectionCount++;

                ActiveGraph.AddEdge(source, target);

                // Recalculate weights for all edges connected to source and target
                foreach (var edge in ActiveGraph.Edges)
                {
                    if (edge.Source == source || edge.Target == source || edge.Source == target || edge.Target == target)
                    {
                        edge.CalculateWeight();
                    }
                }

                return true; // SUCCESS
            }

            return false; // ALREADY EXISTS, FAILURE
        }
        // File Saving Operations
        public void SaveGraph(string filePath)
        {
            // Ensure FileManager is public and static
            FileManager.SaveGraph(ActiveGraph, filePath);
        }

        // File Loading Operations
        public void LoadGraph(string filePath)
        {
            // Load new graph and assign as ActiveGraph
            ActiveGraph = FileManager.LoadGraph(filePath);
            //RecalculateAllWeights();
        }
        public void ApplyForceLayout(int width, int height)
        {
            Layout layout = new Layout();
            // Calculate and save the state after 100 iterations
            layout.CalculateLayout(ActiveGraph, width, height, 100);
        }

        // Shortest Path
        public (List<Node> path, double cost) CalculateShortestPath(Node start, Node end, string algorithmType)
        {
            if (ActiveGraph == null || ActiveGraph.Nodes.Count < 2)
                return (new List<Node>(), 0);

            IShortestPathAlgorithm algorithm;

            if (algorithmType == "A*")
                algorithm = new AStarAlgorithm();
            else
                algorithm = new DijkstraAlgorithm(); // Default

            ShortestPathManager manager = new ShortestPathManager(algorithm);
            return manager.Calculate(ActiveGraph, start, end);
        }
        // Graph Traversal Logic (Polymorphism applied here)
        public List<Node> TraverseGraph(Node startNode, string algorithmType)
        {
            if (ActiveGraph == null || startNode == null)
                return new List<Node>();

            IGraphTraversal traversalAlgorithm;

            // Select algorithm dynamically
            if (algorithmType == "DFS")
                traversalAlgorithm = new DFS();
            else
                traversalAlgorithm = new BFS(); // Default

            return traversalAlgorithm.Traverse(ActiveGraph, startNode);
        }
        public int ColorGraph()
        {
            if (ActiveGraph == null) return 0;
            return Coloring.ApplyWelshPowell(ActiveGraph);
        }
        // --- DEGREE CENTRALITY ANALYSIS ---
        // Returns the top count nodes with the highest connection frequency
        public List<Node> GetTopInfluencers(int count)
        {
            if (ActiveGraph == null || ActiveGraph.Nodes.Count == 0)
                return new List<Node>();

            // Sort by ConnectionCount (primary) and Interaction (secondary) descending
            return ActiveGraph.Nodes
                .OrderByDescending(n => n.ConnectionCount)
                .ThenByDescending(n => n.Interaction)
                .Take(count)
                .ToList();
        }
        /// <summary>
        /// Checks if a node exists at the given coordinates (Point p).
        /// </summary>
        /// <param name="p">Clicked World Coordinate</param>
        /// <param name="radius">Node Radius (Typically NodeRadius = 20)</param>
        /// <returns>Found Node or null</returns>
        public Node FindNodeAtPoint(Point p, int radius)
        {
            // Iterate backwards to select the topmost (last drawn) node in case of overlap.
            for (int i = ActiveGraph.Nodes.Count - 1; i >= 0; i--)
            {
                Node node = ActiveGraph.Nodes[i];

                // Visual center of the node
                // Note: Location is usually the top-left corner, so we add the radius.
                int centerX = node.Location.X + radius;
                int centerY = node.Location.Y + radius;

                // Distance calculation using Pythagorean theorem: ((x1-x2)^2 + (y1-y2)^2) ^ 0.5
                double distance = Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2));

                // If distance is less than or equal to the radius, we are inside the circle.
                if (distance <= radius)
                {
                    return node;
                }
            }
            return null;
        }
        // Check if there is an Edge near the clicked point (Tolerance: 15 pixels)
        // Necessary for deletion via right-click!
        public Edge FindEdgeAtPoint(Point clickPoint, float tolerance = 15f) // 1. Increased tolerance from 5 to 15
        {
            Edge bestMatch = null;
            double minDistance = double.MaxValue; // Track the shortest distance

            foreach (var edge in ActiveGraph.Edges)
            {
                float x1 = edge.Source.Location.X;
                float y1 = edge.Source.Location.Y;
                float x2 = edge.Target.Location.X;
                float y2 = edge.Target.Location.Y;

                // Calculate the distance
                float distance = GetDistanceToLineSegment(clickPoint.X, clickPoint.Y, x1, y1, x2, y2);

                // 2. If within tolerance AND the closest edge found so far
                if (distance <= tolerance && distance < minDistance)
                {
                    minDistance = distance;
                    bestMatch = edge;
                }
            }

            return bestMatch; // Return the closest edge (or null if none found)
        }
        // Mathematical Helper: Distance from a Point to a Line Segment
        private float GetDistanceToLineSegment(float px, float py, float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;
            if (dx == 0 && dy == 0) return (float)Math.Sqrt(Math.Pow(px - x1, 2) + Math.Pow(py - y1, 2));

            float t = ((px - x1) * dx + (py - y1) * dy) / (dx * dx + dy * dy);

            if (t < 0) { dx = px - x1; dy = py - y1; }
            else if (t > 1) { dx = px - x2; dy = py - y2; }
            else { dx = px - (x1 + t * dx); dy = py - (y1 + t * dy); }

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
        public void RecalculateAllWeights()
        {
            if (ActiveGraph == null) return;

            foreach (var edge in ActiveGraph.Edges)
            {
                edge.CalculateWeight();
            }
        }

        public string GetAdjacencyMatrixAsString()
        {
            if (ActiveGraph == null || ActiveGraph.Nodes.Count == 0)
                return "No graph data.";

            var (matrix, nodeOrder) = ActiveGraph.GetAdjacencyMatrix();
            int n = nodeOrder.Count;
            StringBuilder sb = new StringBuilder();

            // Header
            sb.Append("".PadRight(10));
            foreach (var node in nodeOrder)
                sb.Append(node.Name.PadRight(8));
            sb.AppendLine();

            // Rows
            for (int i = 0; i < n; i++)
            {
                sb.Append(nodeOrder[i].Name.PadRight(10));
                for (int j = 0; j < n; j++)
                {
                    string val = matrix[i, j] > 0 ? matrix[i, j].ToString("F2") : "-";
                    sb.Append(val.PadRight(8));
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public string GetAdjacencyListAsString()
        {
            if (ActiveGraph == null || ActiveGraph.Nodes.Count == 0)
                return "Graf verisi bulunamadı.";

            StringBuilder sb = new StringBuilder();

            foreach (var node in ActiveGraph.Nodes)
            {
                // 1. Find all edges connected to this node
                var connections = ActiveGraph.Edges
                    .Where(e => e.Source == node || e.Target == node)
                    .Select(e => new
                    {
                        Neighbor = (e.Source == node) ? e.Target : e.Source,
                        Cost = e.Weight
                    })
                    .ToList();

                // 2. Format the output: "Node (A) has neighbors..."
                sb.AppendLine($"[{node.Name}] (Toplam: {connections.Count})");

                if (connections.Count > 0)
                {
                    foreach (var conn in connections)
                    {
                        sb.AppendLine($"   -> Komşu: {conn.Neighbor.Name.PadRight(10)} | Maliyet: {conn.Cost:F2}");
                    }
                }
                else
                {
                    sb.AppendLine("   -> (Komşu yok)");
                }
                sb.AppendLine(""); // Empty line for readability
            }

            return sb.ToString();
        }
        // used by Node Info Panel
        public List<string> GetNeighborsInfo(Node node)
        {
            if (ActiveGraph == null || node == null) return new List<string>();

            var infoList = new List<string>();

            // Bu düğüme bağlı tüm kenarları bul
            var connectedEdges = ActiveGraph.Edges
                .Where(e => e.Source == node || e.Target == node)
                .ToList();

            foreach (var edge in connectedEdges)
            {
                // Karşıdaki komşu düğümü bul
                Node neighbor = (edge.Source == node) ? edge.Target : edge.Source;

                // Format: "İsim (Maliyet: 0.85)"
                infoList.Add($"{neighbor.Name} (Maliyet: {edge.Weight:F2})");
            }

            if (infoList.Count == 0) infoList.Add("(Komşu yok)");

            return infoList;
        }
    }
}