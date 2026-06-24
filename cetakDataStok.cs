using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using SatprasDesktopApp.Config;

namespace ManajemenSarPras
{
    public partial class cetakDataStok : Form
    {
        private int? idSemesterFilter;
        private rptStok laporanStok;

        public cetakDataStok(int? filterSemester)
        {
            InitializeComponent();

            laporanStok = new rptStok();
            idSemesterFilter = filterSemester;

            LoadReportData();
        }

        private void LoadReportData()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    List<ReportSummary> listSummary = new List<ReportSummary>();
                    List<ReportDetail> listDetail = new List<ReportDetail>();

                    using (SqlCommand cmd = new SqlCommand("[dbo].[sp_CetakStokSummary]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (idSemesterFilter.HasValue) cmd.Parameters.AddWithValue("@idSemester", idSemesterFilter.Value);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listSummary.Add(new ReportSummary
                                {
                                    NamaBarang = reader["NamaBarang"].ToString(),
                                    Stok = Convert.ToInt32(reader["Stok"]),
                                    JenisBarang = reader["JenisBarang"].ToString(),
                                    Satuan = reader["Satuan"].ToString()
                                });
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand("[dbo].[sp_CetakStokDetail]", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (idSemesterFilter.HasValue) cmd.Parameters.AddWithValue("@idSemester", idSemesterFilter.Value);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listDetail.Add(new ReportDetail
                                {
                                    NamaBarang = reader["NamaBarang"].ToString(),
                                    Ruangan = reader["Ruangan"].ToString(),
                                    Kondisi = reader["Kondisi"].ToString(),

                                    // INI BARIS YANG KEMARIN KETINGGALAN
                                    Spesifikasi = reader["Spesifikasi"].ToString(),

                                    Jumlah = Convert.ToInt32(reader["Jumlah"])
                                });
                            }
                        }
                    }

                    if (listSummary.Count == 0)
                    {
                        MessageBox.Show("Nggak ada data yang bisa dicetak.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        return;
                    }

                    try
                    {
                        //laporanStok.Subreports[1].SetDataSource(listSummary);
                        //laporanStok.Subreports[0].SetDataSource(listDetail);
                        laporanStok.Subreports["rptLog.rpt - 01"].SetDataSource(listSummary);
                        laporanStok.Subreports["rptLog.rpt"].SetDataSource(listDetail);

                    }
                    catch
                    {
                        MessageBox.Show("Gagal mengatur sumber data laporan. Pastikan subreport sudah dibuat dengan benar.", "Error Subreport", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    laporanStok.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                    crystalReportViewer1.ReportSource = laporanStok;
                    crystalReportViewer1.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal mengambil data laporan: " + ex.Message, "Error Cetak", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void reportStok1_Load(object sender, EventArgs e)
        {

        }
    }
}