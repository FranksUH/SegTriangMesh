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
            this.chkRebuild = new System.Windows.Forms.CheckBox();
            this.chkgeodesicTest = new System.Windows.Forms.CheckBox();
            this.chkRandomSelect = new System.Windows.Forms.CheckBox();
            this.chkFarest = new System.Windows.Forms.CheckBox();
            this.chkRand = new System.Windows.Forms.CheckBox();
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
            this.groupBox1.Controls.Add(this.chkRebuild);
            this.groupBox1.Controls.Add(this.chkgeodesicTest);
            this.groupBox1.Controls.Add(this.chkRandomSelect);
            this.groupBox1.Controls.Add(this.chkFarest);
            this.groupBox1.Controls.Add(this.chkRand);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblGroups);
            this.groupBox1.Controls.Add(this.lblIterations);
            this.groupBox1.Controls.Add(this.nupGroups);
            this.groupBox1.Controls.Add(this.nudIterations);
            this.groupBox1.Controls.Add(this.btnSegment);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 257);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // chkRebuild
            // 
            this.chkRebuild.AutoSize = true;
            this.chkRebuild.Location = new System.Drawing.Point(119, 194);
            this.chkRebuild.Name = "chkRebuild";
            this.chkRebuild.Size = new System.Drawing.Size(125, 17);
            this.chkRebuild.TabIndex = 14;
            this.chkRebuild.Text = "Rebuild affinity matrix";
            this.chkRebuild.UseVisualStyleBackColor = true;
            this.chkRebuild.CheckedChanged += new System.EventHandler(this.ChkRebuild_CheckedChanged);
            // 
            // chkgeodesicTest
            // 
            this.chkgeodesicTest.AutoSize = true;
            this.chkgeodesicTest.Checked = true;
            this.chkgeodesicTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkgeodesicTest.Location = new System.Drawing.Point(194, 163);
            this.chkgeodesicTest.Name = "chkgeodesicTest";
            this.chkgeodesicTest.Size = new System.Drawing.Size(115, 17);
            this.chkgeodesicTest.TabIndex = 13;
            this.chkgeodesicTest.Text = "Test with geodesic";
            this.chkgeodesicTest.UseVisualStyleBackColor = true;
            this.chkgeodesicTest.CheckedChanged += new System.EventHandler(this.ChkgeodesicTest_CheckedChanged);
            // 
            // chkRandomSelect
            // 
            this.chkRandomSelect.AutoSize = true;
            this.chkRandomSelect.Location = new System.Drawing.Point(51, 163);
            this.chkRandomSelect.Name = "chkRandomSelect";
            this.chkRandomSelect.Size = new System.Drawing.Size(117, 17);
            this.chkRandomSelect.TabIndex = 12;
            this.chkRandomSelect.Text = "Random select test";
            this.chkRandomSelect.UseVisualStyleBackColor = true;
            this.chkRandomSelect.CheckedChanged += new System.EventHandler(this.ChkRandomSelect_CheckedChanged);
            // 
            // chkFarest
            // 
            this.chkFarest.AutoSize = true;
            this.chkFarest.Checked = true;
            this.chkFarest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFarest.Location = new System.Drawing.Point(194, 140);
            this.chkFarest.Name = "chkFarest";
            this.chkFarest.Size = new System.Drawing.Size(81, 17);
            this.chkFarest.TabIndex = 11;
            this.chkFarest.Text = "Farest seed";
            this.chkFarest.UseVisualStyleBackColor = true;
            this.chkFarest.CheckedChanged += new System.EventHandler(this.ChkFarest_CheckedChanged);
            // 
            // chkRand
            // 
            this.chkRand.AutoSize = true;
            this.chkRand.Location = new System.Drawing.Point(51, 140);
            this.chkRand.Name = "chkRand";
            this.chkRand.Size = new System.Drawing.Size(92, 17);
            this.chkRand.TabIndex = 10;
            this.chkRand.Text = "Random seed";
            this.chkRand.UseVisualStyleBackColor = true;
            this.chkRand.CheckedChanged += new System.EventHandler(this.ChkRand_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(200, 100);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 20);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "Auto";
            this.textBox1.TextChanged += new System.EventHandler(this.k_selector_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Piece length";
            // 
            // lblGroups
            // 
            this.lblGroups.AutoSize = true;
            this.lblGroups.Location = new System.Drawing.Point(52, 65);
            this.lblGroups.Name = "lblGroups";
            this.lblGroups.Size = new System.Drawing.Size(91, 13);
            this.lblGroups.TabIndex = 4;
            this.lblGroups.Text = "Number of groups";
            // 
            // lblIterations
            // 
            this.lblIterations.AutoSize = true;
            this.lblIterations.Location = new System.Drawing.Point(52, 25);
            this.lblIterations.Name = "lblIterations";
            this.lblIterations.Size = new System.Drawing.Size(101, 13);
            this.lblIterations.TabIndex = 3;
            this.lblIterations.Text = "Number of iterations";
            // 
            // nupGroups
            // 
            this.nupGroups.Location = new System.Drawing.Point(200, 63);
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
            this.nudIterations.Location = new System.Drawing.Point(200, 23);
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
            this.btnSegment.Location = new System.Drawing.Point(103, 217);
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
            this.ClientSize = new System.Drawing.Size(361, 282);
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
        private System.Windows.Forms.CheckBox chkFarest;
        private System.Windows.Forms.CheckBox chkRand;
        private System.Windows.Forms.CheckBox chkgeodesicTest;
        private System.Windows.Forms.CheckBox chkRandomSelect;
        private System.Windows.Forms.CheckBox chkRebuild;
    }
}