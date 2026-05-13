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
        private BindingSource bsBarang = new BindingSource();
        private bool isBindingApplied = false;

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

            if (this.Controls.ContainsKey("bindingNavigator1"))
            {
                BindingNavigator bn = (BindingNavigator)this.Controls["bindingNavigator1"];
                bn.BindingSource = bsBarang;
            }

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
                    string query = "SELECT DISTINCT namaBarang FROM master.vw_DataBarang";
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

                    string query = "SELECT * FROM master.vw_DataBarang";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE [Nama Barang] LIKE @kw OR [ID/Kode Barang] LIKE @kw OR [Merk] LIKE @kw";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            bsBarang.DataSource = dt;
                            dataGridView1.DataSource = bsBarang;

                            if (dataGridView1.Columns["tipeBarang"] != null)
                                dataGridView1.Columns["tipeBarang"].Visible = false;

                            if (dataGridView1.Columns["idMerk"] != null)
                                dataGridView1.Columns["idMerk"].Visible = false;

                            if (!isBindingApplied && dt.Rows.Count > 0)
                            {
                                ApplySmartBindings();
                                isBindingApplied = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ApplySmartBindings()
        {
            txtIdBarang.DataBindings.Clear();
            txtNamaBarang.DataBindings.Clear();
            txtJumlahBarang.DataBindings.Clear();
            cmbMerk.DataBindings.Clear();
            cmbTipeBarang.DataBindings.Clear();

            txtIdBarang.DataBindings.Add("Text", bsBarang, "ID/Kode Barang", true, DataSourceUpdateMode.Never);
            txtNamaBarang.DataBindings.Add("Text", bsBarang, "Nama Barang", true, DataSourceUpdateMode.Never);
            txtJumlahBarang.DataBindings.Add("Text", bsBarang, "Sisa Stok", true, DataSourceUpdateMode.Never);

            cmbMerk.DataBindings.Add("SelectedValue", bsBarang, "idMerk", true, DataSourceUpdateMode.Never);
            cmbTipeBarang.DataBindings.Add("SelectedValue", bsBarang, "tipeBarang", true, DataSourceUpdateMode.Never);
        }

        private void bsBarang_PositionChanged(object sender, EventArgs e)
        {
            if (!isBindingApplied || bsBarang.Position < 0 || bsBarang.Current == null) return;

            DataRowView row = (DataRowView)bsBarang.Current;
            originalIdBarang = row["ID/Kode Barang"].ToString();

            txtIdBarang.ReadOnly = true;
            isEditMode = true;

            btnTambahBarang.Text = "Update";
            btnHapus.Enabled = true;
        }

        private void dgvBarang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bsBarang_PositionChanged(sender, e);
            }
        }

        private void ResetForm()
        {
            bsBarang.SuspendBinding();

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

            bsBarang.ResumeBinding();
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

                        if (!isEditMode)
                        {
                            string checkQuery = "SELECT COUNT(1) FROM master.vw_DataBarang WHERE [ID/Kode Barang] = @id";
                            using (var checkCmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                checkCmd.Parameters.AddWithValue("@id", txtIdBarang.Text.Trim());
                                if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                                {
                                    throw new Exception("ID Barang sudah digunakan!");
                                }
                            }
                        }

                        string spName = isEditMode ? "master.sp_UpdateBarang" : "master.sp_InsertBarang";

                        using (var cmd = new SqlCommand(spName, conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@namaBarang", txtNamaBarang.Text.Trim());
                            cmd.Parameters.AddWithValue("@idMerk", resolvedIdMerk);
                            cmd.Parameters.AddWithValue("@stok", stokInput);
                            cmd.Parameters.AddWithValue("@tipeBarang", tipeInput);

                            if (isEditMode)
                            {
                                cmd.Parameters.AddWithValue("@idAsli", originalIdBarang);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@idBarang", txtIdBarang.Text.Trim());
                            }

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"Data inventaris berhasil {(isEditMode ? "diperbarui" : "disimpan")}!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isBindingApplied = false;
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

                        using (var cmd = new SqlCommand("master.sp_DeleteBarang", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@idBarang", originalIdBarang);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Aset berhasil dihapus dari sistem.", "Penghapusan Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            isBindingApplied = false;
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