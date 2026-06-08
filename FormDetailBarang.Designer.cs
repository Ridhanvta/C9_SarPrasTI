namespace ManajemenSarPras
{
    partial class FormDetailBarang
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvDetail = new System.Windows.Forms.DataGridView();
            this.labelNamaBarang = new System.Windows.Forms.Label();
            this.txtSpesifikasi = new System.Windows.Forms.RichTextBox();
            this.labelGedung = new System.Windows.Forms.Label();
            this.txtIdDetail = new System.Windows.Forms.RichTextBox();
            this.labelIdDetailBarang = new System.Windows.Forms.Label();
            this.cmbRuangan = new System.Windows.Forms.ComboBox();
            this.cmbGedung = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnKembali = new System.Windows.Forms.Button();
            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnBatal = new System.Windows.Forms.Button();
            this.btnHapus = new System.Windows.Forms.Button();
            this.txtCari = new System.Windows.Forms.RichTextBox();
            this.labelCariDetailBarang = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbBarang = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDetail
            // 
            this.dgvDetail.AllowUserToAddRows = false;
            this.dgvDetail.AllowUserToDeleteRows = false;
            this.dgvDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetail.Location = new System.Drawing.Point(337, 136);
            this.dgvDetail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dgvDetail.Name = "dgvDetail";
            this.dgvDetail.ReadOnly = true;
            this.dgvDetail.RowHeadersWidth = 62;
            this.dgvDetail.RowTemplate.Height = 28;
            this.dgvDetail.Size = new System.Drawing.Size(487, 297);
            this.dgvDetail.TabIndex = 0;
            // 
            // labelNamaBarang
            // 
            this.labelNamaBarang.AutoSize = true;
            this.labelNamaBarang.Location = new System.Drawing.Point(15, 210);
            this.labelNamaBarang.Name = "labelNamaBarang";
            this.labelNamaBarang.Size = new System.Drawing.Size(86, 16);
            this.labelNamaBarang.TabIndex = 1;
            this.labelNamaBarang.Text = "SPESIFIKASI";
            // 
            // txtSpesifikasi
            // 
            this.txtSpesifikasi.Location = new System.Drawing.Point(117, 201);
            this.txtSpesifikasi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSpesifikasi.Name = "txtSpesifikasi";
            this.txtSpesifikasi.Size = new System.Drawing.Size(195, 35);
            this.txtSpesifikasi.TabIndex = 3;
            this.txtSpesifikasi.Text = "";
            // 
            // labelGedung
            // 
            this.labelGedung.AutoSize = true;
            this.labelGedung.Location = new System.Drawing.Point(15, 258);
            this.labelGedung.Name = "labelGedung";
            this.labelGedung.Size = new System.Drawing.Size(66, 16);
            this.labelGedung.TabIndex = 4;
            this.labelGedung.Text = "GEDUNG";
            // 
            // txtIdDetail
            // 
            this.txtIdDetail.Location = new System.Drawing.Point(117, 150);
            this.txtIdDetail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtIdDetail.Name = "txtIdDetail";
            this.txtIdDetail.Size = new System.Drawing.Size(195, 35);
            this.txtIdDetail.TabIndex = 7;
            this.txtIdDetail.Text = "";
            // 
            // labelIdDetailBarang
            // 
            this.labelIdDetailBarang.AutoSize = true;
            this.labelIdDetailBarang.Location = new System.Drawing.Point(15, 153);
            this.labelIdDetailBarang.Name = "labelIdDetailBarang";
            this.labelIdDetailBarang.Size = new System.Drawing.Size(70, 32);
            this.labelIdDetailBarang.TabIndex = 6;
            this.labelIdDetailBarang.Text = "ID DETAIL\r\nBARANG";
            // 
            // cmbRuangan
            // 
            this.cmbRuangan.FormattingEnabled = true;
            this.cmbRuangan.Location = new System.Drawing.Point(117, 296);
            this.cmbRuangan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbRuangan.Name = "cmbRuangan";
            this.cmbRuangan.Size = new System.Drawing.Size(195, 24);
            this.cmbRuangan.TabIndex = 8;
            // 
            // cmbGedung
            // 
            this.cmbGedung.FormattingEnabled = true;
            this.cmbGedung.Location = new System.Drawing.Point(117, 256);
            this.cmbGedung.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbGedung.Name = "cmbGedung";
            this.cmbGedung.Size = new System.Drawing.Size(195, 24);
            this.cmbGedung.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 298);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = "RUANGAN";
            // 
            // btnKembali
            // 
            this.btnKembali.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKembali.Location = new System.Drawing.Point(20, 10);
            this.btnKembali.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(92, 35);
            this.btnKembali.TabIndex = 11;
            this.btnKembali.Text = "Kembali";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click);
            // 
            // btnSimpan
            // 
            this.btnSimpan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSimpan.Location = new System.Drawing.Point(19, 341);
            this.btnSimpan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(145, 44);
            this.btnSimpan.TabIndex = 12;
            this.btnSimpan.Text = "Tambah Detail \r\nBarang";
            this.btnSimpan.UseVisualStyleBackColor = true;
            // 
            // btnBatal
            // 
            this.btnBatal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBatal.Location = new System.Drawing.Point(169, 341);
            this.btnBatal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new System.Drawing.Size(143, 44);
            this.btnBatal.TabIndex = 13;
            this.btnBatal.Text = "Reset Text";
            this.btnBatal.UseVisualStyleBackColor = true;
            // 
            // btnHapus
            // 
            this.btnHapus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHapus.Location = new System.Drawing.Point(95, 391);
            this.btnHapus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(143, 44);
            this.btnHapus.TabIndex = 14;
            this.btnHapus.Text = "Hapus Detail\r\nBarang";
            this.btnHapus.UseVisualStyleBackColor = true;
            // 
            // txtCari
            // 
            this.txtCari.Location = new System.Drawing.Point(439, 92);
            this.txtCari.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCari.Name = "txtCari";
            this.txtCari.Size = new System.Drawing.Size(385, 35);
            this.txtCari.TabIndex = 15;
            this.txtCari.Text = "";
            // 
            // labelCariDetailBarang
            // 
            this.labelCariDetailBarang.AutoSize = true;
            this.labelCariDetailBarang.Location = new System.Drawing.Point(338, 94);
            this.labelCariDetailBarang.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.labelCariDetailBarang.Name = "labelCariDetailBarang";
            this.labelCariDetailBarang.Size = new System.Drawing.Size(91, 32);
            this.labelCariDetailBarang.TabIndex = 16;
            this.labelCariDetailBarang.Text = "CARI DETAIL \r\nBARANG";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Stencil", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(238, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(453, 32);
            this.label2.TabIndex = 18;
            this.label2.Text = "PENGELOLAHAN DETAIL BARANG";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 32);
            this.label3.TabIndex = 19;
            this.label3.Text = "NAMA\r\nBARANG";
            // 
            // cmbBarang
            // 
            this.cmbBarang.FormattingEnabled = true;
            this.cmbBarang.Location = new System.Drawing.Point(117, 102);
            this.cmbBarang.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbBarang.Name = "cmbBarang";
            this.cmbBarang.Size = new System.Drawing.Size(195, 24);
            this.cmbBarang.TabIndex = 20;
            // 
            // FormDetailBarang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 450);
            this.Controls.Add(this.cmbBarang);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelCariDetailBarang);
            this.Controls.Add(this.txtCari);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.btnBatal);
            this.Controls.Add(this.btnSimpan);
            this.Controls.Add(this.btnKembali);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbGedung);
            this.Controls.Add(this.cmbRuangan);
            this.Controls.Add(this.txtIdDetail);
            this.Controls.Add(this.labelIdDetailBarang);
            this.Controls.Add(this.labelGedung);
            this.Controls.Add(this.txtSpesifikasi);
            this.Controls.Add(this.labelNamaBarang);
            this.Controls.Add(this.dgvDetail);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormDetailBarang";
            this.Text = "FormDetailBarang";
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDetail;
        private System.Windows.Forms.Label labelNamaBarang;
        private System.Windows.Forms.RichTextBox txtSpesifikasi;
        private System.Windows.Forms.Label labelGedung;
        private System.Windows.Forms.RichTextBox txtIdDetail;
        private System.Windows.Forms.Label labelIdDetailBarang;
        private System.Windows.Forms.ComboBox cmbRuangan;
        private System.Windows.Forms.ComboBox cmbGedung;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnKembali;
        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnBatal;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.RichTextBox txtCari;
        private System.Windows.Forms.Label labelCariDetailBarang;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbBarang;
    }
}