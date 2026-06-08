using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using SatprasDesktopApp.Config; // Ini buat manggil DatabaseConfig kamu

namespace ManajemenSarPras
{
    public partial class dashboardPage : Form
    {
        public dashboardPage()
        {
            InitializeComponent();
        }

        // 1. Navigasi ke Kelola Barang
        private void kelolaBarang_Click(object sender, EventArgs e)
        {
            kelolaBarang formBarang = new kelolaBarang();
            formBarang.Show();
            this.Hide();
        }

        // 2. Navigasi ke Permintaan Barang
        private void permintaanBarang_Click(object sender, EventArgs e)
        {
            permintaanBarang formMinta = new permintaanBarang();
            formMinta.Show();
            this.Hide();
        }

        // 3. Navigasi ke Form Karyawan (Pengecekan Rutin)
        private void pengecekanRutin_Click(object sender, EventArgs e)
        {
            karyawan formKaryawan = new karyawan();
            formKaryawan.Show();
            this.Hide();
        }

        // 4. Navigasi ke Maintenance
        private void maintennce_Click(object sender, EventArgs e)
        {
            maintenancePage formMaintenance = new maintenancePage();
            formMaintenance.Show();
            this.Hide();
        }

        // 5. Navigasi ke Report
        private void report_Click(object sender, EventArgs e)
        {
            reportPage formReport = new reportPage();
            formReport.Show();
            this.Hide();
        }

        // 6. Navigasi ke Semester
        private void btnSemester_Click(object sender, EventArgs e)
        {
            formSemester frmSemester = new formSemester();
            frmSemester.Show();
            this.Hide();
        }

        // 7. Test Koneksi Database (Tombol Karyawan yang besar)
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        MessageBox.Show("Koneksi ke satprasDB Berhasil!", "Info Koneksi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Waduh, koneksi gagal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dashboardPage_Load(object sender, EventArgs e)
        {
            // Bisa dikosongkan atau buat narik data statistik ringkas
        }
    }
}