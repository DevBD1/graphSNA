using System;
using System.Drawing;
using System.Windows.Forms;

namespace graphSNA.UI
{
    /// <summary>
    /// Dialog form for adding or editing node properties.
    /// Used for creating new users or modifying existing ones.
    /// </summary>
    public partial class InputNodeForm : Form
    {
        // Properties bound to text fields - accessed after dialog closes
        public string NodeName { get { return txtName.Text; } set { txtName.Text = value; } }
        public float Activity { get { float.TryParse(txtActivity.Text, out float v); return v; } set { txtActivity.Text = value.ToString(); } }
        public float Interaction { get { float.TryParse(txtInteraction.Text, out float v); return v; } set { txtInteraction.Text = value.ToString(); } }

        private TextBox txtName, txtActivity, txtInteraction;
        private Button btnSave, btnCancel;

        public InputNodeForm(string title = "Düğüm Bilgileri")
        {
            this.Text = title;
            this.Size = new Size(300, 220);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            int lblX = 20;
            int txtX = 130;

            // Name field
            AddLabel("İsim:", lblX, y);
            txtName = AddTextBox(txtX, y);

            // Activity field (0-100)
            y += 40;
            AddLabel("Aktiflik (0-100):", lblX, y);
            txtActivity = AddTextBox(txtX, y);

            // Interaction field (0-100)
            y += 40;
            AddLabel("Etkileşim (0-100):", lblX, y);
            txtInteraction = AddTextBox(txtX, y);

            // Buttons
            y += 50;
            btnSave = new Button() { Text = "Kaydet", DialogResult = DialogResult.OK, Location = new Point(50, y), Size = new Size(80, 30), BackColor = Color.LightGreen };
            btnCancel = new Button() { Text = "İptal", DialogResult = DialogResult.Cancel, Location = new Point(150, y), Size = new Size(80, 30) };

            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave;
        }

        // Helper: creates and adds a label to the form
        private void AddLabel(string text, int x, int y)
        {
            Label l = new Label() { Text = text, Location = new Point(x, y + 3), AutoSize = true };
            this.Controls.Add(l);
        }

        // Helper: creates and adds a textbox to the form
        private TextBox AddTextBox(int x, int y)
        {
            TextBox t = new TextBox() { Location = new Point(x, y), Width = 140 };
            this.Controls.Add(t);
            return t;
        }
    }
}