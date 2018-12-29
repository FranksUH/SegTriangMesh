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
        public SegmentationSettings()
        {
            InitializeComponent();
            this.numGroups = 3;
            this.numIter = 3;
            this.K = 0;
        } 

        private void btnSegment_Click(object sender, EventArgs e)
        {
            this.numIter = int.Parse(this.nudIterations.Value.ToString());
            this.numGroups = int.Parse(this.nupGroups.Value.ToString());
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out K);
        }
    }
}
