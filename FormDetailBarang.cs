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

        private BindingSource bsDetail = new BindingSource();
        private bool isBindingApplied = false;

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
            this.btnHapus.Click += new EventHandler(btnHapus_Click);

            this.bsDetail.PositionChanged += new EventHandler(bsDetail_PositionChanged);

            if (this.Controls.ContainsKey("btnTestSQL"))
                this.Controls["btnTestSQL"].Click += new EventHandler(btnTestSQL_Click);

            if (this.Controls.ContainsKey("btnRestoreData"))
                this.Controls["btnRestoreData"].Click += new EventHandler(btnRestoreData_Click);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            kelolaBarang masterForm = new kelolaBarang();
            masterForm.Show();
            this.Hide();
        }

        private void FormDetailBarang_Load(object sender, EventArgs e)
        {
            if (this.Controls.ContainsKey("bindingNavigator1"))
            {
                BindingNavigator bn = (BindingNavigator)this.Controls["bindingNavigator1"];
                bn.BindingSource = bsDetail;
            }

            LoadComboBoxBarang();
            LoadComboBoxGedung();
            LoadDataDetail();
            ResetForm();
        }

        private void btnTestSQL_Click(object sender, EventArgs e)
        {
            string payload = "Bocor%' OR 1=1 --";
            RunSQLInjectionDemo(payload);
        }

        private void btnRestoreData_Click(object sender, EventArgs e)
        {
            txtCari.Clear();
            LoadDataDetail();
            MessageBox.Show("Sistem kembali menggunakan arsitektur Parameterized Query yang 100% Kebal SQL Injection.", "Sistem Dipulihkan", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RunSQLInjectionDemo(string payload)
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string vulnerableQuery = "SELECT * FROM [transaction].vw_DataDetailBarang WHERE [Spesifikasi] LIKE '%" + payload + "%'";

                    using (var cmd = new SqlCommand(vulnerableQuery, conn))
                    {
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            dgvDetail.DataSource = null;
                            dgvDetail.Columns.Clear();
                            dgvDetail.DataSource = dt;

                            MessageBox.Show("Payload otomatis disuntikkan: " + payload + "\n\nQuery yang dieksekusi Server:\n" + vulnerableQuery, "SQL Injection Tembus", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server SQL memberikan respon Error:\n\n" + ex.Message, "SQL Injection Berhasil Mengganggu Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadComboBoxBarang()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idBarang, namaBarang FROM master.barang WHERE tipeBarang = 1";
                    using (var da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        DataRow dr = dt.NewRow();
                        dr["idBarang"] = "0";
                        dr["namaBarang"] = "-- Ketik atau Pilih Aset Tetap --";
                        dt.Rows.InsertAt(dr, 0);

                        cmbBarang.DataSource = dt;
                        cmbBarang.DisplayMember = "namaBarang";
                        cmbBarang.ValueMember = "idBarang";

                        cmbBarang.DropDownStyle = ComboBoxStyle.DropDown;
                        cmbBarang.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        cmbBarang.AutoCompleteSource = AutoCompleteSource.ListItems;
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

                    string query = "SELECT * FROM [transaction].vw_DataDetailBarang";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE [Nama Aset] LIKE @kw OR [ID Detail] LIKE @kw OR [Spesifikasi] LIKE @kw OR [Merk] LIKE @kw";
                    }

                    using (var cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (var da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            bsDetail.DataSource = dt;

                            dgvDetail.DataSource = null;
                            dgvDetail.Columns.Clear();
                            dgvDetail.AutoGenerateColumns = true;
                            dgvDetail.DataSource = bsDetail;

                            if (dgvDetail.Columns["idBarang"] != null) dgvDetail.Columns["idBarang"].Visible = false;
                            if (dgvDetail.Columns["idGedung"] != null) dgvDetail.Columns["idGedung"].Visible = false;
                            if (dgvDetail.Columns["idRuangan"] != null) dgvDetail.Columns["idRuangan"].Visible = false;

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
            txtIdDetail.DataBindings.Clear();
            txtSpesifikasi.DataBindings.Clear();
            cmbBarang.DataBindings.Clear();
            cmbGedung.DataBindings.Clear();
            cmbRuangan.DataBindings.Clear();

            txtIdDetail.DataBindings.Add("Text", bsDetail, "ID Detail", true, DataSourceUpdateMode.Never);
            txtSpesifikasi.DataBindings.Add("Text", bsDetail, "Spesifikasi", true, DataSourceUpdateMode.Never);

            cmbBarang.DataBindings.Add("SelectedValue", bsDetail, "idBarang", true, DataSourceUpdateMode.Never);
            cmbGedung.DataBindings.Add("SelectedValue", bsDetail, "idGedung", true, DataSourceUpdateMode.Never);

            Binding bindRuangan = new Binding("SelectedValue", bsDetail, "idRuangan", true, DataSourceUpdateMode.Never);
            bindRuangan.Format += (s, e) => { cmbRuangan.Enabled = true; };
            cmbRuangan.DataBindings.Add(bindRuangan);
        }

        private void bsDetail_PositionChanged(object sender, EventArgs e)
        {
            if (!isBindingApplied || bsDetail.Position < 0 || bsDetail.Current == null) return;

            DataRowView row = (DataRowView)bsDetail.Current;

            originalIdDetail = row["ID Detail"].ToString();
            originalIdBarang = row["idBarang"].ToString();

            txtIdDetail.ReadOnly = true;
            cmbBarang.Enabled = false;

            isEditMode = true;
            btnSimpan.Text = "Update Data";
            btnHapus.Enabled = true;
        }

        private void dgvDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bsDetail_PositionChanged(sender, e);
            }
        }

        private void ResetForm()
        {
            bsDetail.SuspendBinding();

            txtIdDetail.Clear();
            txtSpesifikasi.Clear();

            txtIdDetail.ReadOnly = false;

            cmbBarang.Enabled = true;
            if (cmbBarang.Items.Count > 0) cmbBarang.SelectedIndex = 0;

            if (cmbGedung.Items.Count > 0) cmbGedung.SelectedIndex = 0;

            cmbRuangan.DataSource = null;
            cmbRuangan.Items.Clear();
            cmbRuangan.Items.Add("-- Pilih Ruangan --");
            cmbRuangan.SelectedIndex = 0;
            cmbRuangan.Enabled = false;

            isEditMode = false;
            originalIdDetail = "";
            originalIdBarang = "";

            btnSimpan.Text = "Tambah Detail Barang";
            btnHapus.Enabled = false;
            txtIdDetail.Focus();

            bsDetail.ResumeBinding();
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataDetail(txtCari.Text.Trim());
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdDetail.Text) || string.IsNullOrWhiteSpace(txtSpesifikasi.Text) ||
                cmbBarang.SelectedIndex <= 0 || cmbGedung.SelectedIndex <= 0 || cmbRuangan.SelectedIndex <= 0)
            {
                MessageBox.Show("Lengkapi semua form!", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        if (!isEditMode)
                        {
                            string qCheck = "SELECT COUNT(1) FROM [transaction].vw_DataDetailBarang WHERE [ID Detail] = @id";
                            using (var cmdCheck = new SqlCommand(qCheck, conn, transaction))
                            {
                                cmdCheck.Parameters.AddWithValue("@id", txtIdDetail.Text.Trim());
                                if (Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0)
                                {
                                    throw new Exception("ID Detail sudah terpakai!");
                                }
                            }
                        }

                        string spName = isEditMode ? "[transaction].sp_UpdateDetailBarang" : "[transaction].sp_InsertDetailBarang";

                        using (var cmd = new SqlCommand(spName, conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (isEditMode)
                            {
                                cmd.Parameters.AddWithValue("@idAsli", originalIdDetail);
                                cmd.Parameters.AddWithValue("@idRuangan", Convert.ToInt32(cmbRuangan.SelectedValue));
                                cmd.Parameters.AddWithValue("@spesifikasi", txtSpesifikasi.Text.Trim());
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@idDetailBarang", txtIdDetail.Text.Trim());
                                cmd.Parameters.AddWithValue("@idBarang", cmbBarang.SelectedValue.ToString());
                                cmd.Parameters.AddWithValue("@idRuangan", Convert.ToInt32(cmbRuangan.SelectedValue));
                                cmd.Parameters.AddWithValue("@spesifikasi", txtSpesifikasi.Text.Trim());
                            }

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show("Proses berhasil!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        isBindingApplied = false;
                        LoadDataDetail();
                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalIdDetail)) return;

            if (MessageBox.Show("Yakin ingin menghapus?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;
                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            using (var cmd = new SqlCommand("[transaction].sp_DeleteDetailBarang", conn, transaction))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idDetailBarang", originalIdDetail);
                                cmd.Parameters.AddWithValue("@idBarang", originalIdBarang);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Data terhapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            isBindingApplied = false;
                            LoadDataDetail();
                            ResetForm();
                        }
                        catch (SqlException sqlEx)
                        {
                            transaction.Rollback();
                            if (sqlEx.Number == 547)
                                MessageBox.Show("Aset tidak bisa dihapus.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            else
                                throw sqlEx;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }
    }
}