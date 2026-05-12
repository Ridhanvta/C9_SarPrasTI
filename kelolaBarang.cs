using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class kelolaBarang : Form
    {
        private bool isEditMode = false;
        private string originalIdBarang = "";

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
            this.btnHapus.Click += new EventHandler(btnHapus_Click);
            this.button3.Click += new EventHandler(button3_Click);
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

            LoadComboMerk();
            LoadAutoCompleteNamaBarang();
            LoadDataBarang();
            ResetForm();
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
                            dataGridView1.DataSource = dt;

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
            txtIdBarang.Clear();
            txtNamaBarang.Clear();
            cmbMerk.SelectedIndex = -1;
            cmbMerk.Text = "";
            txtJumlahBarang.Clear();
            cmbTipeBarang.SelectedIndex = -1;

            txtIdBarang.ReadOnly = false;
            txtJumlahBarang.ReadOnly = false;
            txtJumlahBarang.BackColor = SystemColors.Window;

            isEditMode = false;
            originalIdBarang = "";

            btnTambahBarang.Text = "Simpan";
            btnHapus.Enabled = false;
            txtIdBarang.Focus();
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
            }
            else
            {
                txtJumlahBarang.ReadOnly = false;
                txtJumlahBarang.BackColor = SystemColors.Window;

                if (!isEditMode && txtJumlahBarang.Text == "0")
                {
                    txtJumlahBarang.Clear();
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
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                originalIdBarang = row.Cells["ID/Kode Barang"].Value.ToString();
                txtIdBarang.Text = originalIdBarang;
                txtNamaBarang.Text = row.Cells["Nama Barang"].Value.ToString();
                cmbMerk.Text = row.Cells["Merk"].Value.ToString();
                txtJumlahBarang.Text = row.Cells["Sisa Stok"].Value.ToString();

                cmbTipeBarang.SelectedValue = Convert.ToInt32(row.Cells["tipeBarang"].Value);

                txtIdBarang.ReadOnly = true;
                isEditMode = true;

                btnTambahBarang.Text = "Update";
                btnHapus.Enabled = true;
            }
        }

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

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdBarang.Text) ||
                string.IsNullOrWhiteSpace(txtNamaBarang.Text) ||
                string.IsNullOrWhiteSpace(cmbMerk.Text) ||
                string.IsNullOrWhiteSpace(txtJumlahBarang.Text) ||
                cmbTipeBarang.SelectedValue == null)
            {
                MessageBox.Show("Seluruh form wajib diisi termasuk Merk.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tipeInput = Convert.ToInt32(cmbTipeBarang.SelectedValue);
            int stokInput = Convert.ToInt32(txtJumlahBarang.Text.Trim());

            if (tipeInput == 0 && stokInput <= 0 && !isEditMode)
            {
                MessageBox.Show("Untuk Barang Habis Pakai, stok awal harus lebih dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtJumlahBarang.Focus();
                return;
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

                        if (isEditMode)
                        {
                            query = "UPDATE master.barang SET namaBarang = @nama, idMerk = @idMerk, stok = @stok, tipeBarang = @tipe WHERE idBarang = @idAsli";
                        }
                        else
                        {
                            string checkQuery = "SELECT COUNT(1) FROM master.barang WHERE idBarang = @id";
                            using (var checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@id", txtIdBarang.Text.Trim());
                                if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                                {
                                    throw new Exception("ID Barang sudah digunakan!");
                                }
                            }
                            query = "INSERT INTO master.barang (idBarang, namaBarang, idMerk, stok, tipeBarang) VALUES (@id, @nama, @idMerk, @stok, @tipe)";
                        }

                        using (var cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@id", txtIdBarang.Text.Trim());
                            cmd.Parameters.AddWithValue("@nama", txtNamaBarang.Text.Trim());
                            cmd.Parameters.AddWithValue("@idMerk", resolvedIdMerk);
                            cmd.Parameters.AddWithValue("@stok", stokInput);
                            cmd.Parameters.AddWithValue("@tipe", tipeInput);

                            if (isEditMode) cmd.Parameters.AddWithValue("@idAsli", originalIdBarang);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"Data inventaris berhasil {(isEditMode ? "diperbarui" : "disimpan")}!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

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

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalIdBarang)) return;

            if (MessageBox.Show($"Apakah Anda yakin ingin memusnahkan '{txtNamaBarang.Text}' dari katalog sarpras?", "Konfirmasi Penghapusan", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;

                        string query = "DELETE FROM master.barang WHERE idBarang = @id";
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", originalIdBarang);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Aset berhasil dihapus dari sistem.", "Penghapusan Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoadComboMerk();
                            LoadAutoCompleteNamaBarang();
                            LoadDataBarang();
                            ResetForm();
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        MessageBox.Show("OPERASI DITOLAK: Integritas Data!\n\nBarang ini tidak dapat dihapus karena masih terikat dengan data Permintaan, Penempatan Ruangan, atau riwayat Maintenance.", "Proteksi Database Aktif", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Database Error: " + sqlEx.Message);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("System Error: " + ex.Message);
                }
            }
        }
    }
}