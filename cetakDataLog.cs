using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // Tarik data dari VIEW yang udah dibikin di database
                    string query = "SELECT * FROM [dbo].[vwEvaluasiHabisPakai]";
                    List<ReportEvaluasiHabisPakai> listEvaluasi = new List<ReportEvaluasiHabisPakai>();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                listEvaluasi.Add(new ReportEvaluasiHabisPakai
                                {
                                    NamaBarang = reader["NamaBarang"].ToString(),
                                    TotalKeluar = Convert.ToInt32(reader["TotalKeluar"]),
                                    FrekuensiDiminta = Convert.ToInt32(reader["FrekuensiDiminta"]),
                                    FrekuensiRestock = Convert.ToInt32(reader["FrekuensiRestock"]),
                                    RataRataJmlRestock = Convert.ToInt32(reader["RataRataJmlRestock"]),
                                    StatusEvaluasi = reader["StatusEvaluasi"].ToString()
                                });
                            }
                        }
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nampilin evaluasi: " + ex.Message);
            }
        }
    } 
} 