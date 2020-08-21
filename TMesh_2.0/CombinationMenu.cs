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
    public partial class CombinationMenu : Form
    {
        public double angular = 0;
        public double SDFcoef = 0;
        public double geodesic = 0;
        public CombinationMenu()
        {
            InitializeComponent();
            this.angular = 0;
            this.geodesic = 0;
        }

        private void btnParametersReady_Click(object sender, EventArgs e)
        {
            this.angular = (double)this.nupAngular.Value;
            this.geodesic = (double)this.nupGeodesic.Value;
            this.SDFcoef = (double)this.SDF_nup.Value;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnParmsBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void nupAngular_ValueChanged(object sender, EventArgs e)
        {
            nupVolumetric.Value = (decimal)(1 - nupAngular.Value - nupGeodesic.Value - SDF_nup.Value);
        }

        private void nupGeodesic_ValueChanged(object sender, EventArgs e)
        {
            nupVolumetric.Value = (decimal)(1 - nupAngular.Value - nupGeodesic.Value - SDF_nup.Value);
        }

        private void SDF_nup_ValueChanged(object sender, EventArgs e)
        {
            nupVolumetric.Value = (decimal)(1 - nupAngular.Value - nupGeodesic.Value - SDF_nup.Value);
        }
    }
}
