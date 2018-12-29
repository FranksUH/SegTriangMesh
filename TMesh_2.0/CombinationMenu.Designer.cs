namespace TMesh_2._0
{
    partial class CombinationMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombinationMenu));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnParmsBack = new System.Windows.Forms.Button();
            this.btnParametersReady = new System.Windows.Forms.Button();
            this.nupGeodesic = new System.Windows.Forms.NumericUpDown();
            this.nupAngular = new System.Windows.Forms.NumericUpDown();
            this.lblVolumetric = new System.Windows.Forms.Label();
            this.nupVolumetric = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupGeodesic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupAngular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupVolumetric)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblVolumetric);
            this.groupBox1.Controls.Add(this.nupVolumetric);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnParmsBack);
            this.groupBox1.Controls.Add(this.btnParametersReady);
            this.groupBox1.Controls.Add(this.nupGeodesic);
            this.groupBox1.Controls.Add(this.nupAngular);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(325, 204);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Set the parameters";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Geodesic";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(69, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Angular";
            // 
            // btnParmsBack
            // 
            this.btnParmsBack.Location = new System.Drawing.Point(72, 140);
            this.btnParmsBack.Name = "btnParmsBack";
            this.btnParmsBack.Size = new System.Drawing.Size(75, 23);
            this.btnParmsBack.TabIndex = 3;
            this.btnParmsBack.Text = "Back";
            this.btnParmsBack.UseVisualStyleBackColor = true;
            this.btnParmsBack.Click += new System.EventHandler(this.btnParmsBack_Click);
            // 
            // btnParametersReady
            // 
            this.btnParametersReady.Location = new System.Drawing.Point(174, 140);
            this.btnParametersReady.Name = "btnParametersReady";
            this.btnParametersReady.Size = new System.Drawing.Size(75, 23);
            this.btnParametersReady.TabIndex = 2;
            this.btnParametersReady.Text = "OK";
            this.btnParametersReady.UseVisualStyleBackColor = true;
            this.btnParametersReady.Click += new System.EventHandler(this.btnParametersReady_Click);
            // 
            // nupGeodesic
            // 
            this.nupGeodesic.DecimalPlaces = 2;
            this.nupGeodesic.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nupGeodesic.Location = new System.Drawing.Point(174, 64);
            this.nupGeodesic.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupGeodesic.Name = "nupGeodesic";
            this.nupGeodesic.Size = new System.Drawing.Size(75, 20);
            this.nupGeodesic.TabIndex = 1;
            this.nupGeodesic.ValueChanged += new System.EventHandler(this.nupGeodesic_ValueChanged);
            // 
            // nupAngular
            // 
            this.nupAngular.DecimalPlaces = 2;
            this.nupAngular.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nupAngular.Location = new System.Drawing.Point(174, 36);
            this.nupAngular.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupAngular.Name = "nupAngular";
            this.nupAngular.Size = new System.Drawing.Size(75, 20);
            this.nupAngular.TabIndex = 0;
            this.nupAngular.ValueChanged += new System.EventHandler(this.nupAngular_ValueChanged);
            // 
            // lblVolumetric
            // 
            this.lblVolumetric.AutoSize = true;
            this.lblVolumetric.Location = new System.Drawing.Point(69, 92);
            this.lblVolumetric.Name = "lblVolumetric";
            this.lblVolumetric.Size = new System.Drawing.Size(56, 13);
            this.lblVolumetric.TabIndex = 7;
            this.lblVolumetric.Text = "Volumetric";
            // 
            // nupVolumetric
            // 
            this.nupVolumetric.DecimalPlaces = 2;
            this.nupVolumetric.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nupVolumetric.Location = new System.Drawing.Point(174, 90);
            this.nupVolumetric.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nupVolumetric.Name = "nupVolumetric";
            this.nupVolumetric.Size = new System.Drawing.Size(75, 20);
            this.nupVolumetric.TabIndex = 6;
            // 
            // CombinationMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 229);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CombinationMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CombinationMenu";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nupGeodesic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupAngular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nupVolumetric)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnParmsBack;
        private System.Windows.Forms.Button btnParametersReady;
        private System.Windows.Forms.NumericUpDown nupGeodesic;
        private System.Windows.Forms.NumericUpDown nupAngular;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblVolumetric;
        private System.Windows.Forms.NumericUpDown nupVolumetric;
    }
}