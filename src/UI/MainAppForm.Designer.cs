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
            btnFindShortestPath = new Button();
            txtCost = new TextBox();
            lblCost = new Label();
            radioAstar = new RadioButton();
            radioDijsktra = new RadioButton();
            groupBox3 = new GroupBox();
            btnTraverse = new Button();
            btnColoring = new Button();
            radioBFS = new RadioButton();
            radioDFS = new RadioButton();
            groupBox4 = new GroupBox();
            label3 = new Label();
            label2 = new Label();
            textBox2 = new TextBox();
            button6 = new Button();
            button5 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            groupBox5 = new GroupBox();
            button8 = new Button();
            button7 = new Button();
            tabPage2 = new TabPage();
            dataGridView1 = new DataGridView();
            textBox3 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox5.SuspendLayout();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(panel1);
            // 
            // splitContainer1.Panel2
            // 
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
            flowLayoutPanel1.Controls.Add(groupBox3);
            flowLayoutPanel1.Controls.Add(groupBox4);
            flowLayoutPanel1.Controls.Add(groupBox5);
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
            groupBox2.Controls.Add(btnFindShortestPath);
            groupBox2.Controls.Add(txtCost);
            groupBox2.Controls.Add(lblCost);
            groupBox2.Controls.Add(radioAstar);
            groupBox2.Controls.Add(radioDijsktra);
            groupBox2.Location = new Point(3, 86);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(238, 147);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Shortest Path";
            // 
            // btnFindShortestPath
            // 
            btnFindShortestPath.Location = new Point(5, 108);
            btnFindShortestPath.Name = "btnFindShortestPath";
            btnFindShortestPath.Size = new Size(226, 30);
            btnFindShortestPath.TabIndex = 5;
            btnFindShortestPath.Text = "Find Shortest Path";
            btnFindShortestPath.UseVisualStyleBackColor = true;
            btnFindShortestPath.Click += RunFindShortestPath;
            // 
            // txtCost
            // 
            txtCost.Location = new Point(46, 69);
            txtCost.Name = "txtCost";
            txtCost.ReadOnly = true;
            txtCost.Size = new Size(185, 23);
            txtCost.TabIndex = 4;
            // 
            // lblCost
            // 
            lblCost.AutoSize = true;
            lblCost.Location = new Point(6, 72);
            lblCost.Name = "lblCost";
            lblCost.Size = new Size(34, 15);
            lblCost.TabIndex = 3;
            lblCost.Text = "Cost:";
            // 
            // radioAstar
            // 
            radioAstar.AutoSize = true;
            radioAstar.Location = new Point(6, 47);
            radioAstar.Name = "radioAstar";
            radioAstar.Size = new Size(38, 19);
            radioAstar.TabIndex = 1;
            radioAstar.Text = "A*";
            radioAstar.UseVisualStyleBackColor = true;
            // 
            // radioDijsktra
            // 
            radioDijsktra.AutoSize = true;
            radioDijsktra.Checked = true;
            radioDijsktra.Location = new Point(6, 22);
            radioDijsktra.Name = "radioDijsktra";
            radioDijsktra.Size = new Size(64, 19);
            radioDijsktra.TabIndex = 0;
            radioDijsktra.TabStop = true;
            radioDijsktra.Text = "Dijkstra";
            radioDijsktra.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnTraverse);
            groupBox3.Controls.Add(btnColoring);
            groupBox3.Controls.Add(radioBFS);
            groupBox3.Controls.Add(radioDFS);
            groupBox3.Location = new Point(3, 239);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(238, 125);
            groupBox3.TabIndex = 2;
            groupBox3.TabStop = false;
            groupBox3.Text = "Graph Traversal";
            // 
            // btnTraverse
            // 
            btnTraverse.Location = new Point(121, 22);
            btnTraverse.Name = "btnTraverse";
            btnTraverse.Size = new Size(111, 44);
            btnTraverse.TabIndex = 9;
            btnTraverse.Text = "Run Traversal";
            btnTraverse.UseVisualStyleBackColor = true;
            // 
            // btnColoring
            // 
            btnColoring.Location = new Point(5, 83);
            btnColoring.Name = "btnColoring";
            btnColoring.Size = new Size(226, 35);
            btnColoring.TabIndex = 10;
            btnColoring.Text = "Grafı Renklendir (Welsh-Powell)";
            btnColoring.UseVisualStyleBackColor = true;
            // 
            // radioBFS
            // 
            radioBFS.AutoSize = true;
            radioBFS.Checked = true;
            radioBFS.Location = new Point(6, 22);
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
            radioDFS.Location = new Point(5, 47);
            radioDFS.Name = "radioDFS";
            radioDFS.Size = new Size(45, 19);
            radioDFS.TabIndex = 8;
            radioDFS.Text = "DFS";
            radioDFS.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(textBox3);
            groupBox4.Controls.Add(label3);
            groupBox4.Controls.Add(label2);
            groupBox4.Controls.Add(textBox2);
            groupBox4.Controls.Add(button6);
            groupBox4.Controls.Add(button5);
            groupBox4.Controls.Add(label1);
            groupBox4.Controls.Add(textBox1);
            groupBox4.Location = new Point(3, 370);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(238, 147);
            groupBox4.TabIndex = 3;
            groupBox4.TabStop = false;
            groupBox4.Text = "Node";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(7, 78);
            label3.Name = "label3";
            label3.Size = new Size(70, 15);
            label3.TabIndex = 6;
            label3.Text = "Interaction: ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 50);
            label2.Name = "label2";
            label2.Size = new Size(53, 15);
            label2.TabIndex = 5;
            label2.Text = "Activity: ";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(82, 50);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(150, 23);
            textBox2.TabIndex = 4;
            // 
            // button6
            // 
            button6.Location = new Point(120, 113);
            button6.Name = "button6";
            button6.Size = new Size(111, 23);
            button6.TabIndex = 3;
            button6.Text = "Delete";
            button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            button5.Location = new Point(5, 113);
            button5.Name = "button5";
            button5.Size = new Size(111, 23);
            button5.TabIndex = 2;
            button5.Text = "Edit";
            button5.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 22);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 1;
            label1.Text = "Selected:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(82, 22);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(149, 23);
            textBox1.TabIndex = 0;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(button8);
            groupBox5.Controls.Add(button7);
            groupBox5.Location = new Point(3, 523);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(238, 55);
            groupBox5.TabIndex = 4;
            groupBox5.TabStop = false;
            groupBox5.Text = "Edge";
            // 
            // button8
            // 
            button8.Location = new Point(121, 22);
            button8.Name = "button8";
            button8.Size = new Size(110, 23);
            button8.TabIndex = 1;
            button8.Text = "Remove";
            button8.UseVisualStyleBackColor = true;
            // 
            // button7
            // 
            button7.Location = new Point(6, 22);
            button7.Name = "button7";
            button7.Size = new Size(111, 23);
            button7.TabIndex = 0;
            button7.Text = "Add";
            button7.UseVisualStyleBackColor = true;
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
            // textBox3
            // 
            textBox3.Location = new Point(82, 78);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(150, 23);
            textBox3.TabIndex = 7;
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
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox5.ResumeLayout(false);
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
        private RadioButton radioAstar;
        private RadioButton radioDijsktra;
        private RadioButton radioBFS;
        private RadioButton radioDFS;
        private Button btnTraverse;

        //coloring button
        private Button btnColoring;
        private GroupBox groupBox3;
        private GroupBox groupBox4;
        private Label label1;
        private TextBox textBox1;
        private Button button6;
        private Button button5;
        private GroupBox groupBox5;
        private Button button8;
        private Button button7;
        private Label label3;
        private Label label2;
        private TextBox textBox2;
        private TextBox textBox3;
    }
}