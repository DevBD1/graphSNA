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
        public string NodeName
        {
            get { return txtName.Text.Trim(); }
            set { txtName.Text = value; }
        }

        public float Activity
        {
            get
            {
                float.TryParse(txtActivity.Text, out float val);
                return val;
            }
            set { txtActivity.Text = value.ToString(); }
        }

        public float Interaction
        {
            get
            {
                float.TryParse(txtInteraction.Text, out float val);
                return val;
            }
            set { txtInteraction.Text = value.ToString(); }
        }

        // Constructor
        public InputNodeForm(string title)
        {
            // Designer dosyasındaki kodlar
            InitializeComponent();

            this.Text = title;

            // Designer'da butona DialogResult.OK verdiysek, tıklandığı an formu kapatır.
            // Validasyon yapabilmek için bunu 'None' yapıyoruz ki kontrol bizde olsun.
            btnOk.DialogResult = DialogResult.None;
        }

        // Kaydet butonuna basılınca çalışacak olay (Designer'da bağladık)
        private void btnOk_Click(object sender, EventArgs e)
        {
            // A. İsim Kontrolü (Boş olamaz)
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Lütfen bir isim giriniz.", "Eksik Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return; // Formu kapatma, geri dön
            }

            // B. Sayısal Değer Kontrolü
            // Kullanıcı sayı yerine harf girdiyse TryParse 0 döndürür, bu yüzden ayrıca kontrol edelim.
            bool actValid = float.TryParse(txtActivity.Text, out float actVal);
            bool intValid = float.TryParse(txtInteraction.Text, out float intVal);

            if (!actValid || !intValid)
            {
                MessageBox.Show("Lütfen Aktiflik ve Etkileşim için geçerli sayısal değerler giriniz.", "Hatalı Format", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // C. Aralık Kontrolü (0 ile 100 arası)
            if (actVal < 0 || actVal > 100 || intVal < 0 || intVal > 100)
            {
                MessageBox.Show("Değerler 0 ile 100 arasında olmalıdır.", "Sınır Hatası", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Her şey yolunda, formu "OK" sonucuyla kapatabiliriz.
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}