using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class maintenancePage : Form
    {
        private string selectedIdMaintenance = "";
        private string selectedIdDetailBarang = "";
        private string selectedIdBarang = "";
        private int kondisiLama = -1;

        public maintenancePage()
        {
            InitializeComponent();

            this.cmbRuangan.Enabled = false;
            this.cmbDetailBarang.Enabled = false;

            this.Load += new EventHandler(maintenancePage_Load);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            this.dataGridView2.CellClick += new DataGridViewCellEventHandler(dataGridView2_CellClick);

            this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);
            this.textBox2.TextChanged += new EventHandler(textBox2_TextChanged);

            this.btnSimpan.Click += new EventHandler(btnSimpan_Click);
            this.btnHapus.Click += new EventHandler(btnHapus_Click);
            this.btnReset.Click += new EventHandler(btnReset_Click);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void maintenancePage_Load(object sender, EventArgs e)
        {
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is DateTimePicker dtp)
                {
                    dtp.MinDate = DateTime.Today;
                    dtp.MaxDate = DateTime.Today;
                    dtp.Value = DateTime.Today;
                }
            }

            LoadComboSemester();
            LoadComboKaryawan();
            RefreshSemuaTabel();
            ResetForm();
        }

        private void LoadComboKaryawan()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idKaryawan, namaKaryawan FROM [master].[karyawan]";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbKaryawan.DataSource = dt;
                        cmbKaryawan.DisplayMember = "namaKaryawan";
                        cmbKaryawan.ValueMember = "idKaryawan";
                        cmbKaryawan.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbKaryawan.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadComboSemester()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = "SELECT idSemester, tahunAjaran FROM [master].[semester] WHERE tahunAjaran LIKE '%' + CAST(YEAR(GETDATE()) AS VARCHAR) + '%'";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbSemester.DataSource = dt;
                        cmbSemester.DisplayMember = "tahunAjaran";
                        cmbSemester.ValueMember = "idSemester";
                        cmbSemester.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbSemester.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BindBarangControls()
        {
            txtKerusakan.DataBindings.Clear();
            txtTindakLanjut.DataBindings.Clear();
            cmbDetailBarang.DataBindings.Clear();
            cmbKaryawan.DataBindings.Clear();
            cmbRuangan.DataBindings.Clear();
            cmbSemester.DataBindings.Clear();

            txtKerusakan.DataBindings.Add("Text", bsMaintenance, "Kerusakan Barang");
            txtTindakLanjut.DataBindings.Add("Text", bsMaintenance, "Tindak Lanjut");

            cmbDetailBarang.DataBindings.Add("Text", bsMaintenance, "Aset");
            cmbRuangan.DataBindings.Add("Text", bsMaintenance, "Lokasi");

            cmbKaryawan.DataBindings.Add("SelectedValue", bsMaintenance, "idKaryawan");
            cmbSemester.DataBindings.Add("SelectedValue", bsMaintenance, "idSemester");
        }

        private BindingSource bsBarang = new BindingSource();
        private DataTable dtBarang = new DataTable();
        private void LoadDataBarang(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                        SELECT * FROM [dbo].[vwDetailBarangAsset]";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE b.namaBarang LIKE @kw OR db.spesifikasi LIKE @kw OR r.namaRuangan LIKE @kw OR db.idDetailBarang LIKE @kw";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtBarang = new DataTable();
                            da.Fill(dtBarang);

                            bsBarang.DataSource = dtBarang;

                            dataGridView2.DataSource = bsBarang;
                            bindingNavigator1.BindingSource = bsBarang;

                            // buat nyembunyiin ID yang nggak perlu diliat user tapi penting buat kodingan
                            if (dataGridView2.Columns["idBarang"] != null)
                                dataGridView2.Columns["idBarang"].Visible = false;

                            BindBarangControls();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Barang", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }


        private BindingSource bsMaintenance = new BindingSource();
        private DataTable dtMaint = new DataTable();
        private void LoadDataRiwayat(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                        SELECT * FROM [dbo].[vwMaintenanceHistory]";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        dtMaint = new DataTable();
                        da.Fill(dtMaint);
                        bsMaintenance.DataSource = dtMaint;
                        dataGridView1.DataSource = bsMaintenance;
                        bindingNavigator1.BindingSource = bsMaintenance;

                        txtKerusakan.DataBindings.Clear();
                        txtKerusakan.DataBindings.Add("Text", bsMaintenance, "kerusakan");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Riwayat", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        public void RefreshSemuaTabel()
        {
            LoadDataBarang(textBox1.Text.Trim());
            LoadDataRiwayat(textBox2.Text.Trim());
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            LoadDataBarang(textBox1.Text.Trim());
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            LoadDataRiwayat(textBox2.Text.Trim());
        }

        private void ResetForm()
        {
            selectedIdMaintenance = "";
            selectedIdDetailBarang = "";
            selectedIdBarang = "";
            kondisiLama = -1;

            cmbRuangan.Text = "";
            cmbDetailBarang.Text = "";

            cmbKaryawan.SelectedIndex = -1;
            cmbSemester.SelectedIndex = -1;

            txtKerusakan.Clear();
            txtTindakLanjut.Clear();
            rbBaik.Checked = true;

            btnSimpan.Text = "Simpan";
            btnHapus.Enabled = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
            textBox1.Clear();
            textBox2.Clear();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                selectedIdDetailBarang = row.Cells["ID Detail"].Value.ToString();
                selectedIdBarang = row.Cells["idBarang"].Value.ToString();
                kondisiLama = -1;

                cmbDetailBarang.Text = row.Cells["Aset"].Value.ToString();
                cmbRuangan.Text = row.Cells["Lokasi"].Value.ToString();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedIdMaintenance = row.Cells["ID"].Value.ToString();
                selectedIdDetailBarang = row.Cells["idDetailBarang"].Value.ToString();
                selectedIdBarang = row.Cells["idBarang"].Value.ToString();
                kondisiLama = row.Cells["Status"].Value.ToString() == "Baik" ? 1 : 0;

                cmbDetailBarang.Text = row.Cells["Aset"].Value.ToString();
                cmbRuangan.Text = row.Cells["Lokasi"].Value.ToString();

                dtpTglCek.Value = Convert.ToDateTime(row.Cells["Tgl Cek"].Value);

                cmbKaryawan.SelectedValue = Convert.ToInt32(row.Cells["idKaryawan"].Value);
                cmbSemester.SelectedValue = Convert.ToInt32(row.Cells["idSemester"].Value);

                txtKerusakan.Text = row.Cells["Kerusakan"].Value.ToString();
                txtTindakLanjut.Text = row.Cells["Tindak Lanjut"].Value.ToString();

                if (kondisiLama == 1) rbBaik.Checked = true;
                else rbRusak.Checked = true;

                btnSimpan.Text = "Update";
                btnHapus.Enabled = true;
            }
        }

        private void ExecuteStockUpdate(SqlConnection conn, SqlTransaction trans, string idB, int diff)
        {
            if (diff < 0)
            {
                string checkQuery = "SELECT stok FROM [master].[barang] WHERE idBarang = @id";
                using (SqlCommand cCmd = new SqlCommand(checkQuery, conn, trans))
                {
                    cCmd.Parameters.AddWithValue("@id", idB);
                    int currentStok = Convert.ToInt32(cCmd.ExecuteScalar());
                    if (currentStok < Math.Abs(diff))
                    {
                        throw new Exception("Stok Master tidak mencukupi untuk memproses penyusutan akibat kerusakan aset.");
                    }
                }
            }

            string updateQuery = "UPDATE [master].[barang] SET stok = stok + @diff WHERE idBarang = @idB";
            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, trans))
            {
                cmd.Parameters.AddWithValue("@diff", diff);
                cmd.Parameters.AddWithValue("@idB", idB);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedIdDetailBarang) || cmbKaryawan.SelectedValue == null || cmbSemester.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih Data Aset dari tabel di samping kanan, lalu lengkapi Petugas dan Semester.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int kondisiBaru = rbBaik.Checked ? 1 : 0;

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    SqlTransaction transaction = conn.BeginTransaction();

                    try
                    {
                        bool isEdit = !string.IsNullOrEmpty(selectedIdMaintenance);
                        string query;

                        if (isEdit)
                        {
                            query = @"UPDATE [transaction].[maintenance] SET 
                                      idKaryawan=@idK, idDetailBarang=@idD, tglCek=@tgl, 
                                      kondisi=@kon, kerusakan=@ker, tindakLanjut=@tin, idSemester=@smt 
                                      WHERE idMaintenance=@idM";

                            if (kondisiLama == 1 && kondisiBaru == 0) ExecuteStockUpdate(conn, transaction, selectedIdBarang, -1);
                            else if (kondisiLama == 0 && kondisiBaru == 1) ExecuteStockUpdate(conn, transaction, selectedIdBarang, 1);
                        }
                        else
                        {
                            query = @"INSERT INTO [transaction].[maintenance] 
                                      (idKaryawan, idDetailBarang, tglCek, kondisi, kerusakan, tindakLanjut, idSemester) 
                                      VALUES (@idK, @idD, @tgl, @kon, @ker, @tin, @smt)";

                            if (kondisiBaru == 0) ExecuteStockUpdate(conn, transaction, selectedIdBarang, -1);
                        }

                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@idK", cmbKaryawan.SelectedValue);
                            cmd.Parameters.AddWithValue("@idD", selectedIdDetailBarang);
                            cmd.Parameters.AddWithValue("@tgl", dtpTglCek.Value.Date);
                            cmd.Parameters.AddWithValue("@kon", kondisiBaru);
                            cmd.Parameters.AddWithValue("@ker", rbBaik.Checked ? "-" : txtKerusakan.Text.Trim());
                            cmd.Parameters.AddWithValue("@tin", txtTindakLanjut.Text.Trim());
                            cmd.Parameters.AddWithValue("@smt", cmbSemester.SelectedValue);

                            if (isEdit) cmd.Parameters.AddWithValue("@idM", selectedIdMaintenance);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        MessageBox.Show($"Log Maintenance berhasil {(isEdit ? "diperbarui" : "disimpan")} dan stok telah disinkronisasi!", "Operasi Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RefreshSemuaTabel();
                        ResetForm();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show(ex.Message, "Integritas Sistem Menolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Fatal Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedIdMaintenance)) return;

            if (MessageBox.Show("Anda yakin ingin menghapus log maintenance ini secara permanen?", "Konfirmasi Destruktif", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;

                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            if (kondisiLama == 0) ExecuteStockUpdate(conn, transaction, selectedIdBarang, 1);

                            string query = "DELETE FROM [transaction].[maintenance] WHERE idMaintenance=@idM";
                            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idM", selectedIdMaintenance);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Log Maintenance berhasil dihapus dan pemulihan stok telah disinkronisasi.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            RefreshSemuaTabel();
                            ResetForm();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show(ex.Message, "Integritas Sistem Menolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Gagal menghapus: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }
    }
}