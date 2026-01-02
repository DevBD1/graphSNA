namespace graphSNA.UI
{
    partial class InputNodeForm
    {
        private System.ComponentModel.IContainer components = null;

        // Kontrol elemanları (Artık burada tanımlı)
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtActivity;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInteraction;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            label1 = new Label();
            txtName = new TextBox();
            label2 = new Label();
            txtActivity = new TextBox();
            label3 = new Label();
            txtInteraction = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(20, 25);
            label1.Name = "label1";
            label1.Size = new Size(32, 15);
            label1.TabIndex = 0;
            label1.Text = "İsim:";
            // 
            // txtName
            // 
            txtName.Location = new Point(140, 22);
            txtName.Name = "txtName";
            txtName.Size = new Size(180, 23);
            txtName.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(20, 65);
            label2.Name = "label2";
            label2.Size = new Size(90, 15);
            label2.TabIndex = 2;
            label2.Text = "Activity (0-100):";
            // 
            // txtActivity
            // 
            txtActivity.Location = new Point(140, 62);
            txtActivity.Name = "txtActivity";
            txtActivity.Size = new Size(180, 23);
            txtActivity.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(20, 105);
            label3.Name = "label3";
            label3.Size = new Size(107, 15);
            label3.TabIndex = 4;
            label3.Text = "Interaction (0-100):";
            // 
            // txtInteraction
            // 
            txtInteraction.Location = new Point(140, 102);
            txtInteraction.Name = "txtInteraction";
            txtInteraction.Size = new Size(180, 23);
            txtInteraction.TabIndex = 5;
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(50, 160);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(100, 30);
            btnOk.TabIndex = 6;
            btnOk.Text = "Kaydet";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(180, 160);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 30);
            btnCancel.TabIndex = 7;
            btnCancel.Text = "İptal";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // InputNodeForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(350, 211);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtInteraction);
            Controls.Add(label3);
            Controls.Add(txtActivity);
            Controls.Add(label2);
            Controls.Add(txtName);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputNodeForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "InputNodeForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}