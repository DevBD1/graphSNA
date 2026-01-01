using System;
using System.Drawing;
using System.Windows.Forms;

namespace graphSNA.UI
{
    public partial class InputNodeForm : Form
    {
        // Komşular Property'si GİTTİ! Sadece temel özellikler kaldı.
        public string NodeName { get { return txtName.Text; } set { txtName.Text = value; } }
        public float Activity { get { float.TryParse(txtActivity.Text, out float v); return v; } set { txtActivity.Text = value.ToString(); } }
        public float Interaction { get { float.TryParse(txtInteraction.Text, out float v); return v; } set { txtInteraction.Text = value.ToString(); } }

        private TextBox txtName, txtActivity, txtInteraction;
        private Button btnSave, btnCancel;

        public InputNodeForm(string title = "Düğüm Bilgileri")
        {
            this.Text = title;
            this.Size = new Size(300, 220); // Boyut küçüldü
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            int lblX = 20;
            int txtX = 130;

            // 1. İsim
            AddLabel("İsim:", lblX, y);
            txtName = AddTextBox(txtX, y);

            // 2. Aktiflik
            y += 40;
            AddLabel("Aktiflik (0-100):", lblX, y);
            txtActivity = AddTextBox(txtX, y);

            // 3. Etkileşim
            y += 40;
            AddLabel("Etkileşim (0-100):", lblX, y);
            txtInteraction = AddTextBox(txtX, y);

            // Butonlar
            y += 50;
            btnSave = new Button() { Text = "Kaydet", DialogResult = DialogResult.OK, Location = new Point(50, y), Size = new Size(80, 30), BackColor = Color.LightGreen };
            btnCancel = new Button() { Text = "İptal", DialogResult = DialogResult.Cancel, Location = new Point(150, y), Size = new Size(80, 30) };

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave;
        }

        private void AddLabel(string text, int x, int y)
        {
            Label l = new Label() { Text = text, Location = new Point(x, y + 3), AutoSize = true };
            this.Controls.Add(l);
        }

        private TextBox AddTextBox(int x, int y)
        {
            TextBox t = new TextBox() { Location = new Point(x, y), Width = 140 };
            this.Controls.Add(t);
            return t;
        }
    }
}