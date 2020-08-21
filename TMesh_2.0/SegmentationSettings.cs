using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TMesh_2._0
{
    public partial class SegmentationSettings : Form
    {
        public int numIter { get; private set; }
        public int numGroups { get; private set; }
        public int K;
        public bool randomSeed { get; private set; }
        public bool randomTest { get; private set; }
        public bool useGeodesic { get; private set; }
        private bool applies;
        public bool rebuild;
        public SegmentationSettings(bool applies)
        {
            InitializeComponent();
            this.numGroups = 3;
            this.numIter = 3;
            this.applies = applies;
            this.K = 0;
        } 

        private void btnSegment_Click(object sender, EventArgs e)
        {
            this.numIter = int.Parse(this.nudIterations.Value.ToString());
            this.numGroups = int.Parse(this.nupGroups.Value.ToString());
            this.randomSeed = chkRand.Checked;
            this.randomTest = chkRandomSelect.Checked;
            this.useGeodesic = chkgeodesicTest.Checked;
            this.rebuild = chkRebuild.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void k_selector_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out K);
        }

        private void ChkRand_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRand.Checked)
            {
                chkFarest.Checked = false;
                randomSeed = true;
            }
            else
            {
                chkFarest.Checked = true;
                randomSeed = false;
            }
            if (!applies && !rebuild)
                MessageBox.Show("Warning: Your changes not will be applied, please reload the mesh");
        }

        private void ChkFarest_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFarest.Checked)
            {
                chkRand.Checked = false;
                randomSeed = false;
            }
            else
            {
                chkRand.Checked = true;
                randomSeed = true;
            }
            if (!applies && !rebuild)
                MessageBox.Show("Warning: Your changes not will be applied, please reload the mesh");
        }

        private void ChkgeodesicTest_CheckedChanged(object sender, EventArgs e)
        {
            if (!applies && !rebuild)
                MessageBox.Show("Warning: Your changes not will be applied, please reload the mesh");
        }

        private void ChkRandomSelect_CheckedChanged(object sender, EventArgs e)
        {
            if (!applies && !rebuild)
                MessageBox.Show("Warning: Your changes not will be applied, please reload the mesh");
        }

        private void ChkRebuild_CheckedChanged(object sender, EventArgs e)
        {
            this.rebuild = chkRebuild.Checked;
        }
    }
}
