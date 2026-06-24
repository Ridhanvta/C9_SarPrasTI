using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class FormDetailBarang : Form
    {
        private bool isEditMode = false;
        private string originalIdDetail = "";
        private string originalIdBarang = "";

        // Flag pengaman agar tidak terjadi bentrokan data pas form di-reset
        private bool isResetting = false;
        private BindingSource bsDetail = new BindingSource();

        public FormDetailBarang()
        {
            InitializeComponent();
            this.Load += new EventHandler(FormDetailBarang_Load);
            this.dgvDetail.CellClick += new DataGridViewCellEventHandler(dgvDetail_CellClick);
            this.txtCari.TextChanged += new EventHandler(txtCari_TextChanged);
            this.cmbGedung.SelectedIndexChanged += new EventHandler(cmbGedung_SelectedIndexChanged);
            this.cmbBarang.Leave += new EventHandler(cmbBarang_Leave);
            this.btnSimpan.Click += new EventHandler(btnSimpan_Click);
            this.btnBatal.Click += new EventHandler(btnBatal_Click);

            // Kunci input bulk hanya angka
            this.txtJumlahInput.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            };

            // SENSOR UTAMA: Mengikat perubahan navigasi tombol maupun grid ke form inputan
            this.bsDetail.PositionChanged += new EventHandler(bsDetail_PositionChanged);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            kelolaBarang kelola = new kelolaBarang();
            kelola.Show();
            this.Hide();
        }

        private void FormDetailBarang_Load(object sender, EventArgs e)
        {
            LoadComboBoxBarang();
            LoadComboBoxGedung();
            LoadDataDetail();
            ResetForm();
        }

        // FUNGSI UTAMA: Otomatis mengisi form setiap kali posisi data bergeser (klik Grid / klik Navigator)
        private void bsDetail_PositionChanged(object sender, EventArgs e)
        {
            if (isResetting || bsDetail.Current == null) return;

            DataRowView row = (DataRowView)bsDetail.Current;

            originalIdDetail = row["ID Detail"].ToString();
            txtSpesifikasi.Text = row["Spesifikasi"].ToString();
            originalIdBarang = row["idBarang"].ToString();

            cmbBarang.SelectedValue = originalIdBarang;
            cmbGedung.SelectedValue = Convert.ToInt32(row["idGedung"]);
            cmbRuangan.SelectedValue = Convert.ToInt32(row["idRuangan"]);

            cmbBarang.Enabled = false;
            txtJumlahInput.Text = "1";
            txtJumlahInput.Enabled = false;

            isEditMode = true;
            btnSimpan.Text = "Update Data";
        }

        private void LoadComboBoxBarang()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                        SELECT 
                            b.idBarang, 
                            b.namaBarang + '/' + ISNULL(m.namaMerk, 'No Merk') AS displayBarang 
                        FROM master.barang b
                        LEFT JOIN master.merk m ON b.idMerk = m.idMerk
                        WHERE b.tipeBarang = 1";

                    using (var da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        DataRow dr = dt.NewRow();
                        dr["idBarang"] = "-- Pilih Aset Barang --";
                        dr["displayBarang"] = "-- Pilih Aset Barang --";
                        dt.Rows.InsertAt(dr, 0);

                        cmbBarang.DataSource = dt;
                        cmbBarang.DisplayMember = "displayBarang";
                        cmbBarang.ValueMember = "idBarang";
                        cmbBarang.DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadComboBoxGedung()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idGedung, namaGedung FROM master.gedung";
                    using (var da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        DataRow dr = dt.NewRow();
                        dr["idGedung"] = 0;
                        dr["namaGedung"] = "-- Pilih Gedung --";
                        dt.Rows.InsertAt(dr, 0);

                        cmbGedung.DataSource = dt;
                        cmbGedung.DisplayMember = "namaGedung";
                        cmbGedung.ValueMember = "idGedung";
                        cmbGedung.DropDownStyle = ComboBoxStyle.DropDownList;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void cmbGedung_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGedung.SelectedIndex <= 0 || cmbGedung.SelectedValue == null)
            {
                cmbRuangan.DataSource = null;
                cmbRuangan.Items.Clear();
                cmbRuangan.Items.Add("-- Pilih Ruangan --");
                cmbRuangan.SelectedIndex = 0;
                cmbRuangan.Enabled = false;
                cmbRuangan.DropDownStyle = ComboBoxStyle.DropDownList;
                return;
            }

            try
            {
                int idGedung = Convert.ToInt32(cmbGedung.SelectedValue);
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idRuangan, namaRuangan FROM master.ruangan WHERE idGedung = @idGedung";
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idGedung", idGedung);
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            DataRow dr = dt.NewRow();
                            dr["idRuangan"] = 0;
                            dr["namaRuangan"] = "-- Pilih Ruangan --";
                            dt.Rows.InsertAt(dr, 0);

                            cmbRuangan.DataSource = dt;
                            cmbRuangan.DisplayMember = "namaRuangan";
                            cmbRuangan.ValueMember = "idRuangan";
                            cmbRuangan.DropDownStyle = ComboBoxStyle.DropDownList;
                            cmbRuangan.Enabled = true;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void cmbBarang_Leave(object sender, EventArgs e)
        {
            if (cmbBarang.SelectedIndex == -1 && !string.IsNullOrWhiteSpace(cmbBarang.Text))
            {
                MessageBox.Show("Aset tidak ditemukan di dalam Katalog Master.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbBarang.SelectedIndex = 0;
                cmbBarang.Focus();
            }
        }

        private void LoadDataDetail(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = @"
                        SELECT 
                            db.idDetailBarang AS [ID Detail],
                            b.namaBarang AS [Nama Aset],
                            m.namaMerk AS [Merk],
                            db.spesifikasi AS [Spesifikasi],
                            db.satuan AS [Satuan],
                            g.namaGedung AS [Gedung],
                            r.namaRuangan AS [Lokasi Ruangan],
                            db.tglMasuk AS [Tanggal Masuk],
                            s.tahunAjaran AS [Semester],
                            b.idBarang, g.idGedung, r.idRuangan, b.tipeBarang
                        FROM [transaction].detailBarang db
                        LEFT JOIN [master].barang b ON db.idBarang = b.idBarang
                        LEFT JOIN [master].merk m ON b.idMerk = m.idMerk 
                        LEFT JOIN [master].ruangan r ON db.idRuangan = r.idRuangan
                        LEFT JOIN [master].gedung g ON r.idGedung = g.idGedung
                        LEFT JOIN [master].semester s ON db.idSemesterMasuk = s.idSemester";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE b.namaBarang LIKE @kw OR db.idDetailBarang LIKE @kw OR db.spesifikasi LIKE @kw OR m.namaMerk LIKE @kw";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            isResetting = true; // Nyalakan flag proteksi sementara pas data reload
                            bsDetail.DataSource = dt;
                            bindingNavigator1.BindingSource = bsDetail;
                            dgvDetail.DataSource = bsDetail;
                            
                            if (dgvDetail.Columns["tipeBarang"] != null) dgvDetail.Columns["tipeBarang"].Visible = false;
                            if (dgvDetail.Columns["idBarang"] != null) dgvDetail.Columns["idBarang"].Visible = false;
                            if (dgvDetail.Columns["idGedung"] != null) dgvDetail.Columns["idGedung"].Visible = false;
                            if (dgvDetail.Columns["idRuangan"] != null) dgvDetail.Columns["idRuangan"].Visible = false;

                            isResetting = false;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ResetForm()
        {
            isResetting = true; // Kunci sensor perpindahan data pas form dikosongkan manual

            txtSpesifikasi.Clear();
            txtJumlahInput.Text = "1";
            txtJumlahInput.Enabled = true;
            cmbBarang.Enabled = true;

            if (cmbBarang.Items.Count > 0) cmbBarang.SelectedIndex = 0;
            if (cmbGedung.Items.Count > 0) cmbGedung.SelectedIndex = 0;
            cmbRuangan.Enabled = false;

            isEditMode = false;
            originalIdDetail = "";
            originalIdBarang = "";

            btnSimpan.Text = "Tambah Detail Barang";
            btnSimpan.Enabled = true;
            cmbBarang.Focus();

            isResetting = false; // Buka kembali gembok sensor
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

        private void btnBatal_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataDetail(txtCari.Text.Trim());
        }

        private void dgvDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Cukup pindahkan posisi di BindingSource, mapping kolom otomatis ditangani bsDetail_PositionChanged
                bsDetail.Position = e.RowIndex;
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSpesifikasi.Text) || cmbBarang.SelectedIndex <= 0 ||
                string.IsNullOrWhiteSpace(txtJumlahInput.Text) || cmbGedung.SelectedIndex <= 0 || cmbRuangan.SelectedIndex <= 0)
            {
                MessageBox.Show("Lengkapi semua isian form!", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int jumlahLoop = Convert.ToInt32(txtJumlahInput.Text.Trim());
            if (jumlahLoop < 1)
            {
                MessageBox.Show("Jumlah barang minimal pembelian adalah 1!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedIdBarang = cmbBarang.SelectedValue.ToString();
            string satuanFinal = "Pcs";

            if (!isEditMode && jumlahLoop > 25)
            {
                MessageBox.Show("Batas maksimal input sekaligus untuk jenis Aset Tetap/Maintenance adalah 25 unit!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    int idSemesterMasuk = 0;
                    if (!isEditMode)
                    {
                        string currentSemesterText = GetActiveSemesterFromSystem();
                        string qSemester = "SELECT idSemester FROM master.semester WHERE tahunAjaran = @tahunAjaran";
                        using (SqlCommand cmdSem = new SqlCommand(qSemester, conn))
                        {
                            cmdSem.Parameters.AddWithValue("@tahunAjaran", currentSemesterText);
                            object result = cmdSem.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                            {
                                MessageBox.Show($"Semester Aktif ({currentSemesterText}) belum terdaftar di database!\nSilakan daftarkan terlebih dahulu di menu Semester.", "Akses Diblokir", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                return;
                            }
                            idSemesterMasuk = Convert.ToInt32(result);
                        }
                    }

                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        if (isEditMode)
                        {
                            string qUpdate = "UPDATE [transaction].detailBarang SET idRuangan = @ruang, spesifikasi = @spec, satuan = @satuan WHERE idDetailBarang = @idAsli";
                            using (var cmd = new SqlCommand(qUpdate, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ruang", Convert.ToInt32(cmbRuangan.SelectedValue));
                                cmd.Parameters.AddWithValue("@spec", txtSpesifikasi.Text.Trim());
                                cmd.Parameters.AddWithValue("@satuan", satuanFinal);
                                cmd.Parameters.AddWithValue("@idAsli", originalIdDetail);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            for (int i = 0; i < jumlahLoop; i++)
                            {
                                string countQuery = "SELECT COUNT(*) FROM [transaction].detailBarang WHERE idBarang = @idB";
                                int nextSequence = 1;

                                using (SqlCommand cmdCount = new SqlCommand(countQuery, conn, transaction))
                                {
                                    cmdCount.Parameters.AddWithValue("@idB", selectedIdBarang);
                                    nextSequence = Convert.ToInt32(cmdCount.ExecuteScalar()) + 1;
                                }

                                string finalIdDetail = $"{selectedIdBarang}-{nextSequence.ToString("D3")}";

                                string qInsert = "INSERT INTO [transaction].detailBarang (idDetailBarang, idBarang, idRuangan, spesifikasi, satuan, idSemesterMasuk, tglMasuk, statusAset) VALUES (@id, @barang, @ruang, @spec, @satuan, @idSemester, @tglMasuk, @statusAset)";
                                using (var cmdIn = new SqlCommand(qInsert, conn, transaction))
                                {
                                    cmdIn.Parameters.AddWithValue("@id", finalIdDetail);
                                    cmdIn.Parameters.AddWithValue("@barang", selectedIdBarang);
                                    cmdIn.Parameters.AddWithValue("@ruang", Convert.ToInt32(cmbRuangan.SelectedValue));
                                    cmdIn.Parameters.AddWithValue("@spec", txtSpesifikasi.Text.Trim());
                                    cmdIn.Parameters.AddWithValue("@satuan", satuanFinal);
                                    cmdIn.Parameters.AddWithValue("@idSemester", idSemesterMasuk);
                                    cmdIn.Parameters.AddWithValue("@tglMasuk", DateTime.Now);
                                    cmdIn.Parameters.AddWithValue("@statusAset", 1);
                                    cmdIn.ExecuteNonQuery();
                                }
                            }

                            string qStock = "UPDATE master.barang SET stok = stok + @totalLoop WHERE idBarang = @barang";
                            using (var cmdStok = new SqlCommand(qStock, conn, transaction))
                            {
                                cmdStok.Parameters.AddWithValue("@totalLoop", jumlahLoop);
                                cmdStok.Parameters.AddWithValue("@barang", selectedIdBarang);
                                cmdStok.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Proses Sinkronisasi Sukses Berjalan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataDetail();
                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}