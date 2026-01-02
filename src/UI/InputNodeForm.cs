using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace graphSNA.UI
{
    /// <summary>
    /// Dialog form for adding or editing node properties.
    /// Used for creating new users or modifying existing ones.
    /// </summary>
    public partial class InputNodeForm : Form
    {
        // Properties bound to text fields - accessed after dialog closes
        public string NodeName { get; set; }
        public float Activity { get; set; }
        public float Interaction { get; set; }

        // Constructor
        public InputNodeForm(string title)
        {
            // Tüm sihri bu metod yapacak (Designer dosyasındaki kodlar)
            InitializeComponent();

            this.Text = title;
        }

        // Kaydet butonuna basılınca çalışacak olay (Designer'da bağladık)
        private void btnOk_Click(object sender, EventArgs e)
        {
            NodeName = txtName.Text;

            float.TryParse(txtActivity.Text, out float act);
            float.TryParse(txtInteraction.Text, out float inter);

            Activity = act;
            Interaction = inter;
        }
    }
}