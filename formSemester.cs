using SatprasDesktopApp.Config;
using System;
using System.Data;

using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class formSemester : Form
    {
        public formSemester()
        {
            InitializeComponent();

            // Mempertahankan struktur Hardcode Event Binding

        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void formSemester_Load(object sender, EventArgs e)
        {
            try
            {
                txtTahunAjaran.ReadOnly = true; // Kunci textbox dari ketikan manual
                LoadDataSemester();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saat load form: " + ex.Message, "gabisa load form semester", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetActiveSemesterFromSystem()
        {
            int tahunSekarang = DateTime.Now.Year;
            int bulanSekarang = DateTime.Now.Month;

            string tipeSemester;
            string tahunAjaran;

            if (bulanSekarang >= 7)
            {
                tipeSemester = "Ganjil";
                tahunAjaran = $"{tahunSekarang}/{tahunSekarang + 1}";
            }
            else
            {
                tipeSemester = "Genap";
                tahunAjaran = $"{tahunSekarang - 1}/{tahunSekarang}";
            }

            return $"{tahunAjaran} ({tipeSemester})";
        }

        private void LoadDataSemester(string keyword = "")
        {
            try
            {
                DataTable dt = DAL.GetSemesterData(keyword);

                txtTahunAjaran.DataBindings.Clear();
                bindingSource1.DataSource = dt;

                bindingNavigator1.BindingSource = bindingSource1;
                dataGridView1.DataSource = bindingSource1;

                txtTahunAjaran.DataBindings.Add("Text", bindingSource1, "Tahun Ajaran", true, DataSourceUpdateMode.OnPropertyChanged);

                if (dt.Rows.Count > 0)
                {
                    bindingSource1.MoveFirst();
                }
                else
                {
                    txtTahunAjaran.Text = GetActiveSemesterFromSystem();
                }

                if (dataGridView1.Columns["ID"] != null) dataGridView1.Columns["ID"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show("Error Load Data: " + ex.Message); }
        }

        private void ResetForm()
        {
            btnTambah.Text = "Tambah Semester Aktif";
            btnTambah.Enabled = true; // Selalu nyala untuk mode insert data baru
            txtTahunAjaran.Text = GetActiveSemesterFromSystem();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Sekarang tombol ini murni berfungsi sebagai "Reset/Clear" UI saja
            ResetForm();
            LoadDataSemester();
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataSemester(txtCari.Text.Trim());
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bindingSource1.Position = e.RowIndex;

                // Kunci tombol tambah jika user nge-klik data lama di grid (karena tidak boleh di-update)
                btnTambah.Enabled = false;
                btnTambah.Text = "Data Terkunci (No Update)";
            }
        }

        private void btnTambah_Click(object sender, EventArgs e)
        {
            bindingSource1.EndEdit();
            string inputTahun = GetActiveSemesterFromSystem();

            try
            {
                DAL.TambahSemester(inputTahun);
                MessageBox.Show("Semester Aktif Berhasil Ditambahkan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDataSemester();
                ResetForm();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("sudah terdaftar"))
                {
                    MessageBox.Show(ex.Message, "Validasi Tolak", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        // Fungsi Hapus dikosongkan total demi keamanan data integritas kampus
        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kebijakan Sistem: Data Semester yang sudah masuk tidak dapat dihapus!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
}