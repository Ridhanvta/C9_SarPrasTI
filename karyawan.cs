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
        }

        private void karyawan_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'satprasDBDataSet.karyawan' table. You can move, or remove it, as needed.
            this.karyawanTableAdapter.Fill(this.satprasDBDataSet.karyawan);
            LoadDataKaryawan();
            ResetForm();
        }

        private void LoadDataKaryawan(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    DataTable dt = new DataTable();

                    if (string.IsNullOrEmpty(keyword))
                    {
                        // Menggunakan VIEW
                        string query = "SELECT * FROM master.vw_DataKaryawan";
                        SqlDataAdapter da = new SqlDataAdapter(query, conn);
                        da.Fill(dt);
                    }
                    else
                    {
                        // Menggunakan STORED PROCEDURE SEARCH
                        SqlCommand cmd = new SqlCommand("master.sp_ManageKaryawan", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "SEARCH");
                        cmd.Parameters.AddWithValue("@namaKaryawan", keyword);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                    // KRUSIAL: Sinkronisasi Binding
                    richTextBox3.DataBindings.Clear(); // Hapus binding lama agar tidak blank
                    bindingSource1.DataSource = dt;

                    bindingNavigator1.BindingSource = bindingSource1;
                    dataGridView1.DataSource = bindingSource1;

                    // Pasang Binding Otomatis ke TextBox
                    richTextBox3.DataBindings.Add("Text", bindingSource1, "Nama Karyawan", true, DataSourceUpdateMode.OnPropertyChanged);

                    if (dataGridView1.Columns["ID"] != null) dataGridView1.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Load: " + ex.Message); }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Biarkan BindingSource mengatur posisi, TextBox akan update otomatis
                bindingSource1.Position = e.RowIndex;

                DataRowView currentRow = (DataRowView)bindingSource1.Current;
                originalIdKaryawan = currentRow["ID"].ToString();

                isEditMode = true;
                btnTambah.Text = "Update Data";
                btnDelete.Enabled = true;
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            // Validasi: Pastikan data di UI sudah terserap ke BindingSource
            bindingSource1.EndEdit();

            string inputNama = richTextBox3.Text.Trim();
            if (string.IsNullOrEmpty(inputNama))
            {
                MessageBox.Show("Nama tidak boleh kosong!");
                return;
            }

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    using (SqlCommand cmd = new SqlCommand("master.sp_ManageKaryawan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", isEditMode ? "UPDATE" : "INSERT");
                        cmd.Parameters.AddWithValue("@namaKaryawan", inputNama);
                        if (isEditMode) cmd.Parameters.AddWithValue("@idKaryawan", originalIdKaryawan);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Berhasil Simpan/Update!");

                        LoadDataKaryawan(); // Refresh Binding
                        ResetForm();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Database: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalIdKaryawan)) return;

            if (MessageBox.Show("Hapus data ini?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;
                        using (SqlCommand cmd = new SqlCommand("master.sp_ManageKaryawan", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "DELETE");
                            cmd.Parameters.AddWithValue("@idKaryawan", originalIdKaryawan);
                            cmd.ExecuteNonQuery();
                            LoadDataKaryawan();
                            ResetForm();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) MessageBox.Show("Data tidak bisa dihapus karena masih digunakan di tabel lain!");
                }
            }
        }

        private void ResetForm()
        {
            isEditMode = false;
            originalIdKaryawan = "";
            btnTambah.Text = "Simpan Data Baru";
            btnDelete.Enabled = false;
            richTextBox3.DataBindings.Clear(); // Bersihkan binding saat reset
            richTextBox3.Clear();
            LoadDataKaryawan(); // Pasang kembali binding setelah bersih
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataKaryawan(richTextBox1.Text.Trim());
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            new dashboardPage().Show();
            this.Hide();
        }
    }
}