namespace ManajemenSarPras
{
    partial class kelolaBarang
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(kelolaBarang));
            this.btnKembali = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.labelNamaBarang = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtNamaBarang = new System.Windows.Forms.TextBox();
            this.txtIdBarang = new System.Windows.Forms.TextBox();
            this.labelIdBarang = new System.Windows.Forms.Label();
            this.btnTambahBarang = new System.Windows.Forms.Button();
            this.btnHapus = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.labelTipeBarang = new System.Windows.Forms.Label();
            this.cmbTipeBarang = new System.Windows.Forms.ComboBox();
            this.labelJumlahBarang = new System.Windows.Forms.Label();
            this.txtJumlahBarang = new System.Windows.Forms.TextBox();
            this.labelCariBarang = new System.Windows.Forms.Label();
            this.txtCari = new System.Windows.Forms.TextBox();
            this.txtReset = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbMerk = new System.Windows.Forms.ComboBox();
            this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnKembali
            // 
            this.btnKembali.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKembali.Location = new System.Drawing.Point(23, 36);
            this.btnKembali.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(107, 31);
            this.btnKembali.TabIndex = 0;
            this.btnKembali.Text = "Kembali";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(419, 153);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(508, 390);
            this.dataGridView1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Stencil", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(256, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(566, 57);
            this.label1.TabIndex = 2;
            this.label1.Text = "PENGELOLAAN BARANG";
            // 
            // labelNamaBarang
            // 
            this.labelNamaBarang.AutoSize = true;
            this.labelNamaBarang.Location = new System.Drawing.Point(21, 185);
            this.labelNamaBarang.Name = "labelNamaBarang";
            this.labelNamaBarang.Size = new System.Drawing.Size(128, 20);
            this.labelNamaBarang.TabIndex = 3;
            this.labelNamaBarang.Text = "NAMA BARANG";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 203);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 20);
            this.label3.TabIndex = 4;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // txtNamaBarang
            // 
            this.txtNamaBarang.Location = new System.Drawing.Point(185, 182);
            this.txtNamaBarang.Name = "txtNamaBarang";
            this.txtNamaBarang.Size = new System.Drawing.Size(215, 26);
            this.txtNamaBarang.TabIndex = 6;
            // 
            // txtIdBarang
            // 
            this.txtIdBarang.Location = new System.Drawing.Point(185, 135);
            this.txtIdBarang.Name = "txtIdBarang";
            this.txtIdBarang.Size = new System.Drawing.Size(215, 26);
            this.txtIdBarang.TabIndex = 8;
            // 
            // labelIdBarang
            // 
            this.labelIdBarang.AutoSize = true;
            this.labelIdBarang.Location = new System.Drawing.Point(21, 138);
            this.labelIdBarang.Name = "labelIdBarang";
            this.labelIdBarang.Size = new System.Drawing.Size(99, 20);
            this.labelIdBarang.TabIndex = 7;
            this.labelIdBarang.Text = "ID BARANG";
            // 
            // btnTambahBarang
            // 
            this.btnTambahBarang.BackColor = System.Drawing.Color.Lime;
            this.btnTambahBarang.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTambahBarang.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnTambahBarang.Location = new System.Drawing.Point(25, 410);
            this.btnTambahBarang.Name = "btnTambahBarang";
            this.btnTambahBarang.Size = new System.Drawing.Size(124, 34);
            this.btnTambahBarang.TabIndex = 9;
            this.btnTambahBarang.Text = "Tambah Barang";
            this.btnTambahBarang.UseVisualStyleBackColor = false;
            this.btnTambahBarang.Click += new System.EventHandler(this.btnSimpan_Click);
            // 
            // btnHapus
            // 
            this.btnHapus.BackColor = System.Drawing.Color.Red;
            this.btnHapus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHapus.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btnHapus.Location = new System.Drawing.Point(162, 410);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(110, 34);
            this.btnHapus.TabIndex = 11;
            this.btnHapus.Text = "Hapus Barang";
            this.btnHapus.UseVisualStyleBackColor = false;
            this.btnHapus.Click += new System.EventHandler(this.btnHapus_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(25, 450);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(375, 34);
            this.button3.TabIndex = 12;
            this.button3.Text = "Detail Barang";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // labelTipeBarang
            // 
            this.labelTipeBarang.AutoSize = true;
            this.labelTipeBarang.Location = new System.Drawing.Point(21, 290);
            this.labelTipeBarang.Name = "labelTipeBarang";
            this.labelTipeBarang.Size = new System.Drawing.Size(117, 20);
            this.labelTipeBarang.TabIndex = 13;
            this.labelTipeBarang.Text = "TIPE BARANG";
            // 
            // cmbTipeBarang
            // 
            this.cmbTipeBarang.FormattingEnabled = true;
            this.cmbTipeBarang.Items.AddRange(new object[] {
            "Perlu Pengecekkan Rutin",
            "Tidak Perlu Pengecekkan Rutin"});
            this.cmbTipeBarang.Location = new System.Drawing.Point(185, 290);
            this.cmbTipeBarang.Name = "cmbTipeBarang";
            this.cmbTipeBarang.Size = new System.Drawing.Size(215, 28);
            this.cmbTipeBarang.TabIndex = 14;
            // 
            // labelJumlahBarang
            // 
            this.labelJumlahBarang.AutoSize = true;
            this.labelJumlahBarang.Location = new System.Drawing.Point(21, 343);
            this.labelJumlahBarang.Name = "labelJumlahBarang";
            this.labelJumlahBarang.Size = new System.Drawing.Size(151, 40);
            this.labelJumlahBarang.TabIndex = 15;
            this.labelJumlahBarang.Text = "JUMLAH BARANG \r\nNON-CHECK";
            // 
            // txtJumlahBarang
            // 
            this.txtJumlahBarang.Location = new System.Drawing.Point(185, 352);
            this.txtJumlahBarang.Name = "txtJumlahBarang";
            this.txtJumlahBarang.Size = new System.Drawing.Size(215, 26);
            this.txtJumlahBarang.TabIndex = 16;
            // 
            // labelCariBarang
            // 
            this.labelCariBarang.AutoSize = true;
            this.labelCariBarang.Location = new System.Drawing.Point(415, 103);
            this.labelCariBarang.Name = "labelCariBarang";
            this.labelCariBarang.Size = new System.Drawing.Size(121, 20);
            this.labelCariBarang.TabIndex = 18;
            this.labelCariBarang.Text = "CARI BARANG";
            // 
            // txtCari
            // 
            this.txtCari.Location = new System.Drawing.Point(542, 100);
            this.txtCari.Name = "txtCari";
            this.txtCari.Size = new System.Drawing.Size(385, 26);
            this.txtCari.TabIndex = 19;
            // 
            // txtReset
            // 
            this.txtReset.BackColor = System.Drawing.Color.Yellow;
            this.txtReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReset.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.txtReset.Location = new System.Drawing.Point(290, 410);
            this.txtReset.Name = "txtReset";
            this.txtReset.Size = new System.Drawing.Size(110, 34);
            this.txtReset.TabIndex = 20;
            this.txtReset.Text = "Reset Text";
            this.txtReset.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 237);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 20);
            this.label2.TabIndex = 21;
            this.label2.Text = "MERK";
            // 
            // cmbMerk
            // 
            this.cmbMerk.FormattingEnabled = true;
            this.cmbMerk.Location = new System.Drawing.Point(185, 228);
            this.cmbMerk.Name = "cmbMerk";
            this.cmbMerk.Size = new System.Drawing.Size(215, 28);
            this.cmbMerk.TabIndex = 22;
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = null;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = null;
            this.bindingNavigator1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2});
            this.bindingNavigator1.Location = new System.Drawing.Point(0, 0);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(940, 38);
            this.bindingNavigator1.TabIndex = 23;
            this.bindingNavigator1.Text = "bindingNavigator1";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(34, 33);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(34, 28);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 33);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 31);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(54, 28);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 33);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(34, 28);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(34, 28);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 33);
            // 
            // kelolaBarang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(940, 563);
            this.Controls.Add(this.bindingNavigator1);
            this.Controls.Add(this.cmbMerk);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtReset);
            this.Controls.Add(this.txtCari);
            this.Controls.Add(this.labelCariBarang);
            this.Controls.Add(this.txtJumlahBarang);
            this.Controls.Add(this.labelJumlahBarang);
            this.Controls.Add(this.cmbTipeBarang);
            this.Controls.Add(this.labelTipeBarang);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.btnTambahBarang);
            this.Controls.Add(this.txtIdBarang);
            this.Controls.Add(this.labelIdBarang);
            this.Controls.Add(this.txtNamaBarang);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.labelNamaBarang);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnKembali);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "kelolaBarang";
            this.Text = "kelolaBarang";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnKembali;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelNamaBarang;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.TextBox txtNamaBarang;
        private System.Windows.Forms.TextBox txtIdBarang;
        private System.Windows.Forms.Label labelIdBarang;
        private System.Windows.Forms.Button btnTambahBarang;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label labelTipeBarang;
        private System.Windows.Forms.ComboBox cmbTipeBarang;
        private System.Windows.Forms.Label labelJumlahBarang;
        private System.Windows.Forms.TextBox txtJumlahBarang;
        private System.Windows.Forms.Label labelCariBarang;
        private System.Windows.Forms.TextBox txtCari;
        private System.Windows.Forms.Button txtReset;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbMerk;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
    }
}