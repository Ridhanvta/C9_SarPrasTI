using SatprasDesktopApp.Config;
using System;
using System.Data;

using System.Drawing;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class FormDetailBarang : Form
    {
        private bool isEditMode = false;
        private string originalIdDetail = "";
        private string originalIdBarang = "";

        // Flag pengaman agar tidak terjadi bentrokan data pas form di-reset
        private bool isResetting = false;
        private BindingSource bsDetail = new BindingSource();

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

            // Kunci input bulk hanya angka
            this.txtJumlahInput.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            };

            // SENSOR UTAMA: Mengikat perubahan navigasi tombol maupun grid ke form inputan
            this.bsDetail.PositionChanged += new EventHandler(bsDetail_PositionChanged);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            kelolaBarang kelola = new kelolaBarang();
            kelola.Show();
            this.Hide();
        }

        private void FormDetailBarang_Load(object sender, EventArgs e)
        {
            LoadComboBoxBarang();
            LoadComboBoxGedung();
            LoadDataDetail();
            ResetForm();
        }

        // FUNGSI UTAMA: Otomatis mengisi form setiap kali posisi data bergeser (klik Grid / klik Navigator)
        private void bsDetail_PositionChanged(object sender, EventArgs e)
        {
            if (isResetting || bsDetail.Current == null) return;

            DataRowView row = (DataRowView)bsDetail.Current;

            originalIdDetail = row["ID Detail"].ToString();
            txtSpesifikasi.Text = row["Spesifikasi"].ToString();
            originalIdBarang = row["idBarang"].ToString();

            cmbBarang.SelectedValue = originalIdBarang;
            cmbGedung.SelectedValue = Convert.ToInt32(row["idGedung"]);
            cmbRuangan.SelectedValue = Convert.ToInt32(row["idRuangan"]);

            cmbBarang.Enabled = false;
            txtJumlahInput.Text = "1";
            txtJumlahInput.Enabled = false;

            isEditMode = true;
            btnSimpan.Text = "Update Data";
        }

        private void LoadComboBoxBarang()
        {
            try
            {
                DataTable dt = DAL.GetComboBoxBarangAset();

                DataRow dr = dt.NewRow();
                dr["idBarang"] = "-- Pilih Aset Barang --";
                dr["displayBarang"] = "-- Pilih Aset Barang --";
                dt.Rows.InsertAt(dr, 0);

                cmbBarang.DataSource = dt;
                cmbBarang.DisplayMember = "displayBarang";
                cmbBarang.ValueMember = "idBarang";
                cmbBarang.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadComboBoxGedung()
        {
            try
            {
                DataTable dt = DAL.GetComboBoxGedung();

                DataRow dr = dt.NewRow();
                dr["idGedung"] = 0;
                dr["namaGedung"] = "-- Pilih Gedung --";
                dt.Rows.InsertAt(dr, 0);

                cmbGedung.DataSource = dt;
                cmbGedung.DisplayMember = "namaGedung";
                cmbGedung.ValueMember = "idGedung";
                cmbGedung.DropDownStyle = ComboBoxStyle.DropDownList;
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
                DataTable dt = DAL.GetComboBoxRuangan(idGedung);

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
                DataTable dt = DAL.GetDetailBarangData(keyword);

                isResetting = true; // Nyalakan flag proteksi sementara pas data reload
                bsDetail.DataSource = dt;
                bindingNavigator1.BindingSource = bsDetail;
                dgvDetail.DataSource = bsDetail;
                
                if (dgvDetail.Columns["tipeBarang"] != null) dgvDetail.Columns["tipeBarang"].Visible = false;
                if (dgvDetail.Columns["idBarang"] != null) dgvDetail.Columns["idBarang"].Visible = false;
                if (dgvDetail.Columns["idGedung"] != null) dgvDetail.Columns["idGedung"].Visible = false;
                if (dgvDetail.Columns["idRuangan"] != null) dgvDetail.Columns["idRuangan"].Visible = false;

                isResetting = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ResetForm()
        {
            isResetting = true; // Kunci sensor perpindahan data pas form dikosongkan manual

            txtSpesifikasi.Clear();
            txtJumlahInput.Text = "1";
            txtJumlahInput.Enabled = true;
            cmbBarang.Enabled = true;

            if (cmbBarang.Items.Count > 0) cmbBarang.SelectedIndex = 0;
            if (cmbGedung.Items.Count > 0) cmbGedung.SelectedIndex = 0;
            cmbRuangan.Enabled = false;

            isEditMode = false;
            originalIdDetail = "";
            originalIdBarang = "";

            btnSimpan.Text = "Tambah Detail Barang";
            btnSimpan.Enabled = true;
            cmbBarang.Focus();

            isResetting = false; // Buka kembali gembok sensor
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

        private void btnBatal_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataDetail(txtCari.Text.Trim());
        }

        private void dgvDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Cukup pindahkan posisi di BindingSource, mapping kolom otomatis ditangani bsDetail_PositionChanged
                bsDetail.Position = e.RowIndex;
            }
        }

        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSpesifikasi.Text) || cmbBarang.SelectedIndex <= 0 ||
                string.IsNullOrWhiteSpace(txtJumlahInput.Text) || cmbGedung.SelectedIndex <= 0 || cmbRuangan.SelectedIndex <= 0)
            {
                MessageBox.Show("Lengkapi semua isian form!", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int jumlahLoop = Convert.ToInt32(txtJumlahInput.Text.Trim());
            if (jumlahLoop < 1)
            {
                MessageBox.Show("Jumlah barang minimal pembelian adalah 1!", "Validasi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedIdBarang = cmbBarang.SelectedValue.ToString();
            string satuanFinal = "Pcs";

            if (!isEditMode && jumlahLoop > 25)
            {
                MessageBox.Show("Batas maksimal input sekaligus untuk jenis Aset Tetap/Maintenance adalah 25 unit!", "Batas Terlampaui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                DAL.SimpanDetailBarang(isEditMode, originalIdDetail, selectedIdBarang, jumlahLoop, Convert.ToInt32(cmbRuangan.SelectedValue), txtSpesifikasi.Text.Trim(), satuanFinal);
                MessageBox.Show("Proses Sinkronisasi Sukses Berjalan!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadDataDetail();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}