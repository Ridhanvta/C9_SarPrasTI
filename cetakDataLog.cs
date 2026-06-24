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

namespace ManajemenSarPras
{
    

    public partial class cetakDataLog : Form
    {
        public cetakDataLog()
        {
            InitializeComponent();
            LoadReportEvaluasi();
        }

        private void LoadReportEvaluasi()
        {
            try
            {
                DataTable dt = DAL.GetReportEvaluasiData(null);
                List<ReportEvaluasiHabisPakai> listEvaluasi = new List<ReportEvaluasiHabisPakai>();

                foreach (DataRow row in dt.Rows)
                {
                    listEvaluasi.Add(new ReportEvaluasiHabisPakai
                    {
                        NamaBarang = row["NamaBarang"].ToString(),
                        TotalKeluar = Convert.ToInt32(row["TotalKeluar"]),
                        FrekuensiDiminta = Convert.ToInt32(row["jumlah diminta"]),
                        FrekuensiRestock = Convert.ToInt32(row["jumlah restock"]),
                        RataRataJmlRestock = Convert.ToInt32(row["RataRataJmlRestock"]),
                        StatusEvaluasi = row["analisa"].ToString()
                    });
                }

                if (listEvaluasi.Count == 0)
                {
                    MessageBox.Show("Belum ada data evaluasi yang bisa dicetak.");
                    return;
                }

                // Panggil file rptLog punya lo
                rptLog laporan = new rptLog();
                laporan.SetDataSource(listEvaluasi);

                // Bikin Landscape biar kolomnya muat
                laporan.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;

                // Tembak ke Viewer di layar form
                crvLog.ReportSource = laporan;
                crvLog.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nampilin evaluasi: " + ex.Message);
            }
        }
    } 
} 