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
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = "SELECT idSemester, tahunAjaran FROM [master].[semester]";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbTahunAjaran.DataSource = dt;
                            cmbTahunAjaran.DisplayMember = "tahunAjaran";
                            cmbTahunAjaran.ValueMember = "idSemester";
                            cmbTahunAjaran.DropDownStyle = ComboBoxStyle.DropDownList;
                            cmbTahunAjaran.SelectedIndex = -1;
                        }
                    }
                }
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
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                        SELECT DISTINCT 
                            b.namaBarang AS [Nama Barang],
                            b.stok AS [Jumlah Saat Ini],
                            CASE 
                                WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai'
                                WHEN b.tipeBarang = 1 THEN 'Aset Tetap'
                                ELSE 'Tidak Diketahui'
                            END AS [Jenis Barang],
                            b.satuan AS [Satuan]
                        FROM [master].[barang] b
                        LEFT JOIN [master].[merk] m ON b.idMerk = m.idMerk ";

                    if (idSemesterFilter.HasValue)
                    {
                        query += @"
                        INNER JOIN [transaction].[permintaanBarang] p ON b.idBarang = p.idBarang
                        WHERE p.idSemester = @idSemester";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (idSemesterFilter.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@idSemester", idSemesterFilter.Value);
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvReportStok.DataSource = dt;
                            dgvReportStok.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dgvReportStok.ReadOnly = true;
                            dgvReportStok.AllowUserToAddRows = false;
                        }
                    }
                }
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
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // 1. Siapkan adonan query yang sama persis kayak di layar DGV
                    string query = @"
                        SELECT DISTINCT 
                            b.namaBarang AS [Nama Barang],
                            b.stok AS [Jumlah Saat Ini],
                            CASE 
                                WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai'
                                WHEN b.tipeBarang = 1 THEN 'Aset Tetap'
                                ELSE 'Tidak Diketahui'
                            END AS [Jenis Barang],
                            b.satuan AS [Satuan]
                        FROM [master].[barang] b
                        LEFT JOIN [master].[merk] m ON b.idMerk = m.idMerk ";

                    // Cek apakah user lagi nge-filter pakai combobox
                    bool isFiltered = (cmbTahunAjaran.SelectedValue != null && cmbTahunAjaran.SelectedIndex != -1);

                    if (isFiltered)
                    {
                        query += @"
                        INNER JOIN [transaction].[permintaanBarang] p ON b.idBarang = p.idBarang
                        WHERE p.idSemester = @idSemester";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Masukin id semester kalau lagi difilter
                        if (isFiltered)
                        {
                            cmd.Parameters.AddWithValue("@idSemester", Convert.ToInt32(cmbTahunAjaran.SelectedValue));
                        }

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dtCetak = new DataTable();
                            da.Fill(dtCetak);

                            // Validasi kalau datanya kosong, nggak usah diprint
                            if (dtCetak.Rows.Count == 0)
                            {
                                MessageBox.Show("Nggak ada data yang bisa dicetak bro!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            
                            // Ganti 'ReportStokBarang' dengan nama file .rpt lo
                            rptStok rptDoc = new rptStok(); 
                            rptDoc.SetDataSource(dtCetak);

                            // Ganti 'FormPrintViewer' dengan nama Form yang ada CrystalReportViewer-nya
                            //cetakDataStok frmViewer = new cetakDataStok();
                            //frmViewer.crystalReportViewer1.ReportSource = rptDoc;
                            //frmViewer.ShowDialog();
                            

                            MessageBox.Show("Sistem udah siap nyetak! Tinggal uncomment kode Crystal Report-nya.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nyetak laporan: " + ex.Message, "Error Cetak", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}