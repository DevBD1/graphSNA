using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace graphSNA.Model
{
    // Sorumluluk: Sadece Dosya Okuma/Yazma ve işlem sırasını yönetme.
    public static class FileManager
    {
        public static void SaveGraph(Graph graph, string filePath)
        {
            // 1. Veriyi hazırla (Serializer işi)
            List<string> lines = GraphSerializer.SerializeGraph(graph);

            // 2. Dosyayı yaz (IO işi)
            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        public static Graph LoadGraph(string filePath)
        {
            if (!File.Exists(filePath))
            {
                string errorMessage = string.Format(Properties.Resources.Err_FileNotFound, filePath);
                throw new FileNotFoundException(errorMessage, filePath);
            }

            // 1. Dosyayı oku (IO işi)
            string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

            // 2. Veriyi parse et (Serializer işi)
            Graph graph = GraphSerializer.ParseCsv(lines);

            // 3. Konumlandır (Layout işi) - Varsayılan değerler
            LayoutStrategy.ApplyCircularLayout(graph, 400, 300, 200);

            return graph;
        }
    }
}