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
    public partial class TestMenu : Form
    {
        public double[] values;
        public TestMenu()
        {
            InitializeComponent();
            this.values = new double[12];
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            values[0] = double.Parse(p0x.Text);
            values[1] = double.Parse(p0y.Text);
            values[2] = double.Parse(p0z.Text);
            values[3] = double.Parse(p1x.Text);
            values[4] = double.Parse(p1y.Text);
            values[5] = double.Parse(p1z.Text);
            values[6] = double.Parse(p2x.Text);
            values[7] = double.Parse(p2y.Text);
            values[8] = double.Parse(p2z.Text);
            values[9] = double.Parse(p3x.Text);
            values[10] = double.Parse(p3y.Text);
            values[11] = double.Parse(p3z.Text);
            this.Close();
        }
    }
}
