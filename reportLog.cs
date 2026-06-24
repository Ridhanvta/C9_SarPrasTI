using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SatprasDesktopApp.Config; // Pastikan namespace ini sesuai dengan project lo

namespace ManajemenSarPras
{
    public partial class reportLog : Form
    {
        public reportLog()
        {
            InitializeComponent();
            this.Load += new EventHandler(reportLog_Load);
        }

        private void reportLog_Load(object sender, EventArgs e)
        {
            LoadComboTahunAjaran();
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

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData(false);
        }

        private void btnCetak_Click(object sender, EventArgs e)
        {
            LoadData(true);
        }

        private void LoadData(bool isCetak)
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // Query tarik data dari VIEW evaluasi yang sudah kita buat
                    string query = @"
                        SELECT DISTINCT 
                            v.NamaBarang, 
                            v.TotalKeluar, 
                            v.FrekuensiDiminta AS [jumlah diminta], 
                            v.FrekuensiRestock AS [jumlah restock], 
                            v.RataRataJmlRestock, 
                            v.StatusEvaluasi AS [analisa]
                        FROM [dbo].[vwEvaluasiHabisPakai] v
                        INNER JOIN [master].[barang] b ON v.NamaBarang = b.namaBarang ";

                    bool isFiltered = (cmbTahunAjaran.SelectedValue != null && cmbTahunAjaran.SelectedIndex != -1);

                    if (isFiltered)
                    {
                        query += @"
                        INNER JOIN [transaction].[permintaanBarang] p ON b.idBarang = p.idBarang
                        WHERE p.idSemester = @idSemester";
                    }

                    List<ReportEvaluasiHabisPakai> listEvaluasi = new List<ReportEvaluasiHabisPakai>();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (isFiltered)
                        {
                            cmd.Parameters.AddWithValue("@idSemester", Convert.ToInt32(cmbTahunAjaran.SelectedValue));
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listEvaluasi.Add(new ReportEvaluasiHabisPakai
                                {
                                    NamaBarang = reader["NamaBarang"].ToString(),
                                    TotalKeluar = Convert.ToInt32(reader["TotalKeluar"]),
                                    FrekuensiDiminta = Convert.ToInt32(reader["jumlah diminta"]),
                                    FrekuensiRestock = Convert.ToInt32(reader["jumlah restock"]),
                                    RataRataJmlRestock = Convert.ToInt32(reader["RataRataJmlRestock"]),
                                    StatusEvaluasi = reader["analisa"].ToString()
                                });
                            }
                        }
                    }

                    if (listEvaluasi.Count == 0)
                    {
                        MessageBox.Show("Belum ada data evaluasi yang bisa dimuat untuk filter ini.");
                        return;
                    }

                    if (!isCetak)
                    {
                        // Tampilkan data di DataGridView saat tombol Load diklik
                        dgvReportLog.DataSource = listEvaluasi;
                        dgvReportLog.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                        // Sesuaikan header text DataGridView dengan alias yang diinginkan
                        if (dgvReportLog.Columns.Contains("NamaBarang"))
                            dgvReportLog.Columns["NamaBarang"].HeaderText = "Nama Barang";
                        
                        if (dgvReportLog.Columns.Contains("TotalKeluar"))
                            dgvReportLog.Columns["TotalKeluar"].HeaderText = "Total Keluar";
                        
                        if (dgvReportLog.Columns.Contains("FrekuensiDiminta"))
                            dgvReportLog.Columns["FrekuensiDiminta"].HeaderText = "Jumlah Diminta";
                        
                        if (dgvReportLog.Columns.Contains("FrekuensiRestock"))
                            dgvReportLog.Columns["FrekuensiRestock"].HeaderText = "Jumlah Restock";
                        
                        if (dgvReportLog.Columns.Contains("RataRataJmlRestock"))
                            dgvReportLog.Columns["RataRataJmlRestock"].HeaderText = "Rata-Rata Jml Restock";
                        
                        if (dgvReportLog.Columns.Contains("StatusEvaluasi"))
                            dgvReportLog.Columns["StatusEvaluasi"].HeaderText = "Analisa";
                    }
                    else
                    {
                        // Panggil file laporan Crystal Report lo saat tombol Cetak diklik
                        rptLog laporan = new rptLog();
                        laporan.SetDataSource(listEvaluasi);

                        // Setting orientasi kertas
                        laporan.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                        // Tampilkan ke Viewer menggunakan form cetakDataLog
                        cetakDataLog frmViewer = new cetakDataLog();
                        frmViewer.crvLog.ReportSource = laporan;
                        frmViewer.crvLog.Refresh();
                        frmViewer.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nampilin evaluasi: " + ex.Message);
            }
        }
    }
}