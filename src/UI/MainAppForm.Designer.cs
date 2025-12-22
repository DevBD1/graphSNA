namespace graphSNA.UI
{
    partial class MainAppForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainAppForm));
            splitContainer1 = new SplitContainer();
            panel1 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            flowLayoutPanel1 = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            button4 = new Button();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            groupBox2 = new GroupBox();
            
            
            labelTraversal = new Label();
            radioBFS = new RadioButton();
            radioDFS = new RadioButton();
            btnTraverse = new Button();
      

            btnFindShortestPath = new Button();
            txtCost = new TextBox();
            lblCost = new Label();
            labelSelectShortestPath = new Label();
            radioAstar = new RadioButton();
            radioDijsktra = new RadioButton();
            tabPage2 = new TabPage();
            dataGridView1 = new DataGridView();
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
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Panel1.Controls.Add(panel1);
            splitContainer1.Panel2.Controls.Add(tabControl1);
            splitContainer1.Size = new Size(1064, 681);
            splitContainer1.SplitterDistance = 800;
            splitContainer1.TabIndex = 0;
            // 
            // panel1
            // 
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(800, 681);
            panel1.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(260, 681);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.BackColor = SystemColors.ActiveCaption;
            tabPage1.Controls.Add(flowLayoutPanel1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(252, 653);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Functions";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.BackColor = SystemColors.ActiveCaption;
            flowLayoutPanel1.Controls.Add(groupBox1);
            flowLayoutPanel1.Controls.Add(groupBox2);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(3, 3);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(246, 647);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(button4);
            groupBox1.Controls.Add(button3);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(button1);
            groupBox1.Location = new Point(3, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(238, 77);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "<import a file>";
            // 
            // button4
            // 
            button4.BackColor = Color.Transparent;
            button4.Location = new Point(121, 47);
            button4.Name = "button4";
            button4.Size = new Size(111, 23);
            button4.TabIndex = 3;
            button4.Text = "Weighted";
            button4.UseVisualStyleBackColor = false;
            // 
            // button3
            // 
            button3.BackColor = Color.Transparent;
            button3.Location = new Point(6, 47);
            button3.Name = "button3";
            button3.Size = new Size(111, 23);
            button3.TabIndex = 2;
            button3.Text = "Force-Directed";
            button3.UseVisualStyleBackColor = false;
            // 
            // button2
            // 
            button2.Location = new Point(121, 18);
            button2.Name = "button2";
            button2.Size = new Size(111, 23);
            button2.TabIndex = 1;
            button2.Text = "Export";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(6, 18);
            button1.Name = "button1";
            button1.Size = new Size(111, 23);
            button1.TabIndex = 0;
            button1.Text = "Import";
            button1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(btnTraverse); // YENİ
            groupBox2.Controls.Add(radioDFS);    // YENİ
            groupBox2.Controls.Add(radioBFS);    // YENİ
            groupBox2.Controls.Add(labelTraversal); // YENİ
            groupBox2.Controls.Add(btnFindShortestPath);
            groupBox2.Controls.Add(txtCost);
            groupBox2.Controls.Add(lblCost);
            groupBox2.Controls.Add(labelSelectShortestPath);
            groupBox2.Controls.Add(radioAstar);
            groupBox2.Controls.Add(radioDijsktra);
            groupBox2.Location = new Point(3, 86);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(238, 250); // Boyut arttı
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Analysis";
            // 
            // labelSelectShortestPath
            // 
            labelSelectShortestPath.AutoSize = true;
            labelSelectShortestPath.Location = new Point(6, 19);
            labelSelectShortestPath.Name = "labelSelectShortestPath";
            labelSelectShortestPath.Size = new Size(94, 15);
            labelSelectShortestPath.TabIndex = 2;
            labelSelectShortestPath.Text = "Shortest Path:";
            // 
            // radioDijsktra
            // 
            radioDijsktra.AutoSize = true;
            radioDijsktra.Checked = true;
            radioDijsktra.Location = new Point(6, 37);
            radioDijsktra.Name = "radioDijsktra";
            radioDijsktra.Size = new Size(63, 19);
            radioDijsktra.TabIndex = 0;
            radioDijsktra.TabStop = true;
            radioDijsktra.Text = "Dijkstra";
            radioDijsktra.UseVisualStyleBackColor = true;
            // 
            // radioAstar
            // 
            radioAstar.AutoSize = true;
            radioAstar.Location = new Point(6, 62);
            radioAstar.Name = "radioAstar";
            radioAstar.Size = new Size(39, 19);
            radioAstar.TabIndex = 1;
            radioAstar.Text = "A*";
            radioAstar.UseVisualStyleBackColor = true;
            // 
            // lblCost
            // 
            lblCost.AutoSize = true;
            lblCost.Location = new Point(6, 100);
            lblCost.Name = "lblCost";
            lblCost.Size = new Size(34, 15);
            lblCost.TabIndex = 3;
            lblCost.Text = "Cost:";
            // 
            // txtCost
            // 
            txtCost.Location = new Point(47, 97);
            txtCost.Name = "txtCost";
            txtCost.ReadOnly = true;
            txtCost.Size = new Size(185, 23);
            txtCost.TabIndex = 4;
            // 
            // btnFindShortestPath
            // 
            btnFindShortestPath.Location = new Point(6, 126);
            btnFindShortestPath.Name = "btnFindShortestPath";
            btnFindShortestPath.Size = new Size(226, 30);
            btnFindShortestPath.TabIndex = 5;
            btnFindShortestPath.Text = "Find Shortest Path";
            btnFindShortestPath.UseVisualStyleBackColor = true;
            
            // --- BFS / DFS KONTROLLERİ ---
            // 
            // labelTraversal
            // 
            labelTraversal.AutoSize = true;
            labelTraversal.Location = new Point(6, 165);
            labelTraversal.Name = "labelTraversal";
            labelTraversal.Size = new Size(100, 15);
            labelTraversal.TabIndex = 6;
            labelTraversal.Text = "Graph Traversal:";
            // 
            // radioBFS
            // 
            radioBFS.AutoSize = true;
            radioBFS.Checked = true;
            radioBFS.Location = new Point(6, 185);
            radioBFS.Name = "radioBFS";
            radioBFS.Size = new Size(44, 19);
            radioBFS.TabIndex = 7;
            radioBFS.TabStop = true;
            radioBFS.Text = "BFS";
            radioBFS.UseVisualStyleBackColor = true;
            // 
            // radioDFS
            // 
            radioDFS.AutoSize = true;
            radioDFS.Location = new Point(6, 210);
            radioDFS.Name = "radioDFS";
            radioDFS.Size = new Size(45, 19);
            radioDFS.TabIndex = 8;
            radioDFS.Text = "DFS";
            radioDFS.UseVisualStyleBackColor = true;
            // 
            // btnTraverse
            // 
            btnTraverse.Location = new Point(121, 185);
            btnTraverse.Name = "btnTraverse";
            btnTraverse.Size = new Size(111, 44);
            btnTraverse.TabIndex = 9;
            btnTraverse.Text = "Run Traversal";
            btnTraverse.UseVisualStyleBackColor = true;
            // -----------------------------

            // 
            // tabPage2
            // 
            tabPage2.BackColor = SystemColors.ActiveCaption;
            tabPage2.Controls.Add(dataGridView1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(252, 653);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Stats";
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Top;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(246, 200);
            dataGridView1.TabIndex = 0;
            // 
            // MainAppForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(1064, 681);
            Controls.Add(splitContainer1);
            Icon = (Icon)resources.GetObject("$this.Icon");
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
        private Button button4;
        
        // Shortest Path Controls
        private Button btnFindShortestPath;
        private TextBox txtCost;
        private Label lblCost;
        private Label labelSelectShortestPath;
        private RadioButton radioAstar;
        private RadioButton radioDijsktra;

        // NEW Traversal Controls
        private Label labelTraversal;
        private RadioButton radioBFS;
        private RadioButton radioDFS;
        private Button btnTraverse;
    }
}