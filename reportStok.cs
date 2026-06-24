using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SatprasDesktopApp.Config;

// Pastikan lo udah nambahin library Crystal Reports di project lo
// using CrystalDecisions.CrystalReports.Engine;

namespace ManajemenSarPras
{
    public partial class reportStok : Form
    {
        public reportStok()
        {
            InitializeComponent();
        }

        private void reportStok_Load(object sender, EventArgs e)
        {
            // 1. Tarik pilihan semester ke combobox pas form dibuka
            LoadComboTahunAjaran();

            // 2. Tampilkan semua data barang di tabel sbg default
            LoadDataStok();
        }

        private void btnKembali_Click_1(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void dgvReportStok_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void LoadComboTahunAjaran()
        {
            try
            {
                DataTable dt = DAL.GetAllSemesters();
                cmbTahunAjaran.DataSource = dt;
                cmbTahunAjaran.DisplayMember = "tahunAjaran";
                cmbTahunAjaran.ValueMember = "idSemester";
                cmbTahunAjaran.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbTahunAjaran.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Load Tahun Ajaran", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Fungsi Load/Filter yang sempat kosong gue balikin lagi ya
        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cmbTahunAjaran.SelectedValue == null || cmbTahunAjaran.SelectedIndex == -1)
            {
                MessageBox.Show("Pilih Tahun Ajaran dulu, bro!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSemesterTerpilih = Convert.ToInt32(cmbTahunAjaran.SelectedValue);
            LoadDataStok(idSemesterTerpilih);
        }

        private void LoadDataStok(int? idSemesterFilter = null)
        {
            try
            {
                DataTable dt = DAL.GetReportStokData(idSemesterFilter);
                dgvReportStok.DataSource = dt;
                dgvReportStok.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvReportStok.ReadOnly = true;
                dgvReportStok.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nampilin data stok: " + ex.Message, "Error Load Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= FITUR CETAK RPT =================
        private void btnCetak_Click(object sender, EventArgs e)
        {
            try
            {
                // Ambil idSemester dari combobox jika ada filter yang dipilih
                int? idSemesterFilter = null;
                if (cmbTahunAjaran.SelectedValue != null && cmbTahunAjaran.SelectedIndex != -1)
                {
                    idSemesterFilter = Convert.ToInt32(cmbTahunAjaran.SelectedValue);
                }

                // Panggil form cetakDataStok yang sudah punya logika report Object Collection
                cetakDataStok frmViewer = new cetakDataStok(idSemesterFilter);
                frmViewer.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nyetak laporan: " + ex.Message, "Error Cetak", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}