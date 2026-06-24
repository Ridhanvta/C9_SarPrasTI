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
            // Validasi menggunakan SelectedValue dari data-bound combobox
            if (cmbTahunAjaran.SelectedValue == null || cmbTahunAjaran.SelectedIndex == -1)
            {
                MessageBox.Show("Mohon untuk memilih Tahun Ajaran terlebih dahulu", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Ambil ID Semester (ValueMember) buat filter query, dan Teks (DisplayMember) buat judul chart
            int idSemesterTerpilih = Convert.ToInt32(cmbTahunAjaran.SelectedValue);
            string namaSemesterTerpilih = cmbTahunAjaran.Text;

            // Panggil fungsi chart dengan melempar ID-nya
            loadDataChart(idSemesterTerpilih, namaSemesterTerpilih);
        }

        private void LoadComboTahunAjaran()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // Ambil id dan teksnya dari tabel master semester
                    string query = "SELECT idSemester, tahunAjaran FROM [master].[semester]";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            cmbTahunAjaran.DataSource = dt;
                            cmbTahunAjaran.DisplayMember = "tahunAjaran"; // Kolom teks yang tampil
                            cmbTahunAjaran.ValueMember = "idSemester";    // Kolom ID asli di database
                            cmbTahunAjaran.DropDownStyle = ComboBoxStyle.DropDownList;
                            cmbTahunAjaran.SelectedIndex = -1; // Set default kosong di awal
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Load Tahun Ajaran", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Kita ubah parameternya menerima ID (int) dan nama text buat judul
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

                    // Query nembak b.stok (Stok Aktual) dengan filter p.idSemester (Lebih akurat pake int)
                    string query = @"
                        SELECT DISTINCT 
                            b.namaBarang, 
                            b.stok AS StokAktual
                        FROM [transaction].[permintaanBarang] p
                        INNER JOIN [master].[barang] b ON p.idBarang = b.idBarang
                        WHERE p.idSemester = @idSemester AND b.tipeBarang = 0";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idSemester", idSemesterFilter);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string namaBarang = reader["namaBarang"].ToString();
                                int totalStok = Convert.ToInt32(reader["StokAktual"]);

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
            // Panggil fungsi buatan kita yang sudah terstandarisasi DatabaseConfig
            LoadComboTahunAjaran();
        }
    }
}