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

                    string query = "SELECT * FROM [dbo].[vwDaftarRuangan]";
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

                    string query = "SELECT * FROM [dbo].[vwSemesterAktif]";

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        cmbSemester.DataSource = dt;
                        cmbSemester.DisplayMember = "tahunAjaran";
                        cmbSemester.ValueMember = "idSemester";
                        cmbSemester.DropDownStyle = ComboBoxStyle.DropDownList;

                        cmbSemester.Enabled = false; // disable karena semester otomatis di-generate berdasarkan tanggal laptop
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

                    string query = "SELECT * FROM [dbo].[vwKatalogBarang]";

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE ([Nama] LIKE @kw OR [ID] LIKE @kw)";
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

        private BindingSource navBindingSource = new BindingSource();
        private DataTable dtTransaksi = new DataTable();

        private void BindControls()
        {
            txtNma.DataBindings.Clear();
            txtJmlh.DataBindings.Clear();

            txtNma.DataBindings.Add("Text", navBindingSource, "Peminta");
            txtJmlh.DataBindings.Add("Text", navBindingSource, "Qty");
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
                        query += " WHERE [Barang] LIKE @kw OR [Peminta] LIKE @kw";
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(query, conn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@kw", "%" + keyword + "%");
                        dtTransaksi = new DataTable();
                        da.Fill(dtTransaksi);

                        navBindingSource.DataSource = dtTransaksi;
                        dataGridView2.DataSource = navBindingSource;
                        bindingNavigator1.BindingSource = navBindingSource;

                        BindControls();
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
            string query = "SELECT stok FROM [dbo].[vwStokAktual] WHERE idBarang = @id";

            using (SqlCommand cmd = new SqlCommand(query, conn, trans))
            {
                cmd.Parameters.AddWithValue("@id", idBarang);
                object result = cmd.ExecuteScalar();

                // mengembalikan 0 jika data tidak ditemukan
                return result != null ? Convert.ToInt32(result) : 0;
            }
        }

        private void UpdateStokSistem(SqlConnection conn, SqlTransaction trans, string idBarang, int perbedaan)
        {
            using (SqlCommand cmd = new SqlCommand("[master].[sp_UpdateStokBarang]", conn, trans))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id", idBarang);
                cmd.Parameters.AddWithValue("@diff", perbedaan);

                cmd.ExecuteNonQuery();
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            
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
                        conn.Open();

                        SqlTransaction transaction = conn.BeginTransaction();

                        using (SqlCommand cmd = new SqlCommand("sp_DeletePermintaanBarang", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@idPB", selectedIdPermintaan);

                            cmd.ExecuteNonQuery();
                        }

                        MessageBox.Show("Data permintaan dihapus dan stok telah dipulihkan secara otomatis!",
                        "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        RefreshSemuaTabel();
                        ResetForm();
                    }
                }
                catch (Exception ex) { MessageBox.Show("Fatal Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void label4_Click(object sender, EventArgs e) { }
        private void stockBarang_Click(object sender, EventArgs e) { }

        private int GetIdSemesterOtomatis(SqlConnection conn, SqlTransaction transaction)
        {
            // 1. Ambil waktu dari laptop
            DateTime sekarang = DateTime.Now;
            int bulan = sekarang.Month;
            int tahun = sekarang.Year;
            string targetSemester = "";

            // 2. Logika Kalender Kampus
            // Ganjil = September (9) s/d Januari (1)
            // Genap = Februari (2) s/d agustus (8)
            if (bulan >= 9 || bulan == 1)
            {
                int tahunAwal = (bulan == 1) ? tahun - 1 : tahun;
                targetSemester = $"{tahunAwal}/{tahunAwal + 1} Ganjil";
            }
            else
            {
                targetSemester = $"{tahun - 1}/{tahun} Genap";
            }

            // 3. Cari idSemester di database berdasarkan string yang dirakit
            string query = "SELECT idSemester FROM [master].[semester] WHERE tahunAjaran = @TA";
            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@TA", targetSemester);
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    return Convert.ToInt32(result);
                }
                else
                {
                    // Lempar error kalau kaprodi/admin master belum bikin master data semester ini
                    throw new Exception($"Tahun ajaran '{targetSemester}' belum terdaftar");
                }
            }
        }

        private void addPeminta_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIdBarang.Text) || string.IsNullOrWhiteSpace(txtNma.Text) ||
                string.IsNullOrWhiteSpace(txtJmlh.Text) || cmbRuangan.SelectedValue == null) // semester otomatis dari function diatas
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
                        int idSemesterAktif = GetIdSemesterOtomatis(conn, transaction);
                        bool isEdit = !string.IsNullOrEmpty(selectedIdPermintaan);
                        string idBarangBaru = txtIdBarang.Text.Trim();

                
                        using (SqlCommand cmd = new SqlCommand("sp_SavePermintaanBarang", conn, transaction))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            if (isEdit)
                                cmd.Parameters.AddWithValue("@idPB", selectedIdPermintaan);
                            else
                                cmd.Parameters.AddWithValue("@idPB", DBNull.Value);

                            cmd.Parameters.AddWithValue("@idB", txtIdBarang.Text.Trim());
                            cmd.Parameters.AddWithValue("@idR", cmbRuangan.SelectedValue);
                            cmd.Parameters.AddWithValue("@nama", txtNma.Text.Trim());
                            cmd.Parameters.AddWithValue("@jml", jumlahBaru);
                            cmd.Parameters.AddWithValue("@smt", idSemesterAktif);

                            cmd.ExecuteNonQuery();
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
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}