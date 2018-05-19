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
            this.Board = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.abrirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ayudaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ayudaToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.acercaDeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.CampPosLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.camPosX = new System.Windows.Forms.NumericUpDown();
            this.camPosZ = new System.Windows.Forms.NumericUpDown();
            this.camPosY = new System.Windows.Forms.NumericUpDown();
            this.centerX = new System.Windows.Forms.NumericUpDown();
            this.centerZ = new System.Windows.Forms.NumericUpDown();
            this.centerY = new System.Windows.Forms.NumericUpDown();
            this.camUpX = new System.Windows.Forms.NumericUpDown();
            this.camUpZ = new System.Windows.Forms.NumericUpDown();
            this.camUpY = new System.Windows.Forms.NumericUpDown();
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
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.camPosX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camPosZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camPosY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camUpX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camUpZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.camUpY)).BeginInit();
            this.SuspendLayout();
            // 
            // Board
            // 
            this.Board.AccumBits = ((byte)(0));
            this.Board.AutoCheckErrors = false;
            this.Board.AutoFinish = false;
            this.Board.AutoMakeCurrent = true;
            this.Board.AutoSwapBuffers = true;
            this.Board.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.Board.ColorBits = ((byte)(32));
            this.Board.DepthBits = ((byte)(64));
            this.Board.Location = new System.Drawing.Point(12, 65);
            this.Board.Name = "Board";
            this.Board.Size = new System.Drawing.Size(591, 444);
            this.Board.StencilBits = ((byte)(0));
            this.Board.TabIndex = 0;
            this.Board.Scroll += new System.Windows.Forms.ScrollEventHandler(this.Board_Scroll);
            this.Board.Paint += new System.Windows.Forms.PaintEventHandler(this.PaintGL);
            this.Board.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Board_KeyDown);
            this.Board.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Board_MouseClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem,
            this.editarToolStripMenuItem,
            this.ayudaToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1020, 24);
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
            // editarToolStripMenuItem
            // 
            this.editarToolStripMenuItem.Name = "editarToolStripMenuItem";
            this.editarToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.editarToolStripMenuItem.Text = "Editar";
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
            // button3
            // 
            this.button3.Image = global::TMesh_2._0.Properties.Resources.ZoomIn;
            this.button3.Location = new System.Drawing.Point(298, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(40, 38);
            this.button3.TabIndex = 4;
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ZoomIn);
            // 
            // button4
            // 
            this.button4.Image = global::TMesh_2._0.Properties.Resources.ZoomOut;
            this.button4.Location = new System.Drawing.Point(344, 21);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 37);
            this.button4.TabIndex = 5;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.ZoomOut);
            // 
            // button2
            // 
            this.button2.Image = global::TMesh_2._0.Properties.Resources.gift;
            this.button2.Location = new System.Drawing.Point(216, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 38);
            this.button2.TabIndex = 3;
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Image = global::TMesh_2._0.Properties.Resources.Save;
            this.button1.Location = new System.Drawing.Point(171, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(39, 38);
            this.button1.TabIndex = 2;
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CampPosLabel
            // 
            this.CampPosLabel.AutoSize = true;
            this.CampPosLabel.Location = new System.Drawing.Point(632, 64);
            this.CampPosLabel.Name = "CampPosLabel";
            this.CampPosLabel.Size = new System.Drawing.Size(46, 13);
            this.CampPosLabel.TabIndex = 6;
            this.CampPosLabel.Text = "CamPos";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(632, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Center";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(632, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "CamUp";
            // 
            // camPosX
            // 
            this.camPosX.DecimalPlaces = 1;
            this.camPosX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camPosX.Location = new System.Drawing.Point(692, 62);
            this.camPosX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.camPosX.Name = "camPosX";
            this.camPosX.Size = new System.Drawing.Size(48, 20);
            this.camPosX.TabIndex = 9;
            this.camPosX.ValueChanged += new System.EventHandler(this.UpdateCamPosX);
            // 
            // camPosZ
            // 
            this.camPosZ.DecimalPlaces = 1;
            this.camPosZ.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camPosZ.Location = new System.Drawing.Point(800, 62);
            this.camPosZ.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.camPosZ.Name = "camPosZ";
            this.camPosZ.Size = new System.Drawing.Size(48, 20);
            this.camPosZ.TabIndex = 9;
            this.camPosZ.Value = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camPosZ.ValueChanged += new System.EventHandler(this.UpdateCamPosZ);
            // 
            // camPosY
            // 
            this.camPosY.DecimalPlaces = 1;
            this.camPosY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camPosY.Location = new System.Drawing.Point(746, 62);
            this.camPosY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.camPosY.Name = "camPosY";
            this.camPosY.Size = new System.Drawing.Size(48, 20);
            this.camPosY.TabIndex = 9;
            this.camPosY.ValueChanged += new System.EventHandler(this.UpdateCamPosY);
            // 
            // centerX
            // 
            this.centerX.DecimalPlaces = 1;
            this.centerX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.centerX.Location = new System.Drawing.Point(692, 101);
            this.centerX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.centerX.Name = "centerX";
            this.centerX.Size = new System.Drawing.Size(48, 20);
            this.centerX.TabIndex = 9;
            this.centerX.ValueChanged += new System.EventHandler(this.UpdateCenterX);
            // 
            // centerZ
            // 
            this.centerZ.DecimalPlaces = 1;
            this.centerZ.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.centerZ.Location = new System.Drawing.Point(800, 101);
            this.centerZ.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.centerZ.Name = "centerZ";
            this.centerZ.Size = new System.Drawing.Size(48, 20);
            this.centerZ.TabIndex = 9;
            this.centerZ.ValueChanged += new System.EventHandler(this.UpdateCenterZ);
            // 
            // centerY
            // 
            this.centerY.DecimalPlaces = 1;
            this.centerY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.centerY.Location = new System.Drawing.Point(746, 101);
            this.centerY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.centerY.Name = "centerY";
            this.centerY.Size = new System.Drawing.Size(48, 20);
            this.centerY.TabIndex = 9;
            this.centerY.ValueChanged += new System.EventHandler(this.UpdateCenterY);
            // 
            // camUpX
            // 
            this.camUpX.DecimalPlaces = 1;
            this.camUpX.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camUpX.Location = new System.Drawing.Point(692, 141);
            this.camUpX.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.camUpX.Name = "camUpX";
            this.camUpX.Size = new System.Drawing.Size(48, 20);
            this.camUpX.TabIndex = 9;
            this.camUpX.ValueChanged += new System.EventHandler(this.UpdateCamUpX);
            // 
            // camUpZ
            // 
            this.camUpZ.DecimalPlaces = 1;
            this.camUpZ.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camUpZ.Location = new System.Drawing.Point(800, 141);
            this.camUpZ.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.camUpZ.Name = "camUpZ";
            this.camUpZ.Size = new System.Drawing.Size(48, 20);
            this.camUpZ.TabIndex = 9;
            this.camUpZ.ValueChanged += new System.EventHandler(this.UpdateCamUpZ);
            // 
            // camUpY
            // 
            this.camUpY.DecimalPlaces = 1;
            this.camUpY.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.camUpY.Location = new System.Drawing.Point(746, 141);
            this.camUpY.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.camUpY.Name = "camUpY";
            this.camUpY.Size = new System.Drawing.Size(48, 20);
            this.camUpY.TabIndex = 9;
            this.camUpY.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.camUpY.ValueChanged += new System.EventHandler(this.UpdateCamUpY);
            // 
            // chBoxShowAxes
            // 
            this.chBoxShowAxes.AutoSize = true;
            this.chBoxShowAxes.Location = new System.Drawing.Point(673, 216);
            this.chBoxShowAxes.Name = "chBoxShowAxes";
            this.chBoxShowAxes.Size = new System.Drawing.Size(74, 17);
            this.chBoxShowAxes.TabIndex = 10;
            this.chBoxShowAxes.Text = "Show axis";
            this.chBoxShowAxes.UseVisualStyleBackColor = true;
            this.chBoxShowAxes.CheckedChanged += new System.EventHandler(this.chBoxShowAxes_CheckedChanged);
            // 
            // btn_segment
            // 
            this.btn_segment.Location = new System.Drawing.Point(673, 271);
            this.btn_segment.Name = "btn_segment";
            this.btn_segment.Size = new System.Drawing.Size(96, 36);
            this.btn_segment.TabIndex = 11;
            this.btn_segment.Text = "Segmentar";
            this.btn_segment.UseVisualStyleBackColor = true;
            this.btn_segment.Click += new System.EventHandler(this.btn_segment_Click);
            // 
            // lblZoom
            // 
            this.lblZoom.AutoSize = true;
            this.lblZoom.Location = new System.Drawing.Point(23, 535);
            this.lblZoom.Name = "lblZoom";
            this.lblZoom.Size = new System.Drawing.Size(34, 13);
            this.lblZoom.TabIndex = 12;
            this.lblZoom.Text = "Zoom";
            // 
            // scrollZoom
            // 
            this.scrollZoom.AllowDrop = true;
            this.scrollZoom.Location = new System.Drawing.Point(60, 535);
            this.scrollZoom.Maximum = 10;
            this.scrollZoom.Minimum = -10;
            this.scrollZoom.Name = "scrollZoom";
            this.scrollZoom.Size = new System.Drawing.Size(469, 18);
            this.scrollZoom.TabIndex = 13;
            this.scrollZoom.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollZoom_Scroll);
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(673, 475);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(67, 33);
            this.panel1.TabIndex = 14;
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(746, 475);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(67, 33);
            this.panel2.TabIndex = 15;
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(819, 475);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(67, 33);
            this.panel3.TabIndex = 15;
            // 
            // lblFar
            // 
            this.lblFar.AutoSize = true;
            this.lblFar.Location = new System.Drawing.Point(703, 456);
            this.lblFar.Name = "lblFar";
            this.lblFar.Size = new System.Drawing.Size(0, 13);
            this.lblFar.TabIndex = 16;
            // 
            // lblMid
            // 
            this.lblMid.AutoSize = true;
            this.lblMid.Location = new System.Drawing.Point(769, 456);
            this.lblMid.Name = "lblMid";
            this.lblMid.Size = new System.Drawing.Size(0, 13);
            this.lblMid.TabIndex = 17;
            // 
            // lblNear
            // 
            this.lblNear.AutoSize = true;
            this.lblNear.Location = new System.Drawing.Point(848, 456);
            this.lblNear.Name = "lblNear";
            this.lblNear.Size = new System.Drawing.Size(0, 13);
            this.lblNear.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 577);
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
            this.Controls.Add(this.camUpY);
            this.Controls.Add(this.centerY);
            this.Controls.Add(this.camPosY);
            this.Controls.Add(this.camUpZ);
            this.Controls.Add(this.centerZ);
            this.Controls.Add(this.camPosZ);
            this.Controls.Add(this.camUpX);
            this.Controls.Add(this.centerX);
            this.Controls.Add(this.camPosX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CampPosLabel);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Board);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "TMesh 2.0";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.camPosX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camPosZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camPosY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.centerY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camUpX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camUpZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.camUpY)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl Board;
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
        private System.Windows.Forms.Label CampPosLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown camPosX;
        private System.Windows.Forms.NumericUpDown camPosZ;
        private System.Windows.Forms.NumericUpDown camPosY;
        private System.Windows.Forms.NumericUpDown centerX;
        private System.Windows.Forms.NumericUpDown centerZ;
        private System.Windows.Forms.NumericUpDown centerY;
        private System.Windows.Forms.NumericUpDown camUpX;
        private System.Windows.Forms.NumericUpDown camUpZ;
        private System.Windows.Forms.NumericUpDown camUpY;
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
    }
}

