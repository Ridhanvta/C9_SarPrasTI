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

                    // --- QUERY 1: UNTUK HALAMAN 1 (REKAP GLOBAL) ---
                    string querySummary = @"
                SELECT 
                    b.namaBarang AS [NamaBarang],
                    b.stok AS [Stok],
                    CASE WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai' ELSE 'Aset Tetap' END AS [JenisBarang],
                    b.satuan AS [Satuan]
                FROM [master].[barang] b";

                    // --- QUERY 2: UNTUK HALAMAN 2 (DETAIL ASET, KONDISI TERBARU & SPESIFIKASI) ---
                    string queryDetail = @"
                SELECT 
                    b.namaBarang AS [NamaBarang],
                    r.namaRuangan AS [Ruangan],
                    CASE 
                        WHEN m_latest.kondisi = 1 THEN 'Baik'
                        WHEN m_latest.kondisi = 0 THEN 'Rusak'
                        ELSE 'Belum Dicek'
                    END AS [Kondisi],
                    CASE 
                        WHEN db.spesifikasi IS NOT NULL THEN db.spesifikasi 
                        ELSE '-' 
                    END AS [Spesifikasi],
                    COUNT(db.idDetailBarang) AS [Jumlah]
                FROM [master].[barang] b
                INNER JOIN [transaction].[detailBarang] db ON b.idBarang = db.idBarang
                INNER JOIN [master].[ruangan] r ON db.idRuangan = r.idRuangan
                LEFT JOIN (
                    SELECT idDetailBarang, kondisi
                    FROM (
                        SELECT idDetailBarang, kondisi, 
                               ROW_NUMBER() OVER(PARTITION BY idDetailBarang ORDER BY tglCek DESC, idMaintenance DESC) as rn
                        FROM [transaction].[maintenance]
                    ) tmp WHERE rn = 1
                ) m_latest ON db.idDetailBarang = m_latest.idDetailBarang
                WHERE b.tipeBarang = 1 ";

                    if (idSemesterFilter.HasValue)
                    {
                        string filterSem = @" 
                    AND EXISTS (
                        SELECT 1 FROM [transaction].[permintaanBarang] p 
                        WHERE p.idBarang = b.idBarang AND p.idSemester = @idSemester
                    )";
                        querySummary += filterSem;
                        queryDetail += filterSem;
                    }

                    // Spesifikasi wajib masuk ke GROUP BY
                    queryDetail += " GROUP BY b.namaBarang, r.namaRuangan, m_latest.kondisi, db.spesifikasi";

                    List<ReportSummary> listSummary = new List<ReportSummary>();
                    List<ReportDetail> listDetail = new List<ReportDetail>();

                    using (SqlCommand cmd = new SqlCommand(querySummary, conn))
                    {
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

                    using (SqlCommand cmd = new SqlCommand(queryDetail, conn))
                    {
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