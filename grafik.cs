using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.SqlClient;
using SatprasDesktopApp.Config;

namespace ManajemenSarPras
{
    public partial class grafik : Form
    {

        public grafik()
        {
            InitializeComponent();
        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            cmbTahunAjaran.SelectedIndex = -1;
            chartBarang.Series.Clear();
            chartBarang.Titles.Clear();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            if (cmbTahunAjaran.SelectedValue == null || cmbTahunAjaran.SelectedIndex == -1)
            {
                MessageBox.Show("Mohon untuk memilih Tahun Ajaran terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idSemesterTerpilih = Convert.ToInt32(cmbTahunAjaran.SelectedValue);
            string namaSemesterTerpilih = cmbTahunAjaran.Text;

            loadDataChart(idSemesterTerpilih, namaSemesterTerpilih);
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

        public void loadDataChart(int idSemesterFilter, string namaSemester)
        {
            chartBarang.Series.Clear();
            chartBarang.Titles.Clear();
            chartBarang.Legends.Clear();
            chartBarang.ChartAreas.Clear();

            ChartArea ca = new ChartArea("MainArea");
            ca.AxisX.Title = "Nama Barang";
            ca.AxisY.Title = "Jumlah barang yang tersedia saat ini";
            ca.AxisX.LabelStyle.Angle = -45;
            ca.AxisX.Interval = 1;
            ca.BackColor = Color.WhiteSmoke;
            chartBarang.ChartAreas.Add(ca);

            // Filter combo box tetap dipakai buat dinamisasi judul chart
            chartBarang.Titles.Add($"Grafik Stok Barang Aktual - Tahun Ajaran {namaSemester}");

            Series series = new Series("StokBarang");
            series.ChartType = SeriesChartType.Column;
            series.IsValueShownAsLabel = true;
            series.Color = Color.SteelBlue;
            chartBarang.Series.Add(series);

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // Kunci Solusi: Langsung tembak ke tabel master barang tanpa INNER JOIN
                    string query = @"
                SELECT 
                    namaBarang, 
                    stok AS StokAktual
                FROM [master].[barang]";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Kita nggak perlu masukin parameter @idSemester ke SQL lagi
                        // karena kita narik stok aktual global, bukan stok per transaksi
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string namaBarang = reader["namaBarang"].ToString();
                                int totalStok = Convert.ToInt32(reader["StokAktual"]);

                                // Masukin semua barang ke dalam chart
                                series.Points.AddXY(namaBarang, totalStok);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Waduh, ada error nih: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grafik_Load(object sender, EventArgs e)
        {
            LoadComboTahunAjaran();
        }
    }
}