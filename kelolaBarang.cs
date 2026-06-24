using ExcelDataReader;
using SatprasDesktopApp.Config;
using System;
using System.Collections.Generic;
using System.Data;

using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ManajemenSarPras
{
    public partial class kelolaBarang : Form
    {
        private bool isEditMode = false;
        private string originalIdBarang = "";

        // Flag pengaman biar form input gak bentrok pas reload data master
        private bool isResetting = false;
        private BindingSource bsBarang = new BindingSource();

        private DataTable dtPreviewExcel;
        private bool isPreviewMode = false;

        public kelolaBarang()
        {
            InitializeComponent();

            this.Load += new EventHandler(kelolaBarang_Load);
            this.dataGridView1.CellClick += new DataGridViewCellEventHandler(dgvBarang_CellClick);
            this.txtCari.TextChanged += new EventHandler(txtCari_TextChanged);
            this.txtJumlahBarang.KeyPress += new KeyPressEventHandler(txtStok_KeyPress);
            this.cmbTipeBarang.SelectedIndexChanged += new EventHandler(cmbTipeBarang_SelectedIndexChanged);


            this.txtReset.Click += new EventHandler(btnBatal_Click);
            this.button3.Click += new EventHandler(button3_Click);

            // Mengikat pergerakan baris remot Navigator / Klik Grid ke isian form
            this.bsBarang.PositionChanged += new EventHandler(bsBarang_PositionChanged);
        }

        private void btnKembali_Click(object sender, EventArgs e)
        {
            dashboardPage dashboard = new dashboardPage();
            dashboard.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormDetailBarang form = new FormDetailBarang();
            this.Hide();
            form.Show();
        }

        private void kelolaBarang_Load(object sender, EventArgs e)
        {
            DataTable dtTipe = new DataTable();
            dtTipe.Columns.Add("Value", typeof(int));
            dtTipe.Columns.Add("Display", typeof(string));
            dtTipe.Rows.Add(0, "Barang Habis Pakai (Non-Rutin)");
            dtTipe.Rows.Add(1, "Aset Tetap (Pengecekan Rutin)");

            cmbTipeBarang.DataSource = dtTipe;
            cmbTipeBarang.DisplayMember = "Display";
            cmbTipeBarang.ValueMember = "Value";
            cmbTipeBarang.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTipeBarang.SelectedIndex = -1;

            LoadComboBoxKuantitas();
            LoadComboMerk();
            LoadAutoCompleteNamaBarang();
            LoadDataBarang();
            ResetForm();
        }

        // Mengatur form isi otomatis mengikuti remot navigasi
        private void bsBarang_PositionChanged(object sender, EventArgs e)
        {
            if (isResetting || bsBarang.Current == null) return;

            DataRowView row = (DataRowView)bsBarang.Current;

            originalIdBarang = row["ID/Kode Barang"].ToString();
            txtNamaBarang.Text = row["Nama Barang"].ToString();
            cmbMerk.Text = row["Merk"].ToString();
            txtJumlahBarang.Text = row["Sisa Stok"].ToString();

            cmbTipeBarang.SelectedValue = Convert.ToInt32(row["tipeBarang"]);
            cmbKuantitas.SelectedValue = row["Satuan"].ToString();

            cmbTipeBarang.Enabled = false;
            isEditMode = true;

            btnTambahBarang.Text = "Update";
        }

        private void LoadComboBoxKuantitas()
        {
            DataTable dtKuantitas = new DataTable();
            dtKuantitas.Columns.Add("Display", typeof(string));
            dtKuantitas.Columns.Add("NamaSatuan", typeof(string));
            dtKuantitas.Columns.Add("MaxLimit", typeof(int));

            dtKuantitas.Rows.Add("Pcs (Pieces)", "Pcs", 100);
            dtKuantitas.Rows.Add("Rem (Ream)", "Rem", 50);
            dtKuantitas.Rows.Add("Box", "Box", 30);
            dtKuantitas.Rows.Add("Pack", "Pack", 40);
            dtKuantitas.Rows.Add("Roll", "Roll", 20);

            cmbKuantitas.DataSource = dtKuantitas;
            cmbKuantitas.DisplayMember = "Display";
            cmbKuantitas.ValueMember = "NamaSatuan";
            cmbKuantitas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKuantitas.SelectedIndex = -1;
        }

        private void LoadComboMerk()
        {
            try
            {
                DataTable dt = DAL.GetMerkData();
                cmbMerk.DataSource = dt;
                cmbMerk.DisplayMember = "namaMerk";
                cmbMerk.ValueMember = "idMerk";
                cmbMerk.DropDownStyle = ComboBoxStyle.DropDown;
                cmbMerk.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbMerk.AutoCompleteSource = AutoCompleteSource.ListItems;
                cmbMerk.SelectedIndex = -1;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadAutoCompleteNamaBarang()
        {
            try
            {
                DataTable dt = DAL.ExecuteQuery("SELECT DISTINCT namaBarang FROM master.barang");
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                foreach (DataRow row in dt.Rows)
                {
                    collection.Add(row["namaBarang"].ToString());
                }
                txtNamaBarang.AutoCompleteCustomSource = collection;
                txtNamaBarang.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtNamaBarang.AutoCompleteSource = AutoCompleteSource.CustomSource;
            }
            catch (Exception) { }
        }

        private void LoadDataBarang(string keyword = "")
        {
            try
            {
                DataTable dt = DAL.GetBarangData(keyword);

                isResetting = true;
                bsBarang.DataSource = dt;
                bindingNavigator1.BindingSource = bsBarang;
                dataGridView1.DataSource = bsBarang;
                isResetting = false;

                if (dataGridView1.Columns["tipeBarang"] != null)
                    dataGridView1.Columns["tipeBarang"].Visible = false;
                if (dataGridView1.Columns["idMerk"] != null)
                    dataGridView1.Columns["idMerk"].Visible = false;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ResetForm()
        {
            isResetting = true;

            txtNamaBarang.Clear();
            cmbMerk.SelectedIndex = -1;
            cmbMerk.Text = "";
            txtJumlahBarang.Clear();
            cmbTipeBarang.SelectedIndex = -1;
            cmbKuantitas.SelectedIndex = -1;

            txtJumlahBarang.ReadOnly = false;
            txtJumlahBarang.BackColor = SystemColors.Window;

            cmbTipeBarang.Enabled = true;
            cmbKuantitas.Enabled = true;

            isEditMode = false;
            originalIdBarang = "";

            btnTambahBarang.Text = "Simpan";
            txtNamaBarang.Focus();

            isResetting = false;
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void cmbTipeBarang_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTipeBarang.SelectedIndex == -1 || cmbTipeBarang.SelectedValue == null) return;

            int tipe;
            if (!int.TryParse(cmbTipeBarang.SelectedValue.ToString(), out tipe)) return;

            if (tipe == 1)
            {
                txtJumlahBarang.Text = "0";
                txtJumlahBarang.ReadOnly = true;
                txtJumlahBarang.BackColor = SystemColors.Control;

                cmbKuantitas.SelectedValue = "Pcs";
                cmbKuantitas.Enabled = false;
            }
            else
            {
                txtJumlahBarang.ReadOnly = false;
                txtJumlahBarang.BackColor = SystemColors.Window;
                cmbKuantitas.Enabled = true;

                if (!isEditMode)
                {
                    if (txtJumlahBarang.Text == "0") txtJumlahBarang.Clear();
                    cmbKuantitas.SelectedIndex = -1;
                }
            }
        }

        private void txtStok_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCari_TextChanged(object sender, EventArgs e)
        {
            LoadDataBarang(txtCari.Text.Trim());
        }

        private void dgvBarang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                bsBarang.Position = e.RowIndex;
            }
        }

        // ==========================================
        // FUNGSI 2: GENERATOR KODE BARANG OTOMATIS (TERSEDIA KEMBALI)
        // ==========================================
        private void btnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNamaBarang.Text) ||
                string.IsNullOrWhiteSpace(cmbMerk.Text) ||
                string.IsNullOrWhiteSpace(txtJumlahBarang.Text) ||
                cmbTipeBarang.SelectedValue == null ||
                cmbKuantitas.SelectedIndex == -1)
            {
                MessageBox.Show("Seluruh form wajib diisi termasuk Merk dan Tipe Kuantitas Satuan.", "Validasi Ketat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int tipeInput = Convert.ToInt32(cmbTipeBarang.SelectedValue);
            int stokInput = Convert.ToInt32(txtJumlahBarang.Text.Trim());
            string satuanInput = cmbKuantitas.SelectedValue.ToString();

            if (tipeInput == 0 && !isEditMode)
            {
                if (stokInput <= 0)
                {
                    MessageBox.Show("Untuk Barang Habis Pakai, stok awal harus lebih dari 0.", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtJumlahBarang.Focus();
                    return;
                }

                DataRowView selectedKuantitas = (DataRowView)cmbKuantitas.SelectedItem;
                int maxWajar = Convert.ToInt32(selectedKuantitas["MaxLimit"]);

                if (stokInput > maxWajar)
                {
                    MessageBox.Show($"Pembelian berlebihan! Batas kuantitas wajar untuk satuan '{cmbKuantitas.Text}' di kampus adalah maksimal {maxWajar}!", "Batas Tidak Wajar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtJumlahBarang.Focus();
                    return;
                }
            }

            try
            {
                string finalIdBarang = DAL.SimpanBarangMaster(isEditMode, originalIdBarang, txtNamaBarang.Text.Trim(), cmbMerk.Text, stokInput, tipeInput, satuanInput);
                MessageBox.Show($"Data inventaris Master dengan Kode '{finalIdBarang}' [{satuanInput}] berhasil {(isEditMode ? "diperbarui" : "disimpan")}!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadComboMerk();
                LoadAutoCompleteNamaBarang();
                LoadDataBarang();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SwitchToPreviewMode(bool previewOn)
        {
            isPreviewMode = previewOn;

            if (previewOn)
            {
                bsBarang.DataSource = null;
                dataGridView1.DataSource = dtPreviewExcel;

                btnTambahBarang.Enabled = false;
                txtCari.Enabled = false;
                cmbMerk.Enabled = false;
                cmbTipeBarang.Enabled = false;
                cmbKuantitas.Enabled = false;
                txtNamaBarang.Enabled = false;
                txtJumlahBarang.Enabled = false;

                btnDB.Enabled = true;
                btnDB.Text = "Eksekusi DB";
                btnDB.BackColor = Color.OrangeRed;
                btnDB.ForeColor = Color.White;

                txtReset.Text = "Batal Import";
            }
            else
            {
                dtPreviewExcel.Clear();
                btnTambahBarang.Enabled = true;
                txtCari.Enabled = true;
                cmbMerk.Enabled = true;
                cmbTipeBarang.Enabled = true;
                cmbKuantitas.Enabled = true;
                txtNamaBarang.Enabled = true;
                txtJumlahBarang.Enabled = true;

                btnDB.Enabled = false;
                btnDB.Text = "Import Excel";
                btnDB.BackColor = SystemColors.Control;
                btnDB.ForeColor = Color.Black;

                txtReset.Text = "Reset Form";

                LoadDataBarang();
                ResetForm();
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

        private void btnExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var stream = File.Open(ofd.FileName, FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = ExcelReaderFactory.CreateReader(stream))
                            {
                                var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                                {
                                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                                });

                                dtPreviewExcel = result.Tables[0];

                                if (!dtPreviewExcel.Columns.Contains("NamaBarang") || !dtPreviewExcel.Columns.Contains("IDDetailBarang"))
                                {
                                    MessageBox.Show("Format Excel salah. Pastikan Header Excel Anda sesuai dengan standar sistem:\n[NamaBarang, Merk, TipeBarang, Satuan, IsiKonversi, StokHabisPakai, IDDetailBarang, Spesifikasi, NamaGedung, NamaRuangan]", "Penolakan Sistem", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                                    return;
                                }

                                SwitchToPreviewMode(true);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"File terkunci atau rusak. Tutup Excel jika sedang dibuka. Detail: {ex.Message}", "I/O Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnDB_Click(object sender, EventArgs e)
        {
            if (!isPreviewMode || dtPreviewExcel == null || dtPreviewExcel.Rows.Count == 0) return;

            DialogResult dialog = MessageBox.Show($"Sistem akan mengeksekusi {dtPreviewExcel.Rows.Count} baris instruksi ke Database. Tindakan ini bersifat mutlak dan tidak bisa dibatalkan. Eksekusi?", "Konfirmasi Otorisasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dialog != DialogResult.Yes) return;

            try
            {
                DAL.ImportDataBarang(dtPreviewExcel);
                MessageBox.Show("Boom! Semua data sukses masuk ke Master dan Detail sekaligus!", "Import Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                SwitchToPreviewMode(false);
                LoadDataBarang();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Import dibatalkan karena error: " + ex.Message, "Error Database", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
}
}