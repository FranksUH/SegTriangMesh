namespace TMesh_2._0
{
    partial class SegmentationSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SegmentationSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblGroups = new System.Windows.Forms.Label();
            this.lblIterations = new System.Windows.Forms.Label();
            this.nupGroups = new System.Windows.Forms.NumericUpDown();
            this.nudIterations = new System.Windows.Forms.NumericUpDown();
            this.btnSegment = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupGroups)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIterations)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblGroups);
            this.groupBox1.Controls.Add(this.lblIterations);
            this.groupBox1.Controls.Add(this.nupGroups);
            this.groupBox1.Controls.Add(this.nudIterations);
            this.groupBox1.Controls.Add(this.btnSegment);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 230);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(202, 127);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 20);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "Auto";
            this.textBox1.TextChanged += new System.EventHandler(this.k_selector_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Piece length";
            // 
            // lblGroups
            // 
            this.lblGroups.AutoSize = true;
            this.lblGroups.Location = new System.Drawing.Point(54, 92);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Size = new System.Drawing.Size(91, 13);
            this.lblGroups.TabIndex = 4;
            this.lblGroups.Text = "Number of groups";
            // 
            // lblIterations
            // 
            this.lblIterations.AutoSize = true;
            this.lblIterations.Location = new System.Drawing.Point(54, 52);
            this.lblIterations.Name = "lblIterations";
            this.lblIterations.Size = new System.Drawing.Size(101, 13);
            this.lblIterations.TabIndex = 3;
            this.lblIterations.Text = "Number of iterations";
            // 
            // nupGroups
            // 
            this.nupGroups.Location = new System.Drawing.Point(202, 90);
            this.nupGroups.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nupGroups.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupGroups.Name = "nupGroups";
            this.nupGroups.Size = new System.Drawing.Size(57, 20);
            this.nupGroups.TabIndex = 2;
            this.nupGroups.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // nudIterations
            // 
            this.nudIterations.Location = new System.Drawing.Point(202, 50);
            this.nudIterations.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nudIterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudIterations.Name = "nudIterations";
            this.nudIterations.Size = new System.Drawing.Size(57, 20);
            this.nudIterations.TabIndex = 1;
            this.nudIterations.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // btnSegment
            // 
            this.btnSegment.Location = new System.Drawing.Point(103, 174);
            this.btnSegment.Name = "btnSegment";
            this.btnSegment.Size = new System.Drawing.Size(112, 34);
            this.btnSegment.TabIndex = 0;
            this.btnSegment.Text = "Segment";
            this.btnSegment.UseVisualStyleBackColor = true;
            this.btnSegment.Click += new System.EventHandler(this.btnSegment_Click);
            // 
            // SegmentationSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 255);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SegmentationSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SegmentationSettings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupGroups)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudIterations)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSegment;
        private System.Windows.Forms.NumericUpDown nupGroups;
        private System.Windows.Forms.NumericUpDown nudIterations;
        private System.Windows.Forms.Label lblGroups;
        private System.Windows.Forms.Label lblIterations;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
    }
}