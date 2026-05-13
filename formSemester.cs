using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class formSemester : Form
    {
        private bool isEditMode = false;
        private string originalIdSemester = "";

        public formSemester()
        {
            InitializeComponent();

            // Mempertahankan struktur Hardcode Event Binding sesuai permintaan Anda
            this.Load += new EventHandler(formSemester_Load);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellClick);
            this.txtCari.TextChanged += new EventHandler(txtCari_TextChanged);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void formSemester_Load(object sender, EventArgs e)
        {
            LoadDataSemester();
            ResetForm();
        }

        private void LoadDataSemester(string keyword = "")
        {
            try
            {
                using (SqlConnection conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    DataTable dt = new DataTable();

                    // Menggunakan VIEW & Stored Procedure sesuai ketentuan
                    if (string.IsNullOrEmpty(keyword))
                    {
                        string query = "SELECT * FROM master.vw_DataSemester";
                        SqlDataAdapter da = new SqlDataAdapter(query, conn);
                        da.Fill(dt);
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("master.sp_ManageSemester", conn);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", "SEARCH");
                        cmd.Parameters.AddWithValue("@tahunAjaran", keyword);
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }

                    // --- SOLUSI OTOMATISASI BINDING ---

                    // 1. Bersihkan binding lama agar tidak terjadi tabrakan data
                    txtTahunAjaran.DataBindings.Clear();

                    // 2. Isi data ke BindingSource
                    bindingSource1.DataSource = dt;

                    // 3. Sinkronkan kontrol Navigator dan Grid ke satu sumber
                    bindingNavigator1.BindingSource = bindingSource1;
                    dataGridView1.DataSource = bindingSource1;

                    // 4. Lakukan Binding ke TextBox secara eksplisit
                    // Pastikan nama kolom "[Tahun Ajaran]" sesuai dengan VIEW di SQL
                    txtTahunAjaran.DataBindings.Add("Text", bindingSource1, "Tahun Ajaran", true, DataSourceUpdateMode.OnPropertyChanged);

                    // 5. PAKSA REFRESH: Agar data baris pertama langsung muncul di TextBox saat Load
                    if (dt.Rows.Count > 0)
                    {
                        bindingSource1.MoveFirst();
                        // Ambil ID baris pertama untuk state awal jika diperlukan
                        DataRowView firstRow = (DataRowView)bindingSource1.Current;
                        originalIdSemester = firstRow["ID"].ToString();
                    }

                    if (dataGridView1.Columns["ID"] != null) dataGridView1.Columns["ID"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error Load Data: " + ex.Message); }
        }

        private void ResetForm()
        {
            // Jangan bersihkan binding di sini agar teks tetap muncul saat navigasi
            isEditMode = false;
            originalIdSemester = "";

            btnTambah.Text = "Tambah";
            btnUpdate.Text = "Reset Text";
            btnDelete.Enabled = false;

            // Cukup kosongkan teks jika ingin input data baru, 
            // namun ingat ini akan memutus binding sementara jika tidak hati-hati
            txtTahunAjaran.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ResetForm();
            LoadDataSemester(); // Reload untuk mengaktifkan kembali binding otomatis
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataSemester(txtCari.Text.Trim());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Biarkan BindingSource mengatur posisi, TextBox akan mengikuti otomatis
                bindingSource1.Position = e.RowIndex;

                DataRowView currentRow = (DataRowView)bindingSource1.Current;
                originalIdSemester = currentRow["ID"].ToString();

                isEditMode = true;
                btnTambah.Text = "Update Data";
                btnDelete.Enabled = true;
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            // Simpan perubahan dari UI ke BindingSource sebelum ke database
            bindingSource1.EndEdit();

            string inputTahun = txtTahunAjaran.Text.Trim();
            if (string.IsNullOrEmpty(inputTahun)) return;

            try
            {
                using (var conn = DatabaseConfig.GetConnection())
                {
                    if (conn == null) return;
                    using (SqlCommand cmd = new SqlCommand("master.sp_ManageSemester", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Action", isEditMode ? "UPDATE" : "INSERT");
                        cmd.Parameters.AddWithValue("@tahunAjaran", inputTahun);
                        if (isEditMode) cmd.Parameters.AddWithValue("@idSemester", originalIdSemester);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Operasi Berhasil!");

                        LoadDataSemester();
                        ResetForm();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(originalIdSemester)) return;

            if (MessageBox.Show("Hapus data?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    using (var conn = DatabaseConfig.GetConnection())
                    {
                        if (conn == null) return;
                        using (SqlCommand cmd = new SqlCommand("master.sp_ManageSemester", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Action", "DELETE");
                            cmd.Parameters.AddWithValue("@idSemester", originalIdSemester);
                            cmd.ExecuteNonQuery();

                            LoadDataSemester();
                            ResetForm();
                        }
                    }
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) MessageBox.Show("Data masih digunakan di tabel lain!");
                }
            }
        }
    }
}