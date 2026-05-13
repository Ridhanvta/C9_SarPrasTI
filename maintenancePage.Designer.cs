namespace ManajemenSarPras
{
    partial class maintenancePage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(maintenancePage));
            this.btnKembali = new System.Windows.Forms.Button();
            this.labelNamaRuangan = new System.Windows.Forms.Label();
            this.labelNamaBarang = new System.Windows.Forms.Label();
            this.labelTanggal = new System.Windows.Forms.Label();
            this.labelKondisi = new System.Windows.Forms.Label();
            this.labelKerusakan = new System.Windows.Forms.Label();
            this.labelTindakLanjut = new System.Windows.Forms.Label();
            this.labelSemseter = new System.Windows.Forms.Label();
            this.cmbRuangan = new System.Windows.Forms.ComboBox();
            this.cmbDetailBarang = new System.Windows.Forms.ComboBox();
            this.dtpTglCek = new System.Windows.Forms.DateTimePicker();
            this.rbBaik = new System.Windows.Forms.RadioButton();
            this.rbRusak = new System.Windows.Forms.RadioButton();
            this.txtKerusakan = new System.Windows.Forms.TextBox();
            this.txtTindakLanjut = new System.Windows.Forms.TextBox();
            this.cmbSemester = new System.Windows.Forms.ComboBox();
            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.labelKaryawan = new System.Windows.Forms.Label();
            this.cmbKaryawan = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnHapus = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
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
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
            this.bindingNavigator1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnKembali
            // 
            this.btnKembali.Location = new System.Drawing.Point(28, 26);
            this.btnKembali.Name = "btnKembali";
            this.btnKembali.Size = new System.Drawing.Size(109, 30);
            this.btnKembali.TabIndex = 0;
            this.btnKembali.Text = "Kembali";
            this.btnKembali.UseVisualStyleBackColor = true;
            this.btnKembali.Click += new System.EventHandler(this.btnKembali_Click);
            // 
            // labelNamaRuangan
            // 
            this.labelNamaRuangan.AutoSize = true;
            this.labelNamaRuangan.Location = new System.Drawing.Point(42, 143);
            this.labelNamaRuangan.Name = "labelNamaRuangan";
            this.labelNamaRuangan.Size = new System.Drawing.Size(102, 16);
            this.labelNamaRuangan.TabIndex = 1;
            this.labelNamaRuangan.Text = "Nama Ruangan";
            // 
            // labelNamaBarang
            // 
            this.labelNamaBarang.AutoSize = true;
            this.labelNamaBarang.Location = new System.Drawing.Point(42, 106);
            this.labelNamaBarang.Name = "labelNamaBarang";
            this.labelNamaBarang.Size = new System.Drawing.Size(91, 16);
            this.labelNamaBarang.TabIndex = 2;
            this.labelNamaBarang.Text = "Nama Barang";
            // 
            // labelTanggal
            // 
            this.labelTanggal.AutoSize = true;
            this.labelTanggal.Location = new System.Drawing.Point(42, 186);
            this.labelTanggal.Name = "labelTanggal";
            this.labelTanggal.Size = new System.Drawing.Size(58, 16);
            this.labelTanggal.TabIndex = 3;
            this.labelTanggal.Text = "Tanggal";
            // 
            // labelKondisi
            // 
            this.labelKondisi.AutoSize = true;
            this.labelKondisi.Location = new System.Drawing.Point(42, 214);
            this.labelKondisi.Name = "labelKondisi";
            this.labelKondisi.Size = new System.Drawing.Size(51, 16);
            this.labelKondisi.TabIndex = 4;
            this.labelKondisi.Text = "Kondisi";
            // 
            // labelKerusakan
            // 
            this.labelKerusakan.AutoSize = true;
            this.labelKerusakan.Location = new System.Drawing.Point(42, 244);
            this.labelKerusakan.Name = "labelKerusakan";
            this.labelKerusakan.Size = new System.Drawing.Size(71, 16);
            this.labelKerusakan.TabIndex = 5;
            this.labelKerusakan.Text = "Kerusakan";
            // 
            // labelTindakLanjut
            // 
            this.labelTindakLanjut.AutoSize = true;
            this.labelTindakLanjut.Location = new System.Drawing.Point(42, 273);
            this.labelTindakLanjut.Name = "labelTindakLanjut";
            this.labelTindakLanjut.Size = new System.Drawing.Size(87, 16);
            this.labelTindakLanjut.TabIndex = 6;
            this.labelTindakLanjut.Text = "Tindak Lanjut";
            // 
            // labelSemseter
            // 
            this.labelSemseter.AutoSize = true;
            this.labelSemseter.Location = new System.Drawing.Point(42, 338);
            this.labelSemseter.Name = "labelSemseter";
            this.labelSemseter.Size = new System.Drawing.Size(65, 16);
            this.labelSemseter.TabIndex = 7;
            this.labelSemseter.Text = "Semester";
            // 
            // cmbRuangan
            // 
            this.cmbRuangan.FormattingEnabled = true;
            this.cmbRuangan.Location = new System.Drawing.Point(187, 143);
            this.cmbRuangan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbRuangan.Name = "cmbRuangan";
            this.cmbRuangan.Size = new System.Drawing.Size(178, 24);
            this.cmbRuangan.TabIndex = 8;
            // 
            // cmbDetailBarang
            // 
            this.cmbDetailBarang.FormattingEnabled = true;
            this.cmbDetailBarang.Location = new System.Drawing.Point(187, 103);
            this.cmbDetailBarang.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbDetailBarang.Name = "cmbDetailBarang";
            this.cmbDetailBarang.Size = new System.Drawing.Size(178, 24);
            this.cmbDetailBarang.TabIndex = 9;
            // 
            // dtpTglCek
            // 
            this.dtpTglCek.Location = new System.Drawing.Point(187, 184);
            this.dtpTglCek.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpTglCek.Name = "dtpTglCek";
            this.dtpTglCek.Size = new System.Drawing.Size(178, 22);
            this.dtpTglCek.TabIndex = 10;
            // 
            // rbBaik
            // 
            this.rbBaik.AutoSize = true;
            this.rbBaik.Location = new System.Drawing.Point(187, 214);
            this.rbBaik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbBaik.Name = "rbBaik";
            this.rbBaik.Size = new System.Drawing.Size(55, 20);
            this.rbBaik.TabIndex = 11;
            this.rbBaik.TabStop = true;
            this.rbBaik.Text = "Baik";
            this.rbBaik.UseVisualStyleBackColor = true;
            // 
            // rbRusak
            // 
            this.rbRusak.AutoSize = true;
            this.rbRusak.Location = new System.Drawing.Point(293, 214);
            this.rbRusak.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rbRusak.Name = "rbRusak";
            this.rbRusak.Size = new System.Drawing.Size(67, 20);
            this.rbRusak.TabIndex = 12;
            this.rbRusak.TabStop = true;
            this.rbRusak.Text = "Rusak";
            this.rbRusak.UseVisualStyleBackColor = true;
            // 
            // txtKerusakan
            // 
            this.txtKerusakan.Location = new System.Drawing.Point(187, 244);
            this.txtKerusakan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtKerusakan.Name = "txtKerusakan";
            this.txtKerusakan.Size = new System.Drawing.Size(178, 22);
            this.txtKerusakan.TabIndex = 13;
            // 
            // txtTindakLanjut
            // 
            this.txtTindakLanjut.Location = new System.Drawing.Point(187, 273);
            this.txtTindakLanjut.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTindakLanjut.Name = "txtTindakLanjut";
            this.txtTindakLanjut.Size = new System.Drawing.Size(178, 22);
            this.txtTindakLanjut.TabIndex = 14;
            // 
            // cmbSemester
            // 
            this.cmbSemester.FormattingEnabled = true;
            this.cmbSemester.Location = new System.Drawing.Point(187, 332);
            this.cmbSemester.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbSemester.Name = "cmbSemester";
            this.cmbSemester.Size = new System.Drawing.Size(178, 24);
            this.cmbSemester.TabIndex = 15;
            // 
            // btnSimpan
            // 
            this.btnSimpan.Location = new System.Drawing.Point(45, 373);
            this.btnSimpan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(140, 31);
            this.btnSimpan.TabIndex = 16;
            this.btnSimpan.Text = "Simpan";
            this.btnSimpan.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(191, 373);
            this.btnReset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(173, 32);
            this.btnReset.TabIndex = 17;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(45, 564);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(756, 310);
            this.dataGridView1.TabIndex = 18;
            // 
            // labelKaryawan
            // 
            this.labelKaryawan.AutoSize = true;
            this.labelKaryawan.Location = new System.Drawing.Point(42, 303);
            this.labelKaryawan.Name = "labelKaryawan";
            this.labelKaryawan.Size = new System.Drawing.Size(106, 16);
            this.labelKaryawan.TabIndex = 19;
            this.labelKaryawan.Text = "Nama Karyawan";
            // 
            // cmbKaryawan
            // 
            this.cmbKaryawan.FormattingEnabled = true;
            this.cmbKaryawan.Location = new System.Drawing.Point(187, 300);
            this.cmbKaryawan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmbKaryawan.Name = "cmbKaryawan";
            this.cmbKaryawan.Size = new System.Drawing.Size(178, 24);
            this.cmbKaryawan.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Stencil", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(250, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(329, 32);
            this.label1.TabIndex = 21;
            this.label1.Text = "Maintenance Barang";
            // 
            // btnHapus
            // 
            this.btnHapus.Location = new System.Drawing.Point(45, 410);
            this.btnHapus.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnHapus.Name = "btnHapus";
            this.btnHapus.Size = new System.Drawing.Size(319, 30);
            this.btnHapus.TabIndex = 22;
            this.btnHapus.Text = "Hapus";
            this.btnHapus.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Stencil", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(39, 489);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(426, 32);
            this.label2.TabIndex = 23;
            this.label2.Text = "Riwayat Maintance Barang";
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(386, 132);
            this.dataGridView2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersWidth = 62;
            this.dataGridView2.RowTemplate.Height = 28;
            this.dataGridView2.Size = new System.Drawing.Size(415, 308);
            this.dataGridView2.TabIndex = 24;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(505, 103);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(296, 22);
            this.textBox1.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(382, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(110, 16);
            this.label3.TabIndex = 25;
            this.label3.Text = "Cari Data Barang";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(165, 529);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(636, 22);
            this.textBox2.TabIndex = 28;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 531);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(114, 16);
            this.label4.TabIndex = 27;
            this.label4.Text = "Cari Data Riwayat";
            // 
            // bindingNavigator1
            // 
            this.bindingNavigator1.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
            this.bindingNavigator1.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bindingNavigator1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem});
            this.bindingNavigator1.Location = new System.Drawing.Point(0, 0);
            this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bindingNavigator1.Name = "bindingNavigator1";
            this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
            this.bindingNavigator1.Size = new System.Drawing.Size(835, 27);
            this.bindingNavigator1.TabIndex = 29;
            this.bindingNavigator1.Text = "bindingNavigator1";
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(29, 24);
            this.bindingNavigatorMoveFirstItem.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(29, 24);
            this.bindingNavigatorMovePreviousItem.Text = "Move previous";
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "Position";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 27);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "Current position";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(45, 24);
            this.bindingNavigatorCountItem.Text = "of {0}";
            this.bindingNavigatorCountItem.ToolTipText = "Total number of items";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(29, 24);
            this.bindingNavigatorMoveNextItem.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(29, 24);
            this.bindingNavigatorMoveLastItem.Text = "Move last";
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(29, 24);
            this.bindingNavigatorAddNewItem.Text = "Add new";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(29, 24);
            this.bindingNavigatorDeleteItem.Text = "Delete";
            // 
            // maintenancePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(835, 844);
            this.Controls.Add(this.bindingNavigator1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnHapus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbKaryawan);
            this.Controls.Add(this.labelKaryawan);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSimpan);
            this.Controls.Add(this.cmbSemester);
            this.Controls.Add(this.txtTindakLanjut);
            this.Controls.Add(this.txtKerusakan);
            this.Controls.Add(this.rbRusak);
            this.Controls.Add(this.rbBaik);
            this.Controls.Add(this.dtpTglCek);
            this.Controls.Add(this.cmbDetailBarang);
            this.Controls.Add(this.cmbRuangan);
            this.Controls.Add(this.labelSemseter);
            this.Controls.Add(this.labelTindakLanjut);
            this.Controls.Add(this.labelKerusakan);
            this.Controls.Add(this.labelKondisi);
            this.Controls.Add(this.labelTanggal);
            this.Controls.Add(this.labelNamaBarang);
            this.Controls.Add(this.labelNamaRuangan);
            this.Controls.Add(this.btnKembali);
            this.Name = "maintenancePage";
            this.Text = "mntenncePage";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
            this.bindingNavigator1.ResumeLayout(false);
            this.bindingNavigator1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnKembali;
        private System.Windows.Forms.Label labelNamaRuangan;
        private System.Windows.Forms.Label labelNamaBarang;
        private System.Windows.Forms.Label labelTanggal;
        private System.Windows.Forms.Label labelKondisi;
        private System.Windows.Forms.Label labelKerusakan;
        private System.Windows.Forms.Label labelTindakLanjut;
        private System.Windows.Forms.Label labelSemseter;
        private System.Windows.Forms.ComboBox cmbRuangan;
        private System.Windows.Forms.ComboBox cmbDetailBarang;
        private System.Windows.Forms.DateTimePicker dtpTglCek;
        private System.Windows.Forms.RadioButton rbBaik;
        private System.Windows.Forms.RadioButton rbRusak;
        private System.Windows.Forms.TextBox txtKerusakan;
        private System.Windows.Forms.TextBox txtTindakLanjut;
        private System.Windows.Forms.ComboBox cmbSemester;
        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label labelKaryawan;
        private System.Windows.Forms.ComboBox cmbKaryawan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHapus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
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