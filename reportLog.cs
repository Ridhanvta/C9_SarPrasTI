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
    public class reportData
    {
        public string namaBarang { get; set; }
        public int stok { get; set; }
        public string jenisBarang { get; set; }
        public string satuan { get; set; }
    }
    public partial class reportLog: Form
    {
        public reportLog()
        {
            InitializeComponent();
        }

        

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {

        }

        private void btnCetak_Click(object sender, EventArgs e)
        {

        }
    }
}
