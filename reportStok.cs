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


namespace ManajemenSarPras
{
    public partial class reportStok : Form
    {
        public reportStok()
        {
            InitializeComponent();
        }

        // Method ini akan dipanggil saat form pertama kali dibuka
        private void reportStok_Load(object sender, EventArgs e)
        {
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

        // Fungsi utama buat ngambil data dan masukin ke DataGridView
        private void LoadDataStok()
        {
            try
            {
                // Pakai class koneksi andalan lo
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // Query menggunakan CASE WHEN untuk konversi 0/1 jadi teks
                    // Ditambah LEFT JOIN ke tabel merk sebagai referensi join tabel
                    string query = @"
                        SELECT 
                            b.namaBarang AS [Nama Barang],
                            b.stok AS [Jumlah Saat Ini],
                            CASE 
                                WHEN b.tipeBarang = 0 THEN 'Barang Habis Pakai'
                                WHEN b.tipeBarang = 1 THEN 'Aset Tetap'
                                ELSE 'Tidak Diketahui'
                            END AS [Jenis Barang],
                            b.satuan AS [Satuan]
                        FROM [master].[barang] b
                        LEFT JOIN [master].[merk] m ON b.idMerk = m.idMerk";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            // Masukin data ke DataGridView
                            dgvReportStok.DataSource = dt;

                            // Bumbu tambahan biar tampilan tabelnya rapi secara otomatis
                            dgvReportStok.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dgvReportStok.ReadOnly = true; // Biar user gak bisa ngedit data sembarangan dari tabel
                            dgvReportStok.AllowUserToAddRows = false; // Ngilangin baris kosong di paling bawah
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal nampilin data stok: " + ex.Message, "Error Load Data", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}