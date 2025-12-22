using System;
using System.Windows.Forms;
using System.IO;
using graphSNA.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace graphSNA.UI
{
    /// <summary>
    ///  Handles Event Wiring and Button Click Logic
    /// </summary>
    public partial class MainAppForm
    {
        private void RegisterEvents()
        {
            // Dosya İşlemleri
            this.button1.Click += ImportCSV;
            this.button2.Click += ExportCSV;

            // Mouse ve Çizim
            this.panel1.Paint += GraphCanvas_Paint;
            this.panel1.MouseClick += PnlGraph_MouseClick;

            // --- YENİ EKLENENLER ---
            // Shortest Path Butonu
            this.btnFindShortestPath.Click += btnFindShortestPath_Click;

            // Traversal (BFS/DFS) Butonu
            this.btnTraverse.Click += btnTraverse_Click;
        }

        // --- 1. DOSYA İŞLEMLERİ ---
        private void ImportCSV(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Dosyası|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    controller.LoadGraph(ofd.FileName);
                    groupBox1.Text = $"Aktif Dosya: {Path.GetFileName(ofd.FileName)}";
                    panel1.Invalidate();
                    MessageBox.Show(Properties.Resources.Msg_Success);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportCSV(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Dosyası|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                controller.SaveGraph(sfd.FileName);
                MessageBox.Show("Dosya kaydedildi.");
            }
        }

        // --- 2. EN KISA YOL (SHORTEST PATH) MANTIĞI ---
        private void btnFindShortestPath_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 2)
            {
                MessageBox.Show("Lütfen önce bir graf yükleyin.", "Uyarı");
                return;
            }

            // Modu Aç
            isSelectingNodesForPathFinding = true;
            isSelectingForTraversal = false; // Diğer modları kapat
            startNodeForPathFinding = null;
            endNodeForPathFinding = null;

            MessageBox.Show("Lütfen haritadan BAŞLANGIÇ düğümüne tıklayın.", "Adım 1");
        }

        private void HandleShortestPathSelection(Node clickedNode)
        {
            if (startNodeForPathFinding == null)
            {
                if (clickedNode != null)
                {
                    startNodeForPathFinding = clickedNode;
                    selectedNode = clickedNode;
                    MessageBox.Show($"Başlangıç: {clickedNode.Name}\nŞimdi HEDEF düğüme tıklayın.", "Adım 2");
                }
            }
            else if (endNodeForPathFinding == null)
            {
                if (clickedNode != null && clickedNode != startNodeForPathFinding)
                {
                    endNodeForPathFinding = clickedNode;
                    isSelectingNodesForPathFinding = false;
                    RunShortestPathAlgorithm();
                }
                else if (clickedNode == startNodeForPathFinding)
                {
                    MessageBox.Show("Başlangıç ve Bitiş aynı olamaz!", "Hata");
                }
            }
            panel1.Invalidate();
        }

        private void RunShortestPathAlgorithm()
        {
            string algoType = radioAstar.Checked ? "A*" : "Dijkstra";
            var result = controller.CalculateShortestPath(startNodeForPathFinding, endNodeForPathFinding, algoType);

            if (result.path.Count > 0)
            {
                string pathStr = string.Join(" -> ", result.path.Select(n => n.Name));
                txtCost.Text = $"{result.cost:F2}";
                MessageBox.Show($"Yol: {pathStr}\nMaliyet: {result.cost:F2}", "Sonuç");
            }
            else
            {
                txtCost.Text = "Yok";
                MessageBox.Show("Yol bulunamadı.", "Sonuç");
            }

            // Renkleri temizlemek istersen burayı aç:
            // startNodeForPathFinding = null;
            // endNodeForPathFinding = null;
            panel1.Invalidate();
        }

        // --- 3. TRAVERSAL (BFS/DFS) MANTIĞI (YENİ) ---
        private void btnTraverse_Click(object sender, EventArgs e)
        {
            if (controller.ActiveGraph == null || controller.ActiveGraph.Nodes.Count < 1)
            {
                MessageBox.Show("Lütfen bir graf yükleyin.");
                return;
            }

            // Modu Aç
            isSelectingForTraversal = true;
            isSelectingNodesForPathFinding = false;

            MessageBox.Show("Taramayı başlatmak için bir BAŞLANGIÇ düğümüne tıklayın.", "Traversal Modu");
        }

        private void RunTraversalAlgorithm(Node startNode)
        {
            string algo = radioDFS.Checked ? "DFS" : "BFS";

            // Controller'ı Çağır
            List<Node> resultOrder = controller.TraverseGraph(startNode, algo);

            // Sonucu Göster
            if (resultOrder.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Algoritma: {algo}");
                sb.AppendLine($"Başlangıç: {startNode.Name}");
                sb.AppendLine($"Ziyaret Edilen: {resultOrder.Count} düğüm");
                sb.AppendLine("--------------------------------");

                // Format: Ali -> Veli -> Ayşe ...
                string flow = string.Join(" -> ", resultOrder.Select(n => n.Name));
                sb.AppendLine(flow);

                MessageBox.Show(sb.ToString(), "Tarama Sonucu");
            }
        }
    }
}