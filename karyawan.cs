using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
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

            // Pasang satpam lapis pertama di sini (Event KeyPress)
            this.richTextBox3.KeyPress += new KeyPressEventHandler(richTextBox3_KeyPress);
        }

        private void karyawan_Load(object sender, EventArgs e)
        {
            // BARIS INI DIMATIKAN: Biar nggak nabrak koneksi kayak form semester kemaren
            // this.karyawanTableAdapter.Fill(this.satprasDBDataSet.karyawan);

            LoadDataKaryawan();
            ResetForm();
        }

        // ==========================================
        // SATPAM LAPIS 1: Mencegah ngetik angka/simbol
        // ==========================================
        private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Hanya izinkan huruf (IsLetter), spasi (IsWhiteSpace), dan tombol kontrol kayak Backspace (IsControl)
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
            {
                e.Handled = true; // Tolak inputan langsung!
            }
        }

        private void LoadDataKaryawan(string keyword = "")
        {
            try
            {
                DataTable dt = DAL.GetKaryawanGridData(keyword);

                // KRUSIAL: Sinkronisasi Binding
                richTextBox3.DataBindings.Clear(); // Hapus binding lama agar tidak blank
                bindingSource1.DataSource = dt;

                bindingNavigator1.BindingSource = bindingSource1;
                dataGridView1.DataSource = bindingSource1;

                // Pasang Binding Otomatis ke TextBox
                richTextBox3.DataBindings.Add("Text", bindingSource1, "Nama Karyawan", true, DataSourceUpdateMode.OnPropertyChanged);

                if (dataGridView1.Columns["ID"] != null) dataGridView1.Columns["ID"].Visible = false;
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
                MessageBox.Show("Nama tidak boleh kosong!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ==========================================
            // SATPAM LAPIS 2: Jaga-jaga kalau user nge-paste text aneh
            // Regex ini ngecek apakah string isinya cuma huruf (a-z, A-Z) dan spasi (\s)
            // ==========================================
            if (!Regex.IsMatch(inputNama, @"^[a-zA-Z\s]+$"))
            {
                MessageBox.Show("Nama tidak valid! Hanya boleh berisi huruf dan spasi.", "Input Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DAL.ManageKaryawan(isEditMode ? "UPDATE" : "INSERT", originalIdKaryawan, inputNama);
                MessageBox.Show("Berhasil Simpan/Update!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataKaryawan(); // Refresh Binding
                ResetForm();
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
                    DAL.ManageKaryawan("DELETE", originalIdKaryawan, "");
                    LoadDataKaryawan();
                    ResetForm();
                }
                catch (SqlException ex)
                {
                    if (ex.Number == 547) MessageBox.Show("Data tidak bisa dihapus karena masih digunakan di tabel lain!", "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            // Note: Aku biarin pakai richTextBox1 sesuai kodingan aslimu, 
            // pastikan nama objek pencariannya emang bener richTextBox1 ya.
            LoadDataKaryawan(richTextBox1.Text.Trim());
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            new dashboardPage().Show();
            this.Hide();
        }
    }
}