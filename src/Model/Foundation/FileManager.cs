using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace graphSNA.Model.Foundation
{
    /// <summary>
    /// Handles file I/O operations for graph data.
    /// Coordinates between serialization and file system operations.
    /// </summary>
    public static class FileManager
    {
        public static void SaveGraph(Graph graph, string filePath)
        {
            // Convert graph object to CSV lines using serializer
            List<string> lines = GraphSerializer.SerializeGraph(graph);

            // Write all lines to file with UTF-8 encoding (supports Turkish characters)
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        public static Graph LoadGraph(string filePath)
        {
            // Check if file exists before trying to read
            if (!File.Exists(filePath))
            {
                // Get localized error message from Resources
                string errorMessage = string.Format(Properties.Resources.Err_FileNotFound, filePath);
                throw new FileNotFoundException(errorMessage, filePath);
            }

            // Read all lines from CSV file into string array
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // Parse CSV lines and create Graph object with nodes and edges
            Graph graph = GraphSerializer.ParseCsv(lines);

            // Position nodes in a circle (centerX=400, centerY=300, radius=200)
            LayoutStrategy.ApplyCircularLayout(graph, 400, 300, 200);

            return graph;
        }
    }
}