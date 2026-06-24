using ExcelDataReader;
using SatprasDesktopApp.Config;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class kelolaBarang : Form
    {
        private bool isEditMode = false;
        private string originalIdBarang = "";

        // Flag pengaman biar form input gak bentrok pas reload data master
        private bool isResetting = false;
        private BindingSource bsBarang = new BindingSource();

        private DataTable dtPreviewExcel;
        private bool isPreviewMode = false;

        public kelolaBarang()
        {
            InitializeComponent();

            this.Load += new EventHandler(kelolaBarang_Load);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dgvBarang_CellClick);
            this.txtCari.TextChanged += new EventHandler(txtCari_TextChanged);
            this.txtJumlahBarang.KeyPress += new KeyPressEventHandler(txtStok_KeyPress);
            this.cmbTipeBarang.SelectedIndexChanged += new EventHandler(cmbTipeBarang_SelectedIndexChanged);

            this.btnTambahBarang.Click += new EventHandler(btnSimpan_Click);
            this.txtReset.Click += new EventHandler(btnBatal_Click);
            this.button3.Click += new EventHandler(button3_Click);

            // Mengikat pergerakan baris remot Navigator / Klik Grid ke isian form
            this.bsBarang.PositionChanged += new EventHandler(bsBarang_PositionChanged);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormDetailBarang form = new FormDetailBarang();
            this.Hide();
            form.Show();
        }

        private void kelolaBarang_Load(object sender, EventArgs e)
        {
            DataTable dtTipe = new DataTable();
            dtTipe.Columns.Add("Value", typeof(int));
            dtTipe.Columns.Add("Display", typeof(string));
            dtTipe.Rows.Add(0, "Barang Habis Pakai (Non-Rutin)");
            dtTipe.Rows.Add(1, "Aset Tetap (Pengecekan Rutin)");

            cmbTipeBarang.DataSource = dtTipe;
            cmbTipeBarang.DisplayMember = "Display";
            cmbTipeBarang.ValueMember = "Value";
            cmbTipeBarang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipeBarang.SelectedIndex = -1;

            LoadComboBoxKuantitas();
            LoadComboMerk();
            LoadAutoCompleteNamaBarang();
            LoadDataBarang();
            ResetForm();
        }

        // Mengatur form isi otomatis mengikuti remot navigasi
        private void bsBarang_PositionChanged(object sender, EventArgs e)
        {
            if (isResetting || bsBarang.Current == null) return;

            DataRowView row = (DataRowView)bsBarang.Current;

            originalIdBarang = row["ID/Kode Barang"].ToString();
            txtNamaBarang.Text = row["Nama Barang"].ToString();
            cmbMerk.Text = row["Merk"].ToString();
            txtJumlahBarang.Text = row["Sisa Stok"].ToString();

            cmbTipeBarang.SelectedValue = Convert.ToInt32(row["tipeBarang"]);
            cmbKuantitas.SelectedValue = row["Satuan"].ToString();

            cmbTipeBarang.Enabled = false;
            isEditMode = true;

            btnTambahBarang.Text = "Update";
        }

        private void LoadComboBoxKuantitas()
        {
            DataTable dtKuantitas = new DataTable();
            dtKuantitas.Columns.Add("Display", typeof(string));
            dtKuantitas.Columns.Add("NamaSatuan", typeof(string));
            dtKuantitas.Columns.Add("MaxLimit", typeof(int));

            dtKuantitas.Rows.Add("Pcs (Pieces)", "Pcs", 100);
            dtKuantitas.Rows.Add("Rem (Ream)", "Rem", 50);
            dtKuantitas.Rows.Add("Box", "Box", 30);
            dtKuantitas.Rows.Add("Pack", "Pack", 40);
            dtKuantitas.Rows.Add("Roll", "Roll", 20);

            cmbKuantitas.DataSource = dtKuantitas;
            cmbKuantitas.DisplayMember = "Display";
            cmbKuantitas.ValueMember = "NamaSatuan";
            cmbKuantitas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKuantitas.SelectedIndex = -1;
        }

        private void LoadComboMerk()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idMerk, namaMerk FROM master.merk";
                    using (var da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        cmbMerk.DataSource = dt;
                        cmbMerk.DisplayMember = "namaMerk";
                        cmbMerk.ValueMember = "idMerk";

                        cmbMerk.DropDownStyle = ComboBoxStyle.DropDown;
                        cmbMerk.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        cmbMerk.AutoCompleteSource = AutoCompleteSource.ListItems;
                        cmbMerk.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadAutoCompleteNamaBarang()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT DISTINCT namaBarang FROM master.barang";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            AutoCompleteStringCollection source = new AutoCompleteStringCollection();
                            while (reader.Read())
                            {
                                source.Add(reader["namaBarang"].ToString());
                            }
                            txtNamaBarang.AutoCompleteCustomSource = source;
                            txtNamaBarang.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                            txtNamaBarang.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        }
                    }
                }
            }
            catch (Exception) { }
        }

        private void LoadDataBarang(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                    SELECT 
                        b.idBarang AS [ID/Kode Barang],
                        b.namaBarang AS [Nama Barang],
                        m.namaMerk AS [Merk],
                        b.stok AS [Sisa Stok],
                        b.satuan AS [Satuan],
                        CASE WHEN b.tipeBarang = 1 THEN 'Aset Tetap (Rutin)' ELSE 'Habis Pakai (Non-Rutin)' END AS [Kategori],
                        b.tipeBarang,
                        b.idMerk 
                    FROM master.barang b
                    LEFT JOIN master.merk m ON b.idMerk = m.idMerk";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE b.namaBarang LIKE @keyword OR b.idBarang LIKE @keyword OR m.namaMerk LIKE @keyword";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword))
                        {
                            cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                        }

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            isResetting = true;
                            bsBarang.DataSource = dt;
                            bindingNavigator1.BindingSource = bsBarang;
                            dataGridView1.DataSource = bsBarang;
                            isResetting = false;

                            if (dataGridView1.Columns["tipeBarang"] != null)
                                dataGridView1.Columns["tipeBarang"].Visible = false;

                            if (dataGridView1.Columns["idMerk"] != null)
                                dataGridView1.Columns["idMerk"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ResetForm()
        {
            isResetting = true;

            txtNamaBarang.Clear();
            cmbMerk.SelectedIndex = -1;
            cmbMerk.Text = "";
            txtJumlahBarang.Clear();
            cmbTipeBarang.SelectedIndex = -1;
            cmbKuantitas.SelectedIndex = -1;

            txtJumlahBarang.ReadOnly = false;
            txtJumlahBarang.BackColor = SystemColors.Window;

            cmbTipeBarang.Enabled = true;
            cmbKuantitas.Enabled = true;

            isEditMode = false;
            originalIdBarang = "";

            btnTambahBarang.Text = "Simpan";
            txtNamaBarang.Focus();

            isResetting = false;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void cmbTipeBarang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipeBarang.SelectedIndex == -1 || cmbTipeBarang.SelectedValue == null) return;

            int tipe;
            if (!int.TryParse(cmbTipeBarang.SelectedValue.ToString(), out tipe)) return;

            if (tipe == 1)
            {
                txtJumlahBarang.Text = "0";
                txtJumlahBarang.ReadOnly = true;
                txtJumlahBarang.BackColor = SystemColors.Control;

                cmbKuantitas.SelectedValue = "Pcs";
                cmbKuantitas.Enabled = false;
            }
            else
            {
                txtJumlahBarang.ReadOnly = false;
                txtJumlahBarang.BackColor = SystemColors.Window;
                cmbKuantitas.Enabled = true;

                if (!isEditMode)
                {
                    if (txtJumlahBarang.Text == "0") txtJumlahBarang.Clear();
                    cmbKuantitas.SelectedIndex = -1;
                }
            }
        }

        private void txtStok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataBarang(txtCari.Text.Trim());
        }

        private void dgvBarang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bsBarang.Position = e.RowIndex;
            }
        }

        // ==========================================
        // FUNGSI 1: GET ATAU CREATE MERK OTOMATIS (TERSEDIA KEMBALI)
        // ==========================================
        private int GetOrCreateMerk(string namaMerk, SqlConnection conn, SqlTransaction trans)
        {
            namaMerk = namaMerk.Trim();
            string checkQ = "SELECT idMerk FROM master.merk WHERE namaMerk = @nama";

            using (var cmd = new SqlCommand(checkQ, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaMerk);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }

            string insertQ = "INSERT INTO master.merk (namaMerk) OUTPUT INSERTED.idMerk VALUES (@nama)";
            using (var cmd = new SqlCommand(insertQ, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaMerk);
                return (int)cmd.ExecuteScalar();
            }
        }

        // ==========================================
        // FUNGSI 2: GENERATOR KODE BARANG OTOMATIS (TERSEDIA KEMBALI)
        // ==========================================
        private string GenerateIdBarangOtomatis(string namaMerk, int tipeBarang, SqlConnection conn, SqlTransaction trans)
        {
            string cleanMerk = namaMerk.Trim().Replace(" ", "").ToUpper();
            string singkatanMerk = cleanMerk.Length >= 3 ? cleanMerk.Substring(0, 3) : cleanMerk.PadRight(3, 'X');
            string codePrefix = $"TI-{singkatanMerk}-{tipeBarang}-";

            string countQuery = "SELECT COUNT(*) FROM master.barang WHERE idBarang LIKE @prefix + '%'";
            int runningNumber = 1;

            using (SqlCommand cmd = new SqlCommand(countQuery, conn, trans))
            {
                cmd.Parameters.AddWithValue("@prefix", codePrefix);
                runningNumber = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
            }

            return $"{codePrefix}{runningNumber.ToString("D3")}";
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNamaBarang.Text) ||
                string.IsNullOrWhiteSpace(cmbMerk.Text) ||
                string.IsNullOrWhiteSpace(txtJumlahBarang.Text) ||
                cmbTipeBarang.SelectedValue == null ||
                cmbKuantitas.SelectedIndex == -1)
            {
                MessageBox.Show("Seluruh form wajib diisi termasuk Merk dan Tipe Kuantitas Satuan.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tipeInput = Convert.ToInt32(cmbTipeBarang.SelectedValue);
            int stokInput = Convert.ToInt32(txtJumlahBarang.Text.Trim());
            string satuanInput = cmbKuantitas.SelectedValue.ToString();

            if (tipeInput == 0 && !isEditMode)
            {
                if (stokInput <= 0)
                {
                    MessageBox.Show("Untuk Barang Habis Pakai, stok awal harus lebih dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtJumlahBarang.Focus();
                    return;
                }

                DataRowView selectedKuantitas = (DataRowView)cmbKuantitas.SelectedItem;
                int maxWajar = Convert.ToInt32(selectedKuantitas["MaxLimit"]);

                if (stokInput > maxWajar)
                {
                    MessageBox.Show($"Pembelian berlebihan! Batas kuantitas wajar untuk satuan '{cmbKuantitas.Text}' di kampus adalah maksimal {maxWajar}!", "Batas Tidak Wajar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtJumlahBarang.Focus();
                    return;
                }
            }

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        int resolvedIdMerk = GetOrCreateMerk(cmbMerk.Text, conn, transaction);
                        string query;
                        string finalIdBarang;

                        if (isEditMode)
                        {
                            finalIdBarang = originalIdBarang;
                            query = "UPDATE master.barang SET namaBarang = @nama, idMerk = @idMerk, stok = @stok, satuan = @satuan WHERE idBarang = @idAsli";
                        }
                        else
                        {
                            finalIdBarang = GenerateIdBarangOtomatis(cmbMerk.Text, tipeInput, conn, transaction);
                            query = "INSERT INTO master.barang (idBarang, namaBarang, idMerk, stok, tipeBarang, satuan) VALUES (@id, @nama, @idMerk, @stok, @tipe, @satuan)";
                        }

                        using (var cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", finalIdBarang);
                            cmd.Parameters.AddWithValue("@nama", txtNamaBarang.Text.Trim());
                            cmd.Parameters.AddWithValue("@idMerk", resolvedIdMerk);
                            cmd.Parameters.AddWithValue("@stok", stokInput);
                            cmd.Parameters.AddWithValue("@satuan", satuanInput);

                            if (!isEditMode) cmd.Parameters.AddWithValue("@tipe", tipeInput);
                            if (isEditMode) cmd.Parameters.AddWithValue("@idAsli", originalIdBarang);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"Data inventaris Master dengan Kode '{finalIdBarang}' [{satuanInput}] berhasil {(isEditMode ? "diperbarui" : "disimpan")}!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadComboMerk();
                        LoadAutoCompleteNamaBarang();
                        LoadDataBarang();
                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message, "Validasi Sistem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kesalahan Database: " + ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    
        private int GetOrCreateGedung(string namaGedung, SqlConnection conn, SqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(namaGedung)) return 0;
            string query = "SELECT idGedung FROM master.gedung WHERE namaGedung = @nama";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaGedung.Trim());
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }
            using (var cmd = new SqlCommand("INSERT INTO master.gedung (namaGedung) OUTPUT INSERTED.idGedung VALUES (@nama)", conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaGedung.Trim());
                return (int)cmd.ExecuteScalar();
            }
        }

        private int GetOrCreateRuangan(string namaRuangan, int idGedung, SqlConnection conn, SqlTransaction trans)
        {
            if (string.IsNullOrWhiteSpace(namaRuangan)) return 0;
            string query = "SELECT idRuangan FROM master.ruangan WHERE namaRuangan = @nama AND idGedung = @idGedung";
            using (var cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@nama", namaRuangan.Trim());
                cmd.Parameters.AddWithValue("@idGedung", idGedung);
                object result = cmd.ExecuteScalar();
                if (result != null) return Convert.ToInt32(result);
            }
            using (var cmd = new SqlCommand("INSERT INTO master.ruangan (idGedung, namaRuangan) OUTPUT INSERTED.idRuangan VALUES (@idGedung, @nama)", conn, trans))
            {
                cmd.Parameters.AddWithValue("@idGedung", idGedung);
                cmd.Parameters.AddWithValue("@nama", namaRuangan.Trim());
                return (int)cmd.ExecuteScalar();
            }
        }

        private void SwitchToPreviewMode(bool previewOn)
        {
            isPreviewMode = previewOn;

            if (previewOn)
            {
                bsBarang.DataSource = null;
                dataGridView1.DataSource = dtPreviewExcel;

                btnTambahBarang.Enabled = false;
                txtCari.Enabled = false;
                cmbMerk.Enabled = false;
                cmbTipeBarang.Enabled = false;
                cmbKuantitas.Enabled = false;
                txtNamaBarang.Enabled = false;
                txtJumlahBarang.Enabled = false;

                btnDB.Enabled = true;
                btnDB.Text = "Eksekusi DB";
                btnDB.BackColor = Color.OrangeRed;
                btnDB.ForeColor = Color.White;

                txtReset.Text = "Batal Import";
            }
            else
            {
                dtPreviewExcel.Clear();
                btnTambahBarang.Enabled = true;
                txtCari.Enabled = true;
                cmbMerk.Enabled = true;
                cmbTipeBarang.Enabled = true;
                cmbKuantitas.Enabled = true;
                txtNamaBarang.Enabled = true;
                txtJumlahBarang.Enabled = true;

                btnDB.Enabled = false;
                btnDB.Text = "Import Excel";
                btnDB.BackColor = SystemColors.Control;
                btnDB.ForeColor = Color.Black;

                txtReset.Text = "Reset Form";

                LoadDataBarang();
                ResetForm();
            }
        }

        private string GetActiveSemesterFromSystem()
        {
            int tahunSekarang = DateTime.Now.Year;
            int bulanSekarang = DateTime.Now.Month;

            string tipeSemester;
            string tahunAjaran;

            if (bulanSekarang >= 7)
            {
                tipeSemester = "Ganjil";
                tahunAjaran = $"{tahunSekarang}/{tahunSekarang + 1}";
            }
            else
            {
                tipeSemester = "Genap";
                tahunAjaran = $"{tahunSekarang - 1}/{tahunSekarang}";
            }

            return $"{tahunAjaran} ({tipeSemester})";
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                                });

                                dtPreviewExcel = result.Tables[0];

                                if (!dtPreviewExcel.Columns.Contains("NamaBarang") || !dtPreviewExcel.Columns.Contains("IDDetailBarang"))
                                {
                                    MessageBox.Show("Format Excel salah. Pastikan Header Excel Anda sesuai dengan standar sistem:\n[NamaBarang, Merk, TipeBarang, Satuan, IsiKonversi, StokHabisPakai, IDDetailBarang, Spesifikasi, NamaGedung, NamaRuangan]", "Penolakan Sistem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    return;
                                }

                                SwitchToPreviewMode(true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"File terkunci atau rusak. Tutup Excel jika sedang dibuka. Detail: {ex.Message}", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            if (!isPreviewMode || dtPreviewExcel == null || dtPreviewExcel.Rows.Count == 0) return;

            DialogResult dialog = MessageBox.Show($"Sistem akan mengeksekusi {dtPreviewExcel.Rows.Count} baris instruksi ke Database. Tindakan ini bersifat mutlak dan tidak bisa dibatalkan. Eksekusi?", "Konfirmasi Otorisasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog != DialogResult.Yes) return;

            using (var conn = DatabaseConfig.GetConnection())
            {
                if (conn == null) return;
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    int idSemesterMasuk = 0;
                    string currentSemesterText = GetActiveSemesterFromSystem();
                    string qSemester = "SELECT idSemester FROM master.semester WHERE tahunAjaran = @tahunAjaran";
                    using (SqlCommand cmdSem = new SqlCommand(qSemester, conn, transaction))
                    {
                        cmdSem.Parameters.AddWithValue("@tahunAjaran", currentSemesterText);
                        object result = cmdSem.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            throw new Exception($"Semester Aktif ({currentSemesterText}) belum terdaftar di database! Silakan daftarkan terlebih dahulu di menu Semester.");
                        }
                        idSemesterMasuk = Convert.ToInt32(result);
                    }

                    var cacheMasterBarang = new Dictionary<string, string>();

                    foreach (DataRow row in dtPreviewExcel.Rows)
                    {
                        string namaBarang = row["NamaBarang"].ToString().Trim();
                        if (string.IsNullOrWhiteSpace(namaBarang)) continue;

                        string merk = row["Merk"].ToString().Trim();
                        int tipeBarang = Convert.ToInt32(row["TipeBarang"]);
                        string satuan = row["Satuan"].ToString().Trim();
                        int isiKonversi = string.IsNullOrWhiteSpace(row["IsiKonversi"].ToString()) ? 1 : Convert.ToInt32(row["IsiKonversi"]);

                        int idMerk = GetOrCreateMerk(merk, conn, transaction);
                        string masterKey = $"{namaBarang}_{idMerk}_{tipeBarang}";
                        string idBarang = "";

                        if (!cacheMasterBarang.ContainsKey(masterKey))
                        {
                            idBarang = GenerateIdBarangOtomatis(merk, tipeBarang, conn, transaction);

                            int stokAwal = 0;
                            if (tipeBarang == 0)
                            {
                                stokAwal = string.IsNullOrWhiteSpace(row["StokHabisPakai"].ToString()) ? 0 : Convert.ToInt32(row["StokHabisPakai"]);
                            }
                            else
                            {
                                stokAwal = dtPreviewExcel.Select($"NamaBarang = '{namaBarang}' AND Merk = '{merk}'").Length;
                            }

                            string insertMasterQ = "INSERT INTO master.barang (idBarang, namaBarang, stok, tipeBarang, idMerk, satuan, isiKonversi) VALUES (@id, @nama, @stok, @tipe, @idMerk, @satuan, @konversi)";
                            using (var cmdM = new SqlCommand(insertMasterQ, conn, transaction))
                            {
                                cmdM.Parameters.AddWithValue("@id", idBarang);
                                cmdM.Parameters.AddWithValue("@nama", namaBarang);
                                cmdM.Parameters.AddWithValue("@stok", stokAwal);
                                cmdM.Parameters.AddWithValue("@tipe", tipeBarang);
                                cmdM.Parameters.AddWithValue("@idMerk", idMerk);
                                cmdM.Parameters.AddWithValue("@satuan", satuan);
                                cmdM.Parameters.AddWithValue("@konversi", isiKonversi);
                                cmdM.ExecuteNonQuery();
                            }
                            cacheMasterBarang.Add(masterKey, idBarang);
                        }
                        else
                        {
                            idBarang = cacheMasterBarang[masterKey];
                        }

                        if (tipeBarang == 1)
                        {
                            string idDetail = row["IDDetailBarang"].ToString().Trim();
                            string gedung = row["NamaGedung"].ToString().Trim();
                            string ruangan = row["NamaRuangan"].ToString().Trim();
                            string spesifikasi = row["Spesifikasi"].ToString().Trim();

                            if (string.IsNullOrWhiteSpace(idDetail) || string.IsNullOrWhiteSpace(ruangan))
                            {
                                throw new Exception($"Aset '{namaBarang}' tidak memiliki ID Detail atau Nama Ruangan.");
                            }

                            int idGedung = GetOrCreateGedung(gedung, conn, transaction);
                            int idRuangan = GetOrCreateRuangan(ruangan, idGedung, conn, transaction);

                            string insertDetailQ = "INSERT INTO [transaction].[detailBarang] (idDetailBarang, idBarang, idRuangan, spesifikasi, satuan, idSemesterMasuk, tglMasuk, statusAset) VALUES (@idDetail, @idBarang, @idRuangan, @spek, @satuan, @idSemester, @tglMasuk, @statusAset)";
                            using (var cmdD = new SqlCommand(insertDetailQ, conn, transaction))
                            {
                                cmdD.Parameters.AddWithValue("@idDetail", idDetail);
                                cmdD.Parameters.AddWithValue("@idBarang", idBarang);
                                cmdD.Parameters.AddWithValue("@idRuangan", idRuangan);
                                cmdD.Parameters.AddWithValue("@spek", spesifikasi);
                                cmdD.Parameters.AddWithValue("@satuan", satuan);
                                cmdD.Parameters.AddWithValue("@idSemester", idSemesterMasuk);
                                cmdD.Parameters.AddWithValue("@tglMasuk", DateTime.Now);
                                cmdD.Parameters.AddWithValue("@statusAset", 1);
                                cmdD.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Integritas data berhasil disuntikkan ke Database tanpa celah.", "Eksekusi Sempurna", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SwitchToPreviewMode(false);
                }
                catch (SqlException sqlEx)
                {
                    transaction.Rollback();
                    if (sqlEx.Number == 2627)
                        MessageBox.Show("Ditemukan ID Detail Barang ganda (Primary Key Collision) pada file Excel. Transaksi ditarik mundur mutlak.", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else
                        MessageBox.Show($"Kesalahan Arsitektur SQL: {sqlEx.Message}", "Rollback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Cacat Logika: {ex.Message}\nSeluruh instruksi telah di-Rollback.", "Rollback", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
}
}