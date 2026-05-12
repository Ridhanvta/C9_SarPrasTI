using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class karyawan : Form
    {
        private bool isEditMode = false;
        private string originalIdKaryawan = "";

        public karyawan()
        {
            InitializeComponent();
            this.richTextBox3.KeyPress += new KeyPressEventHandler(richTextBox3_KeyPress);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void karyawan_Load(object sender, EventArgs e)
        {
            LoadDataKaryawan();
            ResetForm();
        }

        private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void LoadDataKaryawan(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string query = "SELECT idKaryawan as ID, namaKaryawan as [Nama Karyawan] FROM master.karyawan";
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        query += " WHERE namaKaryawan LIKE @kw";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(keyword)) cmd.Parameters.AddWithValue("@kw", "%" + keyword + "%");

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                            if (dataGridView1.Columns["ID"] != null) dataGridView1.Columns["ID"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Load Data: " + ex.Message); }
        }

        private void ResetForm()
        {
            richTextBox3.Clear();

            isEditMode = false;
            originalIdKaryawan = "";

            btnTambah.Text = "Simpan Data Baru";
            btnUpdate.Text = "Reset Text";

            btnDelete.Enabled = false;
            richTextBox3.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataKaryawan(richTextBox1.Text.Trim());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                originalIdKaryawan = row.Cells["ID"].Value.ToString();
                richTextBox3.Text = row.Cells["Nama Karyawan"].Value.ToString();

                isEditMode = true;

                btnTambah.Text = "Update Data";
                btnDelete.Enabled = true;
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            string inputNama = richTextBox3.Text.Trim();

            if (string.IsNullOrEmpty(inputNama))
            {
                MessageBox.Show("Nama Karyawan tidak boleh kosong.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                richTextBox3.Focus();
                return;
            }

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;

                    string checkQuery = isEditMode
                        ? "SELECT COUNT(1) FROM master.karyawan WHERE namaKaryawan = @nama AND idKaryawan != @id"
                        : "SELECT COUNT(1) FROM master.karyawan WHERE namaKaryawan = @nama";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@nama", inputNama);
                        if (isEditMode) checkCmd.Parameters.AddWithValue("@id", originalIdKaryawan);

                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) > 0)
                        {
                            MessageBox.Show("Karyawan dengan nama ini sudah terdaftar di sistem!", "Duplikasi Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    string query = isEditMode
                        ? "UPDATE master.karyawan SET namaKaryawan = @nama WHERE idKaryawan = @id"
                        : "INSERT INTO master.karyawan (namaKaryawan) VALUES (@nama)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nama", inputNama);
                        if (isEditMode) cmd.Parameters.AddWithValue("@id", originalIdKaryawan);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show($"Data karyawan berhasil {(isEditMode ? "diperbarui" : "ditambahkan")}!", "Operasi Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        LoadDataKaryawan();
                        ResetForm();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Database Error: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalIdKaryawan)) return;

            if (MessageBox.Show($"Anda yakin ingin menghapus Karyawan '{richTextBox3.Text}' dari sistem?", "Konfirmasi Pemberhentian", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;
                        string deleteQuery = "DELETE FROM master.karyawan WHERE idKaryawan = @id";
                        using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@id", originalIdKaryawan);
                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Data karyawan berhasil dihapus.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadDataKaryawan();
                            ResetForm();
                        }
                    }
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        MessageBox.Show("PEMBERHENTIAN DITOLAK!\n\nKaryawan ini masih tercatat sebagai penanggung jawab dalam riwayat Pengecekan Rutin/Maintenance. Data karyawan yang memiliki jejak historis operasional tidak boleh dihapus dari sistem.", "Integritas Data Aktif", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else MessageBox.Show("Error Database: " + sqlEx.Message);
                }
            }
        }
    }
}