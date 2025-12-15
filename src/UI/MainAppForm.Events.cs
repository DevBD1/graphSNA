using System;
using System.Windows.Forms;
using graphSNA.Model;

namespace graphSNA.UI
{
    /// <summary>
    ///  Includes  the main application UI form.
    /// </summary>
    public partial class MainAppForm
    {
        /// <summary>
        ///  The method that connects buttons&panels to their event handlers
        /// </summary>
        private void RegisterEvents()
        {
            this.button1.Click += ImportCSV;
            this.button2.Click += ExportCSV;
            this.button3.Click += RefreshCanvas;
            this.panel1.Paint += GraphCanvas_Paint;
            this.panel1.MouseClick += PnlGraph_MouseClick;
        }
        /// <summary>
        ///  Method for importing the graph from a CSV file
        /// </summary>
        private void ImportCSV(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSV Dosyası|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                controller.LoadGraph(ofd.FileName);
                panel1.Invalidate();
                /// debug
                var graph = controller.ActiveGraph;
                MessageBox.Show($"Yüklenen Düğüm Sayısı: {graph.Nodes.Count}\nYüklenen Kenar (Bağlantı) Sayısı: {graph.Edges.Count}",
                                "Teşhis Raporu");
            }
        }
        /// <summary>
        ///  Method for exporting the graph to a CSV file
        /// </summary>
        private void ExportCSV(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV Dosyası|*.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                controller.SaveGraph(sfd.FileName);
                MessageBox.Show("Kayıt Başarılı.");
            }
        }
        /// <summary>
        ///  Method for applying the force-directed layout algorithm
        /// </summary>
        private void RefreshCanvas(object sender, EventArgs e)
        {
            controller.ApplyForceLayout(panel1.Width, panel1.Height);
            panel1.Invalidate(); // re-draw the panel with new locations
        }

        // --- MOUSE VE PAINT OLAYLARI DA BURADA OLABİLİR ---
        // (GraphCanvas_Paint ve PnlGraph_MouseClick metodlarını da buraya taşıyabilirsiniz)
    }
}