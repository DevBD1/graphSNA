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
            FileManager.SaveGraphToCSV(ActiveGraph, filePath);
        }

        // Dosya Yükleme İşi
        public void LoadGraph(string filePath)
        {
            // Yeni grafı yükle ve aktif graf olarak ata
            ActiveGraph = FileManager.LoadGraphFromCSV(filePath);
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
    }
}