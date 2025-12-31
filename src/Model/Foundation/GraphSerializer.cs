using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graphSNA.Model.Foundation
{
    // Sorumluluk: CSV verisini Graph nesnesine (ve tersine) çevirmek.
    public static class GraphSerializer
    {
        public static Graph ParseCsv(string[] lines)
        {
            Graph graph = new Graph();
            Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
            Dictionary<string, List<string>> neighborMap = new Dictionary<string, List<string>>();

            // --- FAZ 1: DÜĞÜMLERİ OKU ---
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(';');
                if (parts.Length < 4) continue;

                // Veri Temizliği (Logic buraya ait)
                string id = parts[0].Trim().Replace("\uFEFF", "").Replace("\"", "");
                string name = parts[1].Trim().Replace("\"", "");

                float act = 0, inter = 0;
                float.TryParse(parts[2].Trim(), out act);
                float.TryParse(parts[3].Trim(), out inter);

                Node newNode = new Node(id, name, act, inter);

                // Komşuları parse et
                if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                {
                    var neighbors = parts[4].Split(',')
                        .Where(n => !string.IsNullOrWhiteSpace(n))
                        .Select(n => n.Trim().Replace("\"", ""))
                        .ToList();

                    neighborMap[id] = neighbors;
                    newNode.ConnectionCount = neighbors.Count;
                }

                graph.AddNode(newNode);
                if (!nodeMap.ContainsKey(id)) nodeMap.Add(id, newNode);
            }

            // --- FAZ 2: KENARLARI KUR ---
            foreach (var kvp in neighborMap)
            {
                if (!nodeMap.ContainsKey(kvp.Key)) continue;
                Node source = nodeMap[kvp.Key];

                foreach (string targetId in kvp.Value)
                {
                    if (nodeMap.ContainsKey(targetId))
                    {
                        Node target = nodeMap[targetId];
                        // Çift yönlü kontrol
                        if (!graph.Edges.Any(e => e.Source == source && e.Target == target || e.Source == target && e.Target == source))
                        {
                            graph.AddEdge(source, target);
                        }
                    }
                }
            }

            return graph;
        }

        public static List<string> SerializeGraph(Graph graph)
        {
            var lines = new List<string>();
            lines.Add("ID;Name;Activity;Interaction;Neighbors");

            foreach (Node node in graph.Nodes)
            {
                var neighborIds = graph.Edges
                    .Where(e => e.Source == node || e.Target == node)
                    .Select(e => e.Source == node ? e.Target.Id : e.Source.Id)
                    .ToList();

                string neighborString = string.Join(",", neighborIds);
                lines.Add($"{node.Id};{node.Name};{node.Activity};{node.Interaction};{neighborString}");
            }
            return lines;
        }
    }
}