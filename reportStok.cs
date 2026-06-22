using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class reportStok: Form
    {
        public reportStok()
        {
            InitializeComponent();
        }

     

        private void btnKembali_Click_1(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }
    }
}
