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
    ///  The bridge between Form and Data (Graph).
    /// </summary>
    public class GraphController
    {
        // We store the data here, Form cannot access it directly (Encapsulation)
        public Graph ActiveGraph { get; private set; }
        public GraphController()
        {
            ActiveGraph = new Graph();
        }

        // CRUD Operations for Nodes
        public Node AddNode(string name, float act, float inter, long conn, Point loc)
        {
            string randomId = Guid.NewGuid().ToString().Substring(0, 8);
            Node newNode = new Node(randomId, name, act, inter);
            newNode.ConnectionCount = conn;
            newNode.Location = loc;

            ActiveGraph.AddNode(newNode);
            return newNode; // Eklenen düğümü geri döndür (lazım olabilir)
        }
        public void RemoveNode(Node node)
        {
            ActiveGraph.RemoveNode(node);
        }
        public void UpdateNode(Node node, string newName, float act, float inter)
        {
            node.Name = newName;
            node.Activity = act;
            node.Interaction = inter;
        }

        public void RemoveEdge(Node n1, Node n2)
        {
            ActiveGraph.RemoveEdge(n1, n2);
            // Sayacı düşürmek istersen:
            // n1.ConnectionCount--; n2.ConnectionCount--;
        }
        public bool AddEdge(Node source, Node target)
        {
            // 1. Kendine bağlanmayı önle
            if (source == target) return false;

            // 2. Zaten var mı kontrolü (Yönsüz graf mantığı)
            bool exists = ActiveGraph.Edges.Any(e =>
                (e.Source == source && e.Target == target) ||
                (e.Source == target && e.Target == source));

            // 3. Eğer yoksa ekle ve 'true' döndür
            if (!exists)
            {
                ActiveGraph.AddEdge(source, target);

                // İsteğe bağlı sayaç güncelleme
                // source.ConnectionCount++;
                // target.ConnectionCount++;

                return true; // BAŞARILI
            }

            return false; // ZATEN VAR, BAŞARISIZ
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
        /// Verilen koordinatta (Point p) bir düğüm olup olmadığını kontrol eder.
        /// </summary>
        /// <param name="p">Tıklanan Dünya (World) Koordinatı</param>
        /// <param name="radius">Düğümün Yarıçapı (Genelde NodeRadius = 20)</param>
        /// <returns>Bulunan Düğüm veya null</returns>
        public Node FindNodeAtPoint(Point p, int radius)
        {
            // Listeyi tersten dönüyoruz ki üst üste binen düğümlerde 
            // en üsttekini (son çizileni) seçebilelim.
            for (int i = ActiveGraph.Nodes.Count - 1; i >= 0; i--)
            {
                Node node = ActiveGraph.Nodes[i];

                // Düğümün görsel merkezi
                // Not: Location genelde sol-üst köşedir, bu yüzden yarıçap ekliyoruz.
                int centerX = node.Location.X + radius;
                int centerY = node.Location.Y + radius;

                // Pisagor ile mesafe hesabı: ((x1-x2)^2 + (y1-y2)^2) ^ 0.5
                double distance = Math.Sqrt(Math.Pow(p.X - centerX, 2) + Math.Pow(p.Y - centerY, 2));

                // Eğer mesafe yarıçaptan küçük veya eşitse, bu dairenin içindeyiz demektir.
                if (distance <= radius)
                {
                    return node;
                }
            }
            return null;
        }
        // Tıklanan noktaya yakın bir Edge var mı? (Tolerans: 5 piksel)
        // Sağ tıkla silmek için gerekli!
        public Edge FindEdgeAtPoint(Point clickPoint, float tolerance = 5f)
        {
            foreach (var edge in ActiveGraph.Edges)
            {
                float x1 = edge.Source.Location.X;
                float y1 = edge.Source.Location.Y;
                float x2 = edge.Target.Location.X;
                float y2 = edge.Target.Location.Y;

                if (GetDistanceToLineSegment(clickPoint.X, clickPoint.Y, x1, y1, x2, y2) <= tolerance)
                {
                    return edge;
                }
            }
            return null;
        }
        // Matematiksel Yardımcı: Noktanın Çizgiye Uzaklığı
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
    }
}