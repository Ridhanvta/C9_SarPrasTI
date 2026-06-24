using SatprasDesktopApp.Config;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class maintenancePage : Form
    {
        private string selectedIdDetailBarang = "";
        private string selectedIdBarang = "";

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
            this.btnReset.Click += new EventHandler(btnReset_Click);

            this.rbBaik.CheckedChanged += new EventHandler(rbKondisi_CheckedChanged);
            this.rbRusak.CheckedChanged += new EventHandler(rbKondisi_CheckedChanged);

            // Handler btnHapus di-bypass langsung ke pesan peringatan jika di-klik
            this.btnHapus.Click += (s, e) => { MessageBox.Show("Kebijakan Sistem: Log Riwayat Maintenance permanen dan dilarang keras untuk dihapus!", "Akses Ditolak", MessageBoxButtons.OK, MessageBoxIcon.Stop); };
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

            cmbTindakLanjut.Items.Clear();
            cmbTindakLanjut.Items.Add("Diganti");
            cmbTindakLanjut.Items.Add("Diperbaiki");
            cmbTindakLanjut.DropDownStyle = ComboBoxStyle.DropDownList;

            LoadComboSemester();
            LoadComboKaryawan();
            RefreshSemuaTabel();
            ResetForm();
        }

        private string GetActiveSemesterFromSystem()
        {
            int senderTahun = DateTime.Now.Year;
            int senderBulan = DateTime.Now.Month;

            string tipeSemester;
            string tahunAjaran;

            if (senderBulan >= 7)
            {
                tipeSemester = "Ganjil";
                tahunAjaran = $"{senderTahun}/{senderTahun + 1}";
            }
            else
            {
                tipeSemester = "Genap";
                tahunAjaran = $"{senderTahun - 1}/{senderTahun}";
            }

            return $"{tahunAjaran} ({tipeSemester})";
        }

        private void AutoSelectActiveSemester()
        {
            if (cmbSemester.Items.Count > 0)
            {
                cmbSemester.SelectedIndex = 0;
            }
            else
            {
                cmbSemester.SelectedIndex = -1;
            }
        }

        private void rbKondisi_CheckedChanged(object sender, EventArgs e)
        {
            if (rbBaik.Checked)
            {
                txtKerusakan.Text = "-";
                txtKerusakan.Enabled = false;

                cmbTindakLanjut.SelectedIndex = -1;
                cmbTindakLanjut.Enabled = false;
            }
            else if (rbRusak.Checked)
            {
                if (txtKerusakan.Text == "-") txtKerusakan.Clear();
                txtKerusakan.Enabled = true;
                cmbTindakLanjut.Enabled = true;
            }
        }

        private void LoadComboKaryawan()
        {
            try
            {
                DataTable dt = DAL.GetKaryawanData();
                cmbKaryawan.DataSource = dt;
                cmbKaryawan.DisplayMember = "namaKaryawan";
                cmbKaryawan.ValueMember = "idKaryawan";
                cmbKaryawan.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbKaryawan.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Karyawan", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void LoadComboSemester()
        {
            try
            {
                DataTable dt = DAL.GetSemesterAktif(GetActiveSemesterFromSystem());
                cmbSemester.DataSource = dt;
                cmbSemester.DisplayMember = "tahunAjaran";
                cmbSemester.ValueMember = "idSemester";
                cmbSemester.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbSemester.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Semester", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private BindingSource bsBarang = new BindingSource();
        private DataTable dtBarang = new DataTable();
        private void LoadDataBarang(string keyword = "")
        {
            try
            {
                dtBarang = DAL.GetDetailBarangAsset(keyword);
                bsBarang.DataSource = dtBarang;
                dataGridView2.DataSource = bsBarang;
                bindingNavigator1.BindingSource = bsBarang;

                if (dataGridView2.Columns["idBarang"] != null)
                    dataGridView2.Columns["idBarang"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error Load Data Aset", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private BindingSource bsMaintenance = new BindingSource();
        private DataTable dtMaint = new DataTable();
        private void LoadDataRiwayat(string keyword = "")
        {
            try
            {
                dtMaint = DAL.GetMaintenanceHistory(keyword);
                bsMaintenance.DataSource = dtMaint;
                dataGridView1.DataSource = bsMaintenance;

                if (dataGridView1.Columns["idKaryawan"] != null) dataGridView1.Columns["idKaryawan"].Visible = false;
                if (dataGridView1.Columns["idSemester"] != null) dataGridView1.Columns["idSemester"].Visible = false;
                if (dataGridView1.Columns["idDetailBarang"] != null) dataGridView1.Columns["idDetailBarang"].Visible = false;
                if (dataGridView1.Columns["idBarang"] != null) dataGridView1.Columns["idBarang"].Visible = false;
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
            selectedIdDetailBarang = "";
            selectedIdBarang = "";

            cmbRuangan.Text = "";
            cmbDetailBarang.Text = "";

            cmbKaryawan.SelectedIndex = -1;
            cmbTindakLanjut.SelectedIndex = -1;

            txtKerusakan.Clear();
            rbBaik.Checked = true;

            btnSimpan.Text = "Simpan";
            btnSimpan.Enabled = true; // Nyalakan kembali tombol simpan untuk entri baru
            btnHapus.Enabled = false; // Matikan total tombol hapus

            AutoSelectActiveSemester();
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
                bsBarang.Position = e.RowIndex;
                DataRowView row = (DataRowView)bsBarang.Current;

                selectedIdDetailBarang = row["ID Detail"].ToString();
                selectedIdBarang = row["idBarang"].ToString();

                cmbDetailBarang.Text = row["Aset"].ToString();
                cmbRuangan.Text = row["Lokasi"].ToString();

                btnSimpan.Text = "Simpan";
                btnSimpan.Enabled = true; // Siap melayani input data baru
                AutoSelectActiveSemester();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // MODE NYIMAK (READ-ONLY VIEW)
                bsMaintenance.Position = e.RowIndex;
                DataRowView row = (DataRowView)bsMaintenance.Current;

                // Tampilkan data ke control UI biar user bisa baca detail riwayatnya
                txtKerusakan.Text = row["Kerusakan"].ToString();
                string valTindakLanjut = row["Tindak Lanjut"].ToString();
                cmbTindakLanjut.SelectedIndex = cmbTindakLanjut.Items.Contains(valTindakLanjut) ? cmbTindakLanjut.Items.IndexOf(valTindakLanjut) : -1;

                if (row["idKaryawan"] != DBNull.Value) cmbKaryawan.SelectedValue = row["idKaryawan"].ToString();

                string statusKondisi = row["Status"].ToString();
                if (statusKondisi == "Baik") rbBaik.Checked = true;
                else rbRusak.Checked = true;

                cmbDetailBarang.Text = row["Aset"].ToString();
                cmbRuangan.Text = row["Lokasi"].ToString();

                // KRUSIAL PROTEKSI: Kunci tombol simpan agar tidak bisa melakukan kecurangan "UPDATE"
                btnSimpan.Enabled = false;
                btnSimpan.Text = "Riwayat Terkunci (No Update)";
                btnHapus.Enabled = false;
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedIdDetailBarang) || cmbKaryawan.SelectedValue == null || cmbSemester.SelectedValue == null)
            {
                MessageBox.Show("Silakan pilih Data Aset dari tabel di samping kanan, lalu lengkapi Petugas dan Semester.", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rbRusak.Checked && cmbTindakLanjut.SelectedIndex == -1)
            {
                MessageBox.Show("Untuk aset rusak, silakan pilih Tindak Lanjut ('Diganti' / 'Diperbaiki') terlebih dahulu!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int kondisiBaru = rbBaik.Checked ? 1 : 0;
            string tindakLanjutValue = rbBaik.Checked ? "-" : cmbTindakLanjut.SelectedItem.ToString();

            try
            {
                DAL.SimpanMaintenance(
                    cmbKaryawan.SelectedValue.ToString(),
                    selectedIdDetailBarang,
                    selectedIdBarang,
                    dtpTglCek.Value.Date,
                    kondisiBaru,
                    rbBaik.Checked ? "-" : txtKerusakan.Text.Trim(),
                    tindakLanjutValue,
                    Convert.ToInt32(cmbSemester.SelectedValue)
                );

                MessageBox.Show("Log Riwayat Cek Maintenance Berhasil Disimpan Permanen!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshSemuaTabel();
                ResetForm();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ditolak Sistem: " + ex.Message, "Gagal Proteksi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}