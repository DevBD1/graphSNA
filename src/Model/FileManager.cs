using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text; // StringBuilder için gerekli
using System.Windows.Forms;
using System.Drawing;

namespace graphSNA.Model
{
    public static class FileManager
    {
        static Random rnd = new Random();

        // --- KAYDETME İŞLEMİ (SAVE) ---
        public static void SaveGraphToCSV(Graph graph, string filePath)
        {
            try
            {
                // Encoding.UTF8 kullanarak Türkçe karakter sorununu çözeriz
                using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    sw.WriteLine("ID;Ad;Aktiflik;Etkilesim;Komsular");

                    foreach (Node node in graph.Nodes)
                    {
                        List<string> neighborIds = new List<string>();
                        foreach (var edge in graph.Edges)
                        {
                            if (edge.Source == node) neighborIds.Add(edge.Target.Id);
                            else if (edge.Target == node) neighborIds.Add(edge.Source.Id);
                        }
                        string neighborString = string.Join(",", neighborIds);
                        string line = $"{node.Id};{node.Name};{node.Activity};{node.Interaction};{neighborString}";
                        sw.WriteLine(line);
                    }
                }
                MessageBox.Show("Kayıt Başarılı!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        // --- YÜKLEME İŞLEMİ (LOAD) - "ZIRHLI VERSİYON" ---
        public static Graph LoadGraphFromCSV(string filePath)
        {
            Graph newGraph = new Graph();
            if (!File.Exists(filePath)) return newGraph;

            // Hata ayıklama günlüğü (Log)
            StringBuilder debugLog = new StringBuilder();

            try
            {
                var lines = File.ReadAllLines(filePath);
                Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
                Dictionary<string, List<string>> neighborMap = new Dictionary<string, List<string>>();

                // --- FAZ 1: DÜĞÜMLERİ OKU ---
                for (int i = 1; i < lines.Length; i++)
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(';');
                    if (parts.Length < 4) continue;

                    // 1. GİZLİ BOM KARAKTERİNİ TEMİZLE (Kritik Nokta!)
                    string id = parts[0].Trim().Replace("\uFEFF", "").Replace("\"", "");

                    string name = parts[1].Trim().Replace("\"", "");
                    float act = float.Parse(parts[2].Trim());
                    float inter = float.Parse(parts[3].Trim());

                    Node newNode = new Node(id, name, act, inter);

                    // YENİ KOD (ÇEMBER DÜZENİ):
                    // Toplam düğüm sayısına göre açıyı hesapla
                    double angle = (2.0 * Math.PI * (i - 1)) / (lines.Length - 1);

                    // Çemberin Merkezi (Panelin ortası varsayalım: 400, 300)
                    int centerX = 400;
                    int centerY = 300;
                    int radius = 200; // Çemberin genişliği

                    // Trigonometri ile konumu sabitle
                    int x = (int)(centerX + radius * Math.Cos(angle));
                    int y = (int)(centerY + radius * Math.Sin(angle));

                    newNode.Location = new System.Drawing.Point(x, y);

                    // Komşuları Okuma (İndex 4)
                    if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                    {
                        string[] rawNeighbors = parts[4].Split(',');
                        List<string> cleanNeighbors = new List<string>();
                        foreach (var n in rawNeighbors)
                        {
                            if (!string.IsNullOrWhiteSpace(n))
                                cleanNeighbors.Add(n.Trim().Replace("\"", ""));
                        }
                        neighborMap[id] = cleanNeighbors;
                        newNode.ConnectionCount = cleanNeighbors.Count;
                    }

                    newGraph.AddNode(newNode);

                    if (!nodeMap.ContainsKey(id)) nodeMap.Add(id, newNode);
                }

                // --- FAZ 2: KENARLARI KUR ---
                int edgeCount = 0;
                foreach (var kvp in neighborMap)
                {
                    string sourceId = kvp.Key;
                    if (!nodeMap.ContainsKey(sourceId))
                    {
                        debugLog.AppendLine($"Kaynak ID Bulunamadı: {sourceId}");
                        continue;
                    }
                    Node sourceNode = nodeMap[sourceId];

                    foreach (string targetId in kvp.Value)
                    {
                        if (nodeMap.ContainsKey(targetId))
                        {
                            Node targetNode = nodeMap[targetId];

                            // Kenar var mı kontrolü
                            bool exists = newGraph.Edges.Any(e =>
                                (e.Source == sourceNode && e.Target == targetNode) ||
                                (e.Source == targetNode && e.Target == sourceNode));

                            if (!exists)
                            {
                                newGraph.AddEdge(sourceNode, targetNode);
                                edgeCount++;
                            }
                        }
                        else
                        {
                            // Hata durumunda log tut (Sadece ilk 5 hatayı göster)
                            if (debugLog.Length < 500)
                                debugLog.AppendLine($"Hedef ID Bulunamadı: {targetId} (Kaynak: {sourceNode.Name})");
                        }
                    }
                }

                // SONUÇ RAPORU (Eğer kenar sayısı 0 ise sebebini gösterir)
                if (edgeCount == 0 && newGraph.Nodes.Count > 0)
                {
                    MessageBox.Show($"Düğümler yüklendi ama hiç kenar kurulamadı!\n\nOlası Hatalar:\n{debugLog.ToString()}",
                                    "Veri Yükleme Uyarısı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // Başarılıysa bilgi ver (Test aşamasında açık kalsın)
                    // MessageBox.Show($"Başarılı!\nNode: {newGraph.Nodes.Count}\nEdge: {edgeCount}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dosya Okuma Hatası: " + ex.Message);
            }

            return newGraph;
        }
    }
}