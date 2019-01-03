namespace TMesh_2._0
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distanciaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.distanceSelector = new System.Windows.Forms.ToolStripComboBox();
            this.editarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.ayudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ayudaToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chBoxShowAxes = new System.Windows.Forms.CheckBox();
            this.btn_segment = new System.Windows.Forms.Button();
            this.lblZoom = new System.Windows.Forms.Label();
            this.scrollZoom = new System.Windows.Forms.HScrollBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblFar = new System.Windows.Forms.Label();
            this.lblMid = new System.Windows.Forms.Label();
            this.lblNear = new System.Windows.Forms.Label();
            this.chkFarest = new System.Windows.Forms.CheckBox();
            this.btnCeldas = new System.Windows.Forms.Button();
            this.chbJustDistances = new System.Windows.Forms.CheckBox();
            this.Board = new OpenTK.GLControl();
            this.lblInfo = new System.Windows.Forms.Label();
            this.chkLines = new System.Windows.Forms.CheckBox();
            this.visibilityBtn = new System.Windows.Forms.Button();
            this.btnBbox = new System.Windows.Forms.Button();
            this.lblK = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btnCmp = new System.Windows.Forms.Button();
            this.lblSim = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.distanciaToolStripMenuItem,
            this.editarToolStripMenuItem,
            this.ayudaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(891, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.abrirToolStripMenuItem,
            this.salirToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            this.archivoToolStripMenuItem.Size = new System.Drawing.Size(60, 20);
            this.archivoToolStripMenuItem.Text = "Archivo";
            // 
            // abrirToolStripMenuItem
            // 
            this.abrirToolStripMenuItem.Name = "abrirToolStripMenuItem";
            this.abrirToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.abrirToolStripMenuItem.Text = "Abrir";
            this.abrirToolStripMenuItem.Click += new System.EventHandler(this.OpenOFF);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.Exit);
            // 
            // distanciaToolStripMenuItem
            // 
            this.distanciaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.distanceSelector});
            this.distanciaToolStripMenuItem.Name = "distanciaToolStripMenuItem";
            this.distanciaToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.distanciaToolStripMenuItem.Text = "Distancia";
            // 
            // distanceSelector
            // 
            this.distanceSelector.Items.AddRange(new object[] {
            "Angular",
            "Geodesica",
            "Volumetrica",
            "Producto Angular-Geodesica",
            "Combinacion Angular-Geodesica"});
            this.distanceSelector.Name = "distanceSelector";
            this.distanceSelector.Size = new System.Drawing.Size(121, 23);
            this.distanceSelector.SelectedIndexChanged += new System.EventHandler(this.distanceSelector_SelectedIndexChanged);
            // 
            // editarToolStripMenuItem
            // 
            this.editarToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.editarToolStripMenuItem.Name = "editarToolStripMenuItem";
            this.editarToolStripMenuItem.Size = new System.Drawing.Size(69, 20);
            this.editarToolStripMenuItem.Text = "Opciones";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(169, 22);
            this.toolStripMenuItem1.Text = "Test Geodesic Dist";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // ayudaToolStripMenuItem
            // 
            this.ayudaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ayudaToolStripMenuItem1,
            this.acercaDeToolStripMenuItem});
            this.ayudaToolStripMenuItem.Name = "ayudaToolStripMenuItem";
            this.ayudaToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.ayudaToolStripMenuItem.Text = "Ayuda";
            // 
            // ayudaToolStripMenuItem1
            // 
            this.ayudaToolStripMenuItem1.Name = "ayudaToolStripMenuItem1";
            this.ayudaToolStripMenuItem1.Size = new System.Drawing.Size(126, 22);
            this.ayudaToolStripMenuItem1.Text = "Ayuda";
            // 
            // acercaDeToolStripMenuItem
            // 
            this.acercaDeToolStripMenuItem.Name = "acercaDeToolStripMenuItem";
            this.acercaDeToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
            this.acercaDeToolStripMenuItem.Text = "Acerca de";
            // 
            // chBoxShowAxes
            // 
            this.chBoxShowAxes.AutoSize = true;
            this.chBoxShowAxes.Location = new System.Drawing.Point(684, 108);
            this.chBoxShowAxes.Name = "chBoxShowAxes";
            this.chBoxShowAxes.Size = new System.Drawing.Size(74, 17);
            this.chBoxShowAxes.TabIndex = 10;
            this.chBoxShowAxes.Text = "Show axis";
            this.chBoxShowAxes.UseVisualStyleBackColor = true;
            this.chBoxShowAxes.CheckedChanged += new System.EventHandler(this.chBoxShowAxes_CheckedChanged);
            // 
            // btn_segment
            // 
            this.btn_segment.Location = new System.Drawing.Point(684, 220);
            this.btn_segment.Name = "btn_segment";
            this.btn_segment.Size = new System.Drawing.Size(96, 33);
            this.btn_segment.TabIndex = 11;
            this.btn_segment.Text = "Segmentar";
            this.btn_segment.UseVisualStyleBackColor = true;
            this.btn_segment.Click += new System.EventHandler(this.btn_segment_Click);
            // 
            // lblZoom
            // 
            this.lblZoom.AutoSize = true;
            this.lblZoom.Location = new System.Drawing.Point(395, 625);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(34, 13);
            this.lblZoom.TabIndex = 12;
            this.lblZoom.Text = "Zoom";
            // 
            // scrollZoom
            // 
            this.scrollZoom.AllowDrop = true;
            this.scrollZoom.Location = new System.Drawing.Point(389, 648);
            this.scrollZoom.Maximum = 10;
            this.scrollZoom.Minimum = -10;
            this.scrollZoom.Name = "scrollZoom";
            this.scrollZoom.Size = new System.Drawing.Size(480, 15);
            this.scrollZoom.TabIndex = 13;
            this.scrollZoom.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollZoom_Scroll);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(611, 542);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(67, 30);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(684, 542);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(67, 30);
            this.panel2.TabIndex = 15;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(757, 542);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(67, 30);
            this.panel3.TabIndex = 15;
            // 
            // lblFar
            // 
            this.lblFar.AutoSize = true;
            this.lblFar.Location = new System.Drawing.Point(780, 575);
            this.lblFar.Name = "lblFar";
            this.lblFar.Size = new System.Drawing.Size(0, 13);
            this.lblFar.TabIndex = 16;
            // 
            // lblMid
            // 
            this.lblMid.AutoSize = true;
            this.lblMid.Location = new System.Drawing.Point(725, 331);
            this.lblMid.Name = "lblMid";
            this.lblMid.Size = new System.Drawing.Size(0, 13);
            this.lblMid.TabIndex = 17;
            // 
            // lblNear
            // 
            this.lblNear.AutoSize = true;
            this.lblNear.Location = new System.Drawing.Point(681, 575);
            this.lblNear.Name = "lblNear";
            this.lblNear.Size = new System.Drawing.Size(0, 13);
            this.lblNear.TabIndex = 18;
            // 
            // chkFarest
            // 
            this.chkFarest.AutoSize = true;
            this.chkFarest.Location = new System.Drawing.Point(684, 131);
            this.chkFarest.Name = "chkFarest";
            this.chkFarest.Size = new System.Drawing.Size(111, 17);
            this.chkFarest.TabIndex = 19;
            this.chkFarest.Text = "Show farest faces";
            this.chkFarest.UseVisualStyleBackColor = true;
            this.chkFarest.CheckedChanged += new System.EventHandler(this.chkFarest_CheckedChanged);
            // 
            // btnCeldas
            // 
            this.btnCeldas.Location = new System.Drawing.Point(684, 262);
            this.btnCeldas.Name = "btnCeldas";
            this.btnCeldas.Size = new System.Drawing.Size(96, 33);
            this.btnCeldas.TabIndex = 20;
            this.btnCeldas.Text = "Celdas Voronoi";
            this.btnCeldas.UseVisualStyleBackColor = true;
            this.btnCeldas.Click += new System.EventHandler(this.btnCeldas_Click);
            // 
            // chbJustDistances
            // 
            this.chbJustDistances.AutoSize = true;
            this.chbJustDistances.Location = new System.Drawing.Point(684, 154);
            this.chbJustDistances.Name = "chbJustDistances";
            this.chbJustDistances.Size = new System.Drawing.Size(142, 17);
            this.chbJustDistances.TabIndex = 21;
            this.chbJustDistances.Text = "Use just distances matrix";
            this.chbJustDistances.UseVisualStyleBackColor = true;
            this.chbJustDistances.CheckedChanged += new System.EventHandler(this.chbJustDistances_CheckedChanged);
            // 
            // Board
            // 
            this.Board.BackColor = System.Drawing.Color.Black;
            this.Board.Location = new System.Drawing.Point(40, 102);
            this.Board.Name = "Board";
            this.Board.Size = new System.Drawing.Size(478, 475);
            this.Board.TabIndex = 22;
            this.Board.VSync = false;
            this.Board.Load += new System.EventHandler(this.Board_Load);
            this.Board.Paint += new System.Windows.Forms.PaintEventHandler(this.Board_Paint);
            this.Board.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Board_KeyDown);
            this.Board.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Board_MouseDown);
            this.Board.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Board_MouseMove);
            this.Board.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Board_MouseUp);
            this.Board.Resize += new System.EventHandler(this.Board_Resize);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Font = new System.Drawing.Font("Oswald", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.Location = new System.Drawing.Point(45, 638);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(0, 28);
            this.lblInfo.TabIndex = 23;
            // 
            // chkLines
            // 
            this.chkLines.AutoSize = true;
            this.chkLines.Checked = true;
            this.chkLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkLines.Location = new System.Drawing.Point(684, 177);
            this.chkLines.Name = "chkLines";
            this.chkLines.Size = new System.Drawing.Size(77, 17);
            this.chkLines.TabIndex = 24;
            this.chkLines.Text = "Show lines";
            this.chkLines.UseVisualStyleBackColor = true;
            this.chkLines.CheckedChanged += new System.EventHandler(this.chkLines_CheckedChanged);
            // 
            // visibilityBtn
            // 
            this.visibilityBtn.Location = new System.Drawing.Point(684, 301);
            this.visibilityBtn.Name = "visibilityBtn";
            this.visibilityBtn.Size = new System.Drawing.Size(96, 33);
            this.visibilityBtn.TabIndex = 25;
            this.visibilityBtn.Text = "Show Visibility";
            this.visibilityBtn.UseVisualStyleBackColor = true;
            this.visibilityBtn.Click += new System.EventHandler(this.visibilityBtn_Click);
            // 
            // btnBbox
            // 
            this.btnBbox.Location = new System.Drawing.Point(684, 340);
            this.btnBbox.Name = "btnBbox";
            this.btnBbox.Size = new System.Drawing.Size(96, 33);
            this.btnBbox.TabIndex = 26;
            this.btnBbox.Text = "Show BBoxes";
            this.btnBbox.UseVisualStyleBackColor = true;
            this.btnBbox.Click += new System.EventHandler(this.btnBbox_Click);
            // 
            // lblK
            // 
            this.lblK.AutoSize = true;
            this.lblK.Font = new System.Drawing.Font("Oswald", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblK.Location = new System.Drawing.Point(231, 638);
            this.lblK.Name = "lblK";
            this.lblK.Size = new System.Drawing.Size(0, 28);
            this.lblK.TabIndex = 27;
            // 
            // button4
            // 
            this.button4.Image = global::TMesh_2._0.Properties.Resources.ZoomOut;
            this.button4.Location = new System.Drawing.Point(213, 58);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 37);
            this.button4.TabIndex = 5;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.ZoomOut);
            // 
            // button3
            // 
            this.button3.Image = global::TMesh_2._0.Properties.Resources.ZoomIn;
            this.button3.Location = new System.Drawing.Point(167, 58);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(40, 38);
            this.button3.TabIndex = 4;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ZoomIn);
            // 
            // button2
            // 
            this.button2.Image = global::TMesh_2._0.Properties.Resources.gift;
            this.button2.Location = new System.Drawing.Point(85, 58);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 38);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Image = global::TMesh_2._0.Properties.Resources.Save;
            this.button1.Location = new System.Drawing.Point(40, 58);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 38);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnCmp
            // 
            this.btnCmp.Location = new System.Drawing.Point(684, 379);
            this.btnCmp.Name = "btnCmp";
            this.btnCmp.Size = new System.Drawing.Size(96, 33);
            this.btnCmp.TabIndex = 28;
            this.btnCmp.Text = "Compare";
            this.btnCmp.UseVisualStyleBackColor = true;
            this.btnCmp.Click += new System.EventHandler(this.btnCmp_Click);
            // 
            // lblSim
            // 
            this.lblSim.AutoSize = true;
            this.lblSim.Location = new System.Drawing.Point(646, 430);
            this.lblSim.Name = "lblSim";
            this.lblSim.Size = new System.Drawing.Size(35, 13);
            this.lblSim.TabIndex = 29;
            this.lblSim.Text = "label1";
            this.lblSim.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(891, 733);
            this.Controls.Add(this.lblSim);
            this.Controls.Add(this.btnCmp);
            this.Controls.Add(this.lblK);
            this.Controls.Add(this.btnBbox);
            this.Controls.Add(this.visibilityBtn);
            this.Controls.Add(this.chkLines);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.Board);
            this.Controls.Add(this.chbJustDistances);
            this.Controls.Add(this.btnCeldas);
            this.Controls.Add(this.chkFarest);
            this.Controls.Add(this.lblNear);
            this.Controls.Add(this.lblMid);
            this.Controls.Add(this.lblFar);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.scrollZoom);
            this.Controls.Add(this.lblZoom);
            this.Controls.Add(this.btn_segment);
            this.Controls.Add(this.chBoxShowAxes);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TMesh 2.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem abrirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ayudaToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem acercaDeToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox chBoxShowAxes;
        private System.Windows.Forms.Button btn_segment;
        private System.Windows.Forms.Label lblZoom;
        private System.Windows.Forms.HScrollBar scrollZoom;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblFar;
        private System.Windows.Forms.Label lblMid;
        private System.Windows.Forms.Label lblNear;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem distanciaToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox distanceSelector;
        private System.Windows.Forms.CheckBox chkFarest;
        private System.Windows.Forms.Button btnCeldas;
        private System.Windows.Forms.CheckBox chbJustDistances;
        private OpenTK.GLControl Board;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.CheckBox chkLines;
        private System.Windows.Forms.Button visibilityBtn;
        private System.Windows.Forms.Button btnBbox;
        private System.Windows.Forms.Label lblK;
        private System.Windows.Forms.Button btnCmp;
        private System.Windows.Forms.Label lblSim;
    }
}

