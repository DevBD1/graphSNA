using System.Diagnostics;
using graphSNA.Model;

namespace graphSNA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // Run our quick test
            RunBackendTest();
        }

        private void RunBackendTest()
        {
            // 1. Create the Graph Manager
            Graph myGraph = new Graph();

            // 2. Create two test Nodes (Users)
            // Name, Activity, Interaction, ConnectionCount
            Node u1 = new Node("Ali", 10, 5, 2);
            Node u2 = new Node("Veli", 20, 10, 5);

            // 3. Add them to the graph
            myGraph.AddNode(u1);
            myGraph.AddNode(u2);

            // 4. Create a connection (Edge)
            myGraph.AddEdge(u1, u2);

            // 5. Print results to the "Output" window in Visual Studio
            Debug.WriteLine("---------------- TEST START ----------------");
            Debug.WriteLine($"Total Nodes: {myGraph.Nodes.Count}"); // Should be 2
            Debug.WriteLine($"Total Edges: {myGraph.Edges.Count}"); // Should be 1
            Debug.WriteLine($"Edge Weight: {myGraph.Edges[0].Weight}"); // Should be 1.0 (default)
            Debug.WriteLine("---------------- TEST END ------------------");

            // 6. Show the result as POP-UP
            string sonucMesaji = $"TEST BAŞARILI!\n\n" +
                                 $"Toplam Düğüm (Nodes): {myGraph.Nodes.Count}\n" +
                                 $"Toplam Kenar (Edges): {myGraph.Edges.Count}\n" +
                                 $"Kenar Ağırlığı: {myGraph.Edges[0].Weight}";

            MessageBox.Show(sonucMesaji, "graphSNA Backend Testi", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
