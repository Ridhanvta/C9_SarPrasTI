using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class permintaanBarang : Form
    {
        // Variabel Kontrol State Form
        private string selectedIdPermintaan = "";
        private string masterSatuanBarang = "Pcs"; // Menyimpan satuan besar dari katalog (Rem/Box/Pack/Pcs)
        private int stokTersediaBarangPcs = 0;      // Menyimpan stok aktual dalam satuan terkecil (Pcs)
        private int isiKonversiBarang = 1;          // Menyimpan kapasitas isi 1 satuan besar (misal: Rem = 500)
        private bool isResetting = false;

        private BindingSource navBindingSource = new BindingSource();
        private DataTable dtTransaksi = new DataTable();

        public permintaanBarang()
        {
            InitializeComponent();

            // Setup Awal UI Komponen ID Barang (Read-Only & Terkunci)
            this.txtIdBarang.ReadOnly = true;
            this.txtIdBarang.BackColor = SystemColors.Control;

            // Setup Dropdown Satuan Agar Tidak Bisa Diketik Manual
            this.cmbSatuanRequest.DropDownStyle = ComboBoxStyle.DropDownList;

            // Mapping Event Handler
            this.Load += new EventHandler(permintaanBarang_Load);
            this.addPeminta.Click += new EventHandler(btnSimpan_Click);
            this.updatePminjam.Click += new EventHandler(btnReset_Click); // Murni sebagai tombol Reset Form
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            this.dataGridView2.CellClick += new DataGridViewCellEventHandler(dataGridView2_CellClick);

            this.textBox2.TextChanged += new EventHandler(textBox2_TextChanged);
            this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);

            this.txtNma.KeyPress += new KeyPressEventHandler(txtNma_KeyPress);
            this.txtJmlh.KeyPress += new KeyPressEventHandler(txtJmlh_KeyPress);

            // Sensor Navigator BindingSource untuk sinkronisasi data tabel riwayat transaksi
            this.navBindingSource.PositionChanged += new EventHandler(navBindingSource_PositionChanged);

            // Kunci Keamanan: Override paksa jika tombol hapus bawaan designer diklik
          
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void permintaanBarang_Load(object sender, EventArgs e)
        {
            // Otomatisasi pembatasan kalender log laptop hari ini
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is DateTimePicker dtp)
                {
                    dtp.MinDate = DateTime.Today;
                    dtp.MaxDate = DateTime.Today;
                    dtp.Value = DateTime.Today;
                }
            }

            LoadComboRuangan();
            LoadComboSemester(); // Automasi semester aktif
            RefreshSemuaTabel();
            ResetForm();
        }

        // VALIDASI: Input nama hanya boleh huruf dan spasi
        private void txtNma_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // VALIDASI: Input jumlah hanya boleh angka bulat murni
        private void txtJmlh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // OTOMASI: Mengambil String Semester Akademik Berdasarkan Tanggal Laptop
        private string GetActiveSemesterFromSystem()
        {
            int tahunSekarang = DateTime.Now.Year;
            int bulanSekarang = DateTime.Now.Month;

            // Standar Akademik Kampus: Ganjil (Juli - Desember), Genap (Januari - Juni)
            string tipeSemester = (bulanSekarang >= 7) ? "Ganjil" : "Genap";
            string tahunAjaran = (bulanSekarang >= 7) ? $"{tahunSekarang}/{tahunSekarang + 1}" : $"{tahunSekarang - 1}/{tahunSekarang}";

            return $"{tahunAjaran} ({tipeSemester})";
        }

        private void LoadComboRuangan()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT * FROM [dbo].[vwDaftarRuangan]";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbRuangan.DataSource = dt;
                        cmbRuangan.DisplayMember = "namaRuangan";
                        cmbRuangan.ValueMember = "idRuangan";
                        cmbRuangan.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Ruangan", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadComboSemester()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idSemester, tahunAjaran FROM [master].[semester] WHERE tahunAjaran = @active";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@active", GetActiveSemesterFromSystem());
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            cmbSemester.DataSource = dt;
                            cmbSemester.DisplayMember = "tahunAjaran";
                            cmbSemester.ValueMember = "idSemester";

                            cmbSemester.Enabled = false; // Dikunci otomatis oleh sistem laptop
                            if (dt.Rows.Count > 0) cmbSemester.SelectedIndex = 0;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Semester", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadDataBarang(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    // MENGAMBIL DATA LANGSUNG DARI TABEL MASTER (Kunci: Hanya barang habis pakai/Non-Maintenance -> tipeBarang = 0)
                    // Format kolom disesuaikan dengan ID Detail Barang agar seragam
                    string query = @"SELECT idBarang AS [ID], namaBarang AS [Nama Barang], stok AS [Total Stok (Pcs)], 
                                    satuan AS [Satuan Master], isiKonversi AS [Isi Per Satuan]
                                    FROM [master].[barang] WHERE tipeBarang = 0";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " AND (namaBarang LIKE @kw OR idBarang LIKE @kw)";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Katalog Barang", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadDataTransaksi(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT * FROM [dbo].[vwDataTransaksi]";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE [Barang] LIKE @kw OR [Peminta] LIKE @kw";
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                        dtTransaksi = new DataTable();
                        da.Fill(dtTransaksi);

                        isResetting = true;
                        navBindingSource.DataSource = dtTransaksi;
                        dataGridView2.DataSource = navBindingSource;
                        bindingNavigator1.BindingSource = navBindingSource;
                        isResetting = false;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Riwayat Transaksi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // SENSOR NAVIGATOR LOG: Mengunci data transaksi lama agar tidak bisa di-update/delete (Read-Only)
        private void navBindingSource_PositionChanged(object sender, EventArgs e)
        {
            if (isResetting || navBindingSource.Current == null) return;

            DataRowView row = (DataRowView)navBindingSource.Current;

            selectedIdPermintaan = row["ID"].ToString();
            txtIdBarang.Text = row["ID Barang"].ToString();
            cmbRuangan.SelectedValue = Convert.ToInt32(row["idRuangan"]);
            txtNma.Text = row["Peminta"].ToString();
            txtJmlh.Text = row["Qty"].ToString();
            cmbSemester.SelectedValue = Convert.ToInt32(row["idSemester"]);

            // Kunci Pilihan Satuan Kuantitas Log Lama
            cmbSatuanRequest.Items.Clear();
            cmbSatuanRequest.Items.Add("Pcs");
            cmbSatuanRequest.SelectedIndex = 0;
            cmbSatuanRequest.Enabled = false;

            // Matikan Hak Akses Manipulasi Data
            addPeminta.Enabled = false;
            addPeminta.Text = "Log Permanen (No Update)";
        }

        public void RefreshSemuaTabel()
        {
            LoadDataBarang(textBox2.Text.Trim());
            LoadDataTransaksi(textBox1.Text.Trim());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LoadDataBarang(textBox2.Text.Trim());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadDataTransaksi(textBox1.Text.Trim());
        }

        private void ResetForm()
        {
            isResetting = true;

            txtIdBarang.Clear();
            cmbRuangan.SelectedIndex = -1;
            txtNma.Clear();
            txtJmlh.Clear();

            cmbSatuanRequest.Items.Clear();
            cmbSatuanRequest.Enabled = false;

            selectedIdPermintaan = "";
            masterSatuanBarang = "Pcs";
            stokTersediaBarangPcs = 0;
            isiKonversiBarang = 1;

            addPeminta.Text = "Tambah Permintaan";
            addPeminta.Enabled = true;

            if (cmbSemester.Items.Count > 0) cmbSemester.SelectedIndex = 0;

            isResetting = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
            textBox2.Clear();
            textBox1.Clear();
            RefreshSemuaTabel();
        }

        // SELEKSI BARANG (Tabel Katalog Kanan): Menyiapkan Opsi Kuantitas Dinamis
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                ResetForm();

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                txtIdBarang.Text = row.Cells["ID"].Value.ToString();
                masterSatuanBarang = row.Cells["Satuan Master"].Value.ToString();
                stokTersediaBarangPcs = Convert.ToInt32(row.Cells["Total Stok (Pcs)"].Value);
                isiKonversiBarang = Convert.ToInt32(row.Cells["Isi Per Satuan"].Value);

                // LOGIC DINAMIS SATUAN: Masukkan opsi satuan master besar & eceran Pcs
                cmbSatuanRequest.Items.Clear();
                cmbSatuanRequest.Items.Add(masterSatuanBarang); // Misal: Rem, Box, atau Pack

                if (masterSatuanBarang != "Pcs")
                {
                    cmbSatuanRequest.Items.Add("Pcs"); // Sediakan opsi eceran murni tanpa batas kelipatan pecahan!
                }

                cmbSatuanRequest.SelectedIndex = 0;
                cmbSatuanRequest.Enabled = true;
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                navBindingSource.Position = e.RowIndex;
            }
        }

        private int GetIdSemesterOtomatis(SqlConnection conn, SqlTransaction transaction)
        {
            string query = "SELECT idSemester FROM [master].[semester] WHERE tahunAjaran = @TA";
            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@TA", GetActiveSemesterFromSystem());
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
                else throw new Exception($"Tahun akademik '{GetActiveSemesterFromSystem()}' belum terdaftar di master data semester!");
            }
        }

        // PROSES TRANSAKSI TAMBAH LOG PERMINTAAN BARANG
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdBarang.Text) || string.IsNullOrWhiteSpace(txtNma.Text) ||
                string.IsNullOrWhiteSpace(txtJmlh.Text) || cmbRuangan.SelectedValue == null || cmbSatuanRequest.SelectedIndex == -1)
            {
                MessageBox.Show("Seluruh komponen form wajib diisi secara lengkap.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int jumlahRequested = Convert.ToInt32(txtJmlh.Text.Trim());
            if (jumlahRequested <= 0)
            {
                MessageBox.Show("Jumlah barang yang diminta harus lebih dari angka 0.", "Validasi Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string satuanDipilih = cmbSatuanRequest.SelectedItem.ToString();

            // =======================================================
            // 1. EVALUASI ATURAN BATAS MAKSIMUM PERMINTAAN KAMPUS
            // =======================================================
            if (satuanDipilih == "Pcs" && jumlahRequested > 100) { MessageBox.Show("Batas maksimum permintaan eceran adalah 100 Pcs per pengajuan!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (satuanDipilih == "Rem" && jumlahRequested > 5) { MessageBox.Show("Batas maksimum pengajuan kertas adalah 5 Rem!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (satuanDipilih == "Box" && jumlahRequested > 5) { MessageBox.Show("Batas maksimum pengajuan barang adalah 5 Box!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (satuanDipilih == "Pack" && jumlahRequested > 5) { MessageBox.Show("Batas maksimum pengajuan barang adalah 5 Pack!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (satuanDipilih == "Roll" && jumlahRequested > 3) { MessageBox.Show("Batas maksimum pengajuan kabel/isolasi adalah 3 Roll!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            // =======================================================
            // 2. KALKULATOR MATEMATIKA KONVERSI QUANTITY KE PCS MURNI
            // =======================================================
            int totalJumlahDalamPcsAsli = 0;

            if (satuanDipilih == masterSatuanBarang)
            {
                // Kasus A: User memilih satuan besar (Contoh: minta 2 Rem -> 2 * 500 = 1000 Pcs)
                totalJumlahDalamPcsAsli = jumlahRequested * isiKonversiBarang;
            }
            else if (satuanDipilih == "Pcs")
            {
                // Kasus B: User mengecer secara bebas (Contoh: minta 50 Pcs -> Langsung bernilai 50 Pcs tanpa pembulatan desimal!)
                totalJumlahDalamPcsAsli = jumlahRequested;
            }

            // =======================================================
            // 3. VALIDASI KETERSEDIAAN STOK REAL-TIME
            // =======================================================
            if (stokTersediaBarangPcs < totalJumlahDalamPcsAsli)
            {
                MessageBox.Show($"Transaksi digagalkan! Stok di gudang tidak mencukupi.\nSisa stok aktual saat ini: {stokTersediaBarangPcs} Pcs.", "Stok Defisit", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // =======================================================
            // 4. EKSEKUSI DATA SECARA ATOMIK KE DATABASE SQL SERVER
            // =======================================================
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        int idSemesterAktif = GetIdSemesterOtomatis(conn, transaction);

                        using (SqlCommand cmd = new SqlCommand("sp_SavePermintaanBarang", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Paksa parameter ID Log bernilai NULL agar Stored Procedure murni melakukan INSERT log baru (Anti Update)
                            cmd.Parameters.AddWithValue("@idPB", DBNull.Value);
                            cmd.Parameters.AddWithValue("@idB", txtIdBarang.Text.Trim());
                            cmd.Parameters.AddWithValue("@idR", cmbRuangan.SelectedValue);
                            cmd.Parameters.AddWithValue("@nama", txtNma.Text.Trim());

                            // Kirimkan total kuantitas hasil kalkulasi Pcs murni ke Stored Procedure database
                            cmd.Parameters.AddWithValue("@jml", totalJumlahDalamPcsAsli);
                            cmd.Parameters.AddWithValue("@smt", idSemesterAktif);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"Log Berhasil Disimpan! Permintaan sebanyak {jumlahRequested} {satuanDipilih} telah tercatat secara permanen.", "Transaksi Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RefreshSemuaTabel();
                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Integritas Transaksi Gagal: " + ex.Message, "Sistem Menolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Fatal Database Connection Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void permintaanBarang_Load_1(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void stockBarang_Click(object sender, EventArgs e) { }

        private void permintaanBarang_Load_2(object sender, EventArgs e)
        {

        }
    }
}