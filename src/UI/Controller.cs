using graphSNA.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace graphSNA.UI
{
    /// <summary>
    ///  The bridge between Form and Data (Graph).
    /// </summary>
    public class GraphController
    {
        // We store the data here, Form cannot access directly (Encapsulation)
        public Graph ActiveGraph { get; private set; }

        public GraphController()
        {
            ActiveGraph = new Graph();
        }

        // Yeni Düğüm Ekleme İşi
        public void AddNode(string name, float act, float inter, long conn, Point loc)
        {
            string randomId = Guid.NewGuid().ToString().Substring(0, 8);

            // conn (ConnectionCount) parametresi artık Node içine gitmiyor.
            // İleride otomatik hesaplanacak.
            Node newNode = new Node(randomId, name, act, inter);
            newNode.ConnectionCount = conn; // Manuel eklemede elle atayabiliriz
            newNode.Location = loc;

            ActiveGraph.AddNode(newNode);
        }

        // Dosya Kaydetme İşi
        public void SaveGraph(string filePath)
        {
            // FileManager'ın public ve static olduğundan emin olun
            FileManager.SaveGraph(ActiveGraph, filePath);
        }

        // Dosya Yükleme İşi
        public void LoadGraph(string filePath)
        {
            // Yeni grafı yükle ve aktif graf olarak ata
            ActiveGraph = FileManager.LoadGraph(filePath);
        }

        // Tıklanan noktada düğüm var mı kontrolü (Mantık işi)
        public Node FindNodeAtPoint(Point p, int radius)
        {
            foreach (Node node in ActiveGraph.Nodes)
            {
                int centerX = node.Location.X + radius;
                int centerY = node.Location.Y + radius;

                // Pisagor Hesabı
                double distance = Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2));

                if (distance <= radius)
                    return node;
            }
            return null;
        }

        public void ApplyForceLayout(int width, int height)
        {
            Layout layout = new Layout();
            // 100 iterasyon boyunca hesapla ve en son halini kaydet
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
                algorithm = new DijkstraAlgorithm(); // Varsayılan

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
            return ColoringAlgorithm.ApplyWelshPowell(ActiveGraph);
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
    }
}