using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graphSNA.Model.Foundation
{
    /// <summary>
    /// Handles serialization and deserialization of Graph objects to/from CSV format.
    /// Responsible for parsing node data and reconstructing edge relationships.
    /// </summary>
    public static class GraphSerializer
    {
        // CSV Format: ID;Name;Activity;Interaction;Neighbors
        // Example: 1;Ali;85.5;72.3;2,3,5

        public static Graph ParseCsv(string[] lines)
        {
            Graph graph = new Graph();
            
            // Maps node ID to Node object for quick lookup when creating edges
            Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
            
            // Stores neighbor IDs for each node (will be used to create edges later)
            Dictionary<string, List<string>> neighborMap = new Dictionary<string, List<string>>();

            // PHASE 1: Read all nodes first (skip header at index 0)
            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                // Split by semicolon: [0]=ID, [1]=Name, [2]=Activity, [3]=Interaction, [4]=Neighbors
                string[] parts = line.Split(';');
                if (parts.Length < 4) continue;

                // Clean data: remove BOM character (\uFEFF) and quotes
                string id = parts[0].Trim().Replace("\uFEFF", "").Replace("\"", "");
                string name = parts[1].Trim().Replace("\"", "");

                // Parse float values (TryParse returns 0 if parsing fails)
                float act = 0, inter = 0;
                float.TryParse(parts[2].Trim(), out act);
                float.TryParse(parts[3].Trim(), out inter);

                // Create node with parsed values
                Node newNode = new Node(id, name, act, inter);

                // Parse neighbors if column exists (comma-separated IDs)
                if (parts.Length > 4 && !string.IsNullOrWhiteSpace(parts[4]))
                {
                    // Split "2,3,5" into ["2", "3", "5"]
                    var neighbors = parts[4].Split(',')
                        .Where(n => !string.IsNullOrWhiteSpace(n))  // Skip empty entries
                        .Select(n => n.Trim().Replace("\"", ""))    // Clean each ID
                        .Where(n => n != id) // Kendine referansı yoksay
                        .ToList();

                    neighborMap[id] = neighbors;
                    newNode.ConnectionCount = neighbors.Count;  // Set degree (connection count)
                }

                graph.AddNode(newNode);
                if (!nodeMap.ContainsKey(id)) nodeMap.Add(id, newNode);
            }

            // PHASE 2: Create edges using neighbor information
            // We do this in a second pass because all nodes must exist first
            foreach (var kvp in neighborMap)
            {
                if (!nodeMap.ContainsKey(kvp.Key)) continue;
                Node source = nodeMap[kvp.Key];

                foreach (string targetId in kvp.Value)
                {
                    if (nodeMap.ContainsKey(targetId))
                    {
                        Node target = nodeMap[targetId];
                        
                        // Check if edge already exists (undirected: A-B == B-A)
                        bool edgeExists = graph.Edges.Any(e => 
                            (e.Source == source && e.Target == target) || 
                            (e.Source == target && e.Target == source));
                        
                        if (!edgeExists)
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
            
            // Add CSV header row
            lines.Add("ID;Name;Activity;Interaction;Neighbors");

            foreach (Node node in graph.Nodes)
            {
                // Find all neighbors by checking edges where this node is source or target
                var neighborIds = graph.Edges
                    .Where(e => e.Source == node || e.Target == node)
                    .Select(e => e.Source == node ? e.Target.Id : e.Source.Id)
                    .ToList();

                // Join neighbor IDs with comma: "2,3,5"
                string neighborString = string.Join(",", neighborIds);
                
                // Format: ID;Name;Activity;Interaction;Neighbors
                lines.Add($"{node.Id};{node.Name};{node.Activity};{node.Interaction};{neighborString}");
            }
            
            return lines;
        }
    }
}