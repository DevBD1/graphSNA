using System;
using System.Windows.Forms;
using System.IO;
using graphSNA.Model;
using graphSNA.Properties;

namespace graphSNA.UI
{
    /// <summary>
    ///  Represents the main application UI form.
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
            //this.button3.Click += RefreshCanvas;
            //this.button4.Click += RefreshDefaultLayout;
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
                try
                {
                    controller.LoadGraph(ofd.FileName);

                    // show short name in the label inside groupBox1
                    //labelActiveFile.Text = Path.GetFileName(ofd.FileName);

                    // optionally show full path in the group box caption or a tooltip
                    groupBox1.Text = $"Active file: {Path.GetFileName(ofd.FileName)}";
                    // if you add a ToolTip component
                    // toolTip1.SetToolTip(labelActiveFile, ofd.FileName); 

                    panel1.Invalidate();
                    /// debug
                    var graph = controller.ActiveGraph;
                    MessageBox.Show($"Yüklenen Düğüm Sayısı: {graph.Nodes.Count}\nYüklenen Kenar (Bağlantı) Sayısı: {graph.Edges.Count}",
                                "Teşhis Raporu");
                    MessageBox.Show(Properties.Resources.Msg_Success);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
        ///// <summary>
        /////  Method for applying the force-directed layout algorithm
        ///// </summary>
        //private void RefreshCanvas(object sender, EventArgs e)
        //{
        //    controller.ApplyForceLayout(panel1.Width, panel1.Height);
        //    // re-draw the panel with new locations
        //    panel1.Invalidate();
        //}
        ///// <summary>
        /////  Method for applying the node-and-edge weighted layout algorithm (the default draw).
        /////  This delegates to the controller's weighted/default layout implementation and requests a redraw.
        ///// </summary>
        //private void RefreshDefaultLayout(object sender, EventArgs e)
        //{
        //    // Controller should provide an ApplyWeightedLayout or ApplyDefaultLayout method.
        //    // If it doesn't exist yet, add it there following the same pattern as ApplyForceLayout.
        //    controller.ApplyWeightedLayout(panel1.Width, panel1.Height);
        //    // re-draw the panel with new locations
        //    panel1.Invalidate();
        //}
    }
}