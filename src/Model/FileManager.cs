using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace graphSNA.Model
{
    public static class FileManager
    {
        // KAYDETME İŞLEMİ (SAVE)
        public static void SaveGraphToCSV(Graph graph, string filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    // Başlık Satırı (Header)
                    sw.WriteLine("Ad;Aktiflik;Etkilesim;BaglantiSayisi;X;Y;Komsular");

                    foreach (Node node in graph.Nodes)
                    {
                        // 1. Düğümün kendi özelliklerini al
                        string line = $"{node.Name};{node.Activity};{node.Interaction};{node.ConnectionCount};{node.Location.X};{node.Location.Y}";

                        // 2. Komşularını bul (Bu düğüme bağlı olan diğer düğümler)
                        // Edge listesinden bu node'un kaynak (Source) olduğu bağlantıları buluyoruz.
                        // (Yönsüz graf olduğu için hem Source hem Target kontrol edilebilir ama kayıtta tek yön yeterli olabilir,
                        // ancak tam garanti için tüm bağlantıları tarıyoruz).
                        List<string> neighbors = new List<string>();

                        foreach (var edge in graph.Edges)
                        {
                            if (edge.Source == node) neighbors.Add(edge.Target.Name);
                            else if (edge.Target == node) neighbors.Add(edge.Source.Name);
                        }

                        // Komşuları virgülle ayırarak ekle (Örn: "Ali;...;Veli,Ayşe")
                        if (neighbors.Count > 0)
                        {
                            line += ";" + string.Join(",", neighbors);
                        }

                        sw.WriteLine(line);
                    }
                }
                MessageBox.Show("Kayıt Başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // YÜKLEME İŞLEMİ (LOAD)
        public static Graph LoadGraphFromCSV(string filePath)
        {
            Graph newGraph = new Graph();
            var lines = File.ReadAllLines(filePath);

            // Sözlük: İsimden Node nesnesine hızlı erişim için
            Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();

            // Geçici hafıza: Hangi isim kiminle komşu? (Bağlantıları sonra kuracağız)
            Dictionary<string, string[]> neighborMap = new Dictionary<string, string[]>();

            // ADIM 1: Önce tüm DÜĞÜMLERİ (Nodes) oluştur
            // (1. satır başlık olduğu için i=1'den başlıyoruz)
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(';');
                if (parts.Length < 6) continue; // Hatalı satırsa atla

                string name = parts[0];
                double act = double.Parse(parts[1]);
                double inter = double.Parse(parts[2]);
                double conn = double.Parse(parts[3]);
                int x = int.Parse(parts[4]);
                int y = int.Parse(parts[5]);

                Node newNode = new Node(name, act, inter, conn);
                newNode.Location = new System.Drawing.Point(x, y);

                newGraph.AddNode(newNode);
                nodeMap[name] = newNode;

                // Komşuları varsa hafızaya at (Daha sonra bağlayacağız)
                if (parts.Length > 6 && !string.IsNullOrWhiteSpace(parts[6]))
                {
                    string[] neighbors = parts[6].Split(',');
                    neighborMap[name] = neighbors;
                }
            }

            // ADIM 2: Şimdi BAĞLANTILARI (Edges) kur
            foreach (var kvp in neighborMap)
            {
                Node sourceNode = nodeMap[kvp.Key];

                foreach (string neighborName in kvp.Value)
                {
                    if (nodeMap.ContainsKey(neighborName))
                    {
                        Node targetNode = nodeMap[neighborName];

                        // Çift taraflı eklemeyi önlemek için kontrol:
                        // Sadece "Ali -> Veli" ekleyelim, "Veli -> Ali" zaten aynı kenardır.
                        // Bunu sağlamak için basit bir string karşılaştırması yapabiliriz veya Graph.cs kontrol etmeli.
                        // Şimdilik Graph.cs içinde kontrol olmadığını varsayarak doğrudan ekliyoruz,
                        // ancak Edge constructor'ı aynı kenarın tekrar eklenmesini engellemez.
                        // Basit çözüm: Zaten bağlılar mı kontrol et.

                        bool alreadyConnected = newGraph.Edges.Any(e =>
                            (e.Source == sourceNode && e.Target == targetNode) ||
                            (e.Source == targetNode && e.Target == sourceNode));

                        if (!alreadyConnected)
                        {
                            newGraph.AddEdge(sourceNode, targetNode);
                        }
                    }
                }
            }

            return newGraph;
        }
    }
}