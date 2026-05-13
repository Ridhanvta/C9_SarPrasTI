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
        private string selectedIdPermintaan = "";
        private string idBarangLama = "";
        private int jumlahLama = 0;

        public permintaanBarang()
        {
            InitializeComponent();

            this.txtIdBarang.ReadOnly = true;
            this.txtIdBarang.BackColor = SystemColors.Control;

            this.Load += new EventHandler(permintaanBarang_Load);
            this.addPeminta.Click += new EventHandler(btnSimpan_Click);
            this.updatePminjam.Click += new EventHandler(btnReset_Click);
            this.hpsPermintaan.Click += new EventHandler(btnHapus_Click);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            this.dataGridView2.CellClick += new DataGridViewCellEventHandler(dataGridView2_CellClick);

            this.textBox2.TextChanged += new EventHandler(textBox2_TextChanged);
            this.textBox1.TextChanged += new EventHandler(textBox1_TextChanged);

            this.txtNma.KeyPress += new KeyPressEventHandler(txtNma_KeyPress);
            this.txtJmlh.KeyPress += new KeyPressEventHandler(txtJmlh_KeyPress);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void permintaanBarang_Load(object sender, EventArgs e)
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

            LoadComboRuangan();
            LoadComboSemester();
            RefreshSemuaTabel();
            ResetForm();
        }

        private void txtNma_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtJmlh_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LoadComboRuangan()
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    string query = "SELECT idRuangan, namaRuangan FROM [master].[ruangan]";
                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbRuangan.DataSource = dt;
                        cmbRuangan.DisplayMember = "namaRuangan";
                        cmbRuangan.ValueMember = "idRuangan";
                        cmbRuangan.DropDownStyle = ComboBoxStyle.DropDownList;
                        cmbRuangan.SelectedIndex = -1;
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

        private void LoadDataBarang(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = "SELECT idBarang AS [ID], namaBarang AS [Nama], stok AS [Stok] FROM [master].[barang] WHERE tipeBarang = 0";

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
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Barang", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadDataTransaksi(string keyword = "")
        {
            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = @"
                                   SELECT * FROM [dbo].[vwDataTransaksi]";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " AND (b.namaBarang LIKE @kw OR p.namaPeminta LIKE @kw OR r.namaRuangan LIKE @kw OR p.idPermintaanBarang LIKE @kw)";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView2.DataSource = dt;

                            if (dataGridView2.Columns["idRuangan"] != null) dataGridView2.Columns["idRuangan"].Visible = false;
                            if (dataGridView2.Columns["idSemester"] != null) dataGridView2.Columns["idSemester"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Transaksi", MessageBoxButtons.OK, MessageBoxIcon.Error); }
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
            txtIdBarang.Clear();
            cmbRuangan.SelectedIndex = -1;
            txtNma.Clear();
            txtJmlh.Clear();
            cmbSemester.SelectedIndex = -1;

            selectedIdPermintaan = "";
            idBarangLama = "";
            jumlahLama = 0;

            addPeminta.Text = "Tambah Permintaan";
            updatePminjam.Text = "Reset Form";
            hpsPermintaan.Enabled = false;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
            textBox2.Clear();
            textBox1.Clear();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                txtIdBarang.Text = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value.ToString();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                selectedIdPermintaan = row.Cells["ID"].Value.ToString();
                txtIdBarang.Text = row.Cells["ID Barang"].Value.ToString();
                idBarangLama = row.Cells["ID Barang"].Value.ToString();

                cmbRuangan.SelectedValue = Convert.ToInt32(row.Cells["idRuangan"].Value);
                txtNma.Text = row.Cells["Peminta"].Value.ToString();

                txtJmlh.Text = row.Cells["Qty"].Value.ToString();
                jumlahLama = Convert.ToInt32(row.Cells["Qty"].Value);

                cmbSemester.SelectedValue = Convert.ToInt32(row.Cells["idSemester"].Value);

                addPeminta.Text = "Update Permintaan";
                hpsPermintaan.Enabled = true;
            }
        }

        private int CekStokAktual(SqlConnection conn, SqlTransaction trans, string idBarang)
        {
            string query = "SELECT stok FROM [master].[barang] WHERE idBarang = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@id", idBarang);
                object result = cmd.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private void UpdateStokSistem(SqlConnection conn, SqlTransaction trans, string idBarang, int perbedaan)
        {
            string query = "UPDATE [master].[barang] SET stok = stok + @diff WHERE idBarang = @id";
            using (SqlCommand cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@diff", perbedaan);
                cmd.Parameters.AddWithValue("@id", idBarang);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdBarang.Text) || string.IsNullOrWhiteSpace(txtNma.Text) ||
                string.IsNullOrWhiteSpace(txtJmlh.Text) || cmbRuangan.SelectedValue == null || cmbSemester.SelectedValue == null)
            {
                MessageBox.Show("Seluruh form wajib diisi secara lengkap.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtJmlh.Text.Trim(), out int jumlahBaru) || jumlahBaru <= 0)
            {
                MessageBox.Show("Jumlah permintaan harus berupa angka lebih dari 0.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        bool isEdit = !string.IsNullOrEmpty(selectedIdPermintaan);
                        string idBarangBaru = txtIdBarang.Text.Trim();

                        if (!isEdit)
                        {
                            int stokTersedia = CekStokAktual(conn, transaction, idBarangBaru);
                            if (stokTersedia < jumlahBaru)
                            {
                                throw new Exception($"Stok tidak mencukupi! Sisa stok aktual: {stokTersedia}");
                            }

                            string qInsert = "INSERT INTO [transaction].[permintaanBarang] (idBarang, idRuangan, namaPeminta, jumlah, tglPermintaan, idSemester) VALUES (@idB, @idR, @nama, @jml, GETDATE(), @smt)";
                            using (SqlCommand cmd = new SqlCommand(qInsert, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idB", idBarangBaru);
                                cmd.Parameters.AddWithValue("@idR", cmbRuangan.SelectedValue);
                                cmd.Parameters.AddWithValue("@nama", txtNma.Text.Trim());
                                cmd.Parameters.AddWithValue("@jml", jumlahBaru);
                                cmd.Parameters.AddWithValue("@smt", cmbSemester.SelectedValue);
                                cmd.ExecuteNonQuery();
                            }

                            UpdateStokSistem(conn, transaction, idBarangBaru, -jumlahBaru);
                        }
                        else
                        {
                            if (idBarangBaru == idBarangLama)
                            {
                                int selisih = jumlahBaru - jumlahLama;
                                if (selisih > 0)
                                {
                                    int stokTersedia = CekStokAktual(conn, transaction, idBarangBaru);
                                    if (stokTersedia < selisih)
                                    {
                                        throw new Exception($"Penambahan permintaan ditolak. Kekurangan stok: {selisih - stokTersedia} unit.");
                                    }
                                }
                                UpdateStokSistem(conn, transaction, idBarangBaru, -selisih);
                            }
                            else
                            {
                                int stokTersedia = CekStokAktual(conn, transaction, idBarangBaru);
                                if (stokTersedia < jumlahBaru)
                                {
                                    throw new Exception($"Barang pengganti tidak memiliki stok yang cukup! Sisa stok: {stokTersedia}");
                                }

                                UpdateStokSistem(conn, transaction, idBarangLama, jumlahLama);
                                UpdateStokSistem(conn, transaction, idBarangBaru, -jumlahBaru);
                            }

                            string qUpdate = "UPDATE [transaction].[permintaanBarang] SET idBarang=@idB, idRuangan=@idR, namaPeminta=@nama, jumlah=@jml, idSemester=@smt WHERE idPermintaanBarang=@idPB";
                            using (SqlCommand cmd = new SqlCommand(qUpdate, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idB", idBarangBaru);
                                cmd.Parameters.AddWithValue("@idR", cmbRuangan.SelectedValue);
                                cmd.Parameters.AddWithValue("@nama", txtNma.Text.Trim());
                                cmd.Parameters.AddWithValue("@jml", jumlahBaru);
                                cmd.Parameters.AddWithValue("@smt", cmbSemester.SelectedValue);
                                cmd.Parameters.AddWithValue("@idPB", selectedIdPermintaan);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show($"Transaksi permintaan berhasil diproses secara atomik!", "Operasi Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            if (string.IsNullOrEmpty(selectedIdPermintaan)) return;

            if (MessageBox.Show("Menghapus data permintaan ini akan mengembalikan stok barang ke dalam Master Katalog. Lanjutkan?", "Konfirmasi Destruktif", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;

                        SqlTransaction transaction = conn.BeginTransaction();

                        try
                        {
                            UpdateStokSistem(conn, transaction, idBarangLama, jumlahLama);

                            string qDelete = "DELETE FROM [transaction].[permintaanBarang] WHERE idPermintaanBarang=@idPB";
                            using (SqlCommand cmd = new SqlCommand(qDelete, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@idPB", selectedIdPermintaan);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Data permintaan dihapus dan stok telah dipulihkan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            RefreshSemuaTabel();
                            ResetForm();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Gagal menghapus: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Fatal Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void label4_Click(object sender, EventArgs e) { }
        private void stockBarang_Click(object sender, EventArgs e) { }
    }
}