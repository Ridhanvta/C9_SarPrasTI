using System;
using System.Collections.Generic;
using System.Data;
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
                List<ReportSummary> listSummary = new List<ReportSummary>();
                List<ReportDetail> listDetail = new List<ReportDetail>();

                DataTable dtSummary = DAL.GetCetakStokSummary(idSemesterFilter);
                foreach (DataRow row in dtSummary.Rows)
                {
                    listSummary.Add(new ReportSummary
                    {
                        NamaBarang = row["NamaBarang"].ToString(),
                        Stok = Convert.ToInt32(row["Stok"]),
                        JenisBarang = row["JenisBarang"].ToString(),
                        Satuan = row["Satuan"].ToString()
                    });
                }

                DataTable dtDetail = DAL.GetCetakStokDetail(idSemesterFilter);
                foreach (DataRow row in dtDetail.Rows)
                {
                    listDetail.Add(new ReportDetail
                    {
                        NamaBarang = row["NamaBarang"].ToString(),
                        Ruangan = row["Ruangan"].ToString(),
                        Kondisi = row["Kondisi"].ToString(),
                        Spesifikasi = row["Spesifikasi"].ToString(),
                        Jumlah = Convert.ToInt32(row["Jumlah"])
                    });
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