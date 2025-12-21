namespace graphSNA.UI
{
    partial class MainAppForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainAppForm));
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            groupBox2 = new GroupBox();
            tabPage2 = new TabPage();
            dataGridView1 = new DataGridView();
            radioDijsktra = new RadioButton();
            radioAstar = new RadioButton();
            labelSelectShortestPath = new Label();
            lblCost = new Label();
            txtCost = new TextBox();
            btnFindShortestPath = new Button();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(6, 7, 6, 7);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(2280, 1680);
            splitContainer1.SplitterDistance = 1714;
            splitContainer1.SplitterWidth = 9;
            splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(6, 7, 6, 7);
            panel1.Name = "panel1";
            panel1.Size = new Size(1714, 1680);
            panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Margin = new Padding(6, 7, 6, 7);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(557, 1680);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.ActiveCaption;
            tabPage1.Controls.Add(flowLayoutPanel1);
            tabPage1.Location = new Point(4, 46);
            tabPage1.Margin = new Padding(6, 7, 6, 7);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(6, 7, 6, 7);
            tabPage1.Size = new Size(549, 1630);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Functions";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BackColor = SystemColors.ActiveCaption;
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(6, 7);
            flowLayoutPanel1.Margin = new Padding(6, 7, 6, 7);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(537, 1616);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Location = new Point(6, 7);
            groupBox1.Margin = new Padding(6, 7, 6, 7);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(6, 7, 6, 7);
            groupBox1.Size = new Size(510, 257);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Data Source";
            // 
            // button3
            // 
            button3.Location = new Point(13, 180);
            button3.Margin = new Padding(6, 7, 6, 7);
            button3.Name = "button3";
            button3.Size = new Size(484, 57);
            button3.TabIndex = 2;
            button3.Text = "Save";
            button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(259, 109);
            button2.Margin = new Padding(6, 7, 6, 7);
            button2.Name = "button2";
            button2.Size = new Size(238, 57);
            button2.TabIndex = 1;
            button2.Text = "Export";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(13, 109);
            button1.Margin = new Padding(6, 7, 6, 7);
            button1.Name = "button1";
            button1.Size = new Size(238, 57);
            button1.TabIndex = 0;
            button1.Text = "Import";
            button1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnFindShortestPath);
            groupBox2.Controls.Add(txtCost);
            groupBox2.Controls.Add(lblCost);
            groupBox2.Controls.Add(labelSelectShortestPath);
            groupBox2.Controls.Add(radioAstar);
            groupBox2.Controls.Add(radioDijsktra);
            groupBox2.Location = new Point(6, 278);
            groupBox2.Margin = new Padding(6, 7, 6, 7);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(6, 7, 6, 7);
            groupBox2.Size = new Size(510, 405);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "groupBox2";
            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.ActiveCaption;
            tabPage2.Controls.Add(dataGridView1);
            tabPage2.Location = new Point(4, 46);
            tabPage2.Margin = new Padding(6, 7, 6, 7);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(6, 7, 6, 7);
            tabPage2.Size = new Size(549, 1630);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Stats";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.Location = new Point(6, 7);
            dataGridView1.Margin = new Padding(6, 7, 6, 7);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.RowHeadersWidth = 76;
            dataGridView1.Size = new Size(537, 493);
            dataGridView1.TabIndex = 0;
            // 
            // radioDijsktra
            // 
            radioDijsktra.AutoSize = true;
            radioDijsktra.Checked = true;
            radioDijsktra.Location = new Point(24, 102);
            radioDijsktra.Name = "radioDijsktra";
            radioDijsktra.Size = new Size(124, 41);
            radioDijsktra.TabIndex = 0;
            radioDijsktra.TabStop = true;
            radioDijsktra.Text = "Dikstra";
            radioDijsktra.UseVisualStyleBackColor = true;
            // 
            // radioAstar
            // 
            radioAstar.AutoSize = true;
            radioAstar.Location = new Point(259, 102);
            radioAstar.Name = "radioAstar";
            radioAstar.Size = new Size(70, 41);
            radioAstar.TabIndex = 1;
            radioAstar.Text = "A*";
            radioAstar.UseVisualStyleBackColor = true;
            // 
            // labelSelectShortestPath
            // 
            labelSelectShortestPath.AutoSize = true;
            labelSelectShortestPath.Location = new Point(9, 43);
            labelSelectShortestPath.Name = "labelSelectShortestPath";
            labelSelectShortestPath.Size = new Size(376, 37);
            labelSelectShortestPath.TabIndex = 2;
            labelSelectShortestPath.Text = "Select Shortest Path Algorithm";
            // 
            // lblCost
            // 
            lblCost.AutoSize = true;
            lblCost.Location = new Point(24, 161);
            lblCost.Name = "lblCost";
            lblCost.Size = new Size(76, 37);
            lblCost.TabIndex = 3;
            lblCost.Text = "Cost:";
            // 
            // txtCost
            // 
            txtCost.Location = new Point(130, 161);
            txtCost.Name = "txtCost";
            txtCost.ReadOnly = true;
            txtCost.Size = new Size(185, 43);
            txtCost.TabIndex = 4;
            txtCost.TextChanged += textBox1_TextChanged;
            // 
            // btnFindShortestPath
            // 
            btnFindShortestPath.Location = new Point(24, 210);
            btnFindShortestPath.Name = "btnFindShortestPath";
            btnFindShortestPath.Size = new Size(473, 43);
            btnFindShortestPath.TabIndex = 5;
            btnFindShortestPath.Text = "Find Shortest Path";
            btnFindShortestPath.UseVisualStyleBackColor = true;
            btnFindShortestPath.Click += findShortestPathButton_Click;
            // 
            // MainAppForm
            // 
            AutoScaleDimensions = new SizeF(15F, 37F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(2280, 1680);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(6, 7, 6, 7);
            Name = "MainAppForm";
            Text = "graphSNA";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private Panel panel1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private FlowLayoutPanel flowLayoutPanel1;
        private TabPage tabPage2;
        private GroupBox groupBox1;
        private Button button2;
        private Button button1;
        private GroupBox groupBox2;
        private Button button3;
        private DataGridView dataGridView1;
        private Button btnFindShortestPath;
        private TextBox txtCost;
        private Label lblCost;
        private Label labelSelectShortestPath;
        private RadioButton radioAstar;
        private RadioButton radioDijsktra;
    }
}