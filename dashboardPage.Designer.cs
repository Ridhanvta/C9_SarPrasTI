namespace ManajemenSarPras
{
    partial class dashboardPage
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.kelolaBarang = new System.Windows.Forms.Button();
            this.permintaanBarang = new System.Windows.Forms.Button();
            this.labelKaryawan = new System.Windows.Forms.Button();
            this.maintennce = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSemester = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnGrafik = new System.Windows.Forms.Button();
            this.btnReportStok = new System.Windows.Forms.Button();
            this.btnReportLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // kelolaBarang
            // 
            this.kelolaBarang.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.kelolaBarang.Location = new System.Drawing.Point(174, 199);
            this.kelolaBarang.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.kelolaBarang.Name = "kelolaBarang";
            this.kelolaBarang.Size = new System.Drawing.Size(180, 50);
            this.kelolaBarang.TabIndex = 0;
            this.kelolaBarang.Text = "Kelola Barang";
            this.kelolaBarang.UseVisualStyleBackColor = true;
            this.kelolaBarang.Click += new System.EventHandler(this.kelolaBarang_Click);
            // 
            // permintaanBarang
            // 
            this.permintaanBarang.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.permintaanBarang.Location = new System.Drawing.Point(174, 322);
            this.permintaanBarang.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.permintaanBarang.Name = "permintaanBarang";
            this.permintaanBarang.Size = new System.Drawing.Size(180, 50);
            this.permintaanBarang.TabIndex = 1;
            this.permintaanBarang.Text = "Permintaan Barang";
            this.permintaanBarang.UseVisualStyleBackColor = true;
            this.permintaanBarang.Click += new System.EventHandler(this.permintaanBarang_Click);
            // 
            // labelKaryawan
            // 
            this.labelKaryawan.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelKaryawan.Location = new System.Drawing.Point(384, 199);
            this.labelKaryawan.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.labelKaryawan.Name = "labelKaryawan";
            this.labelKaryawan.Size = new System.Drawing.Size(180, 50);
            this.labelKaryawan.TabIndex = 2;
            this.labelKaryawan.Text = "Karyawan";
            this.labelKaryawan.UseVisualStyleBackColor = true;
            this.labelKaryawan.Click += new System.EventHandler(this.pengecekanRutin_Click);
            // 
            // maintennce
            // 
            this.maintennce.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.maintennce.Location = new System.Drawing.Point(174, 262);
            this.maintennce.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.maintennce.Name = "maintennce";
            this.maintennce.Size = new System.Drawing.Size(180, 50);
            this.maintennce.TabIndex = 3;
            this.maintennce.Text = "Maintenance";
            this.maintennce.UseVisualStyleBackColor = true;
            this.maintennce.Click += new System.EventHandler(this.maintennce_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Stencil", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(52, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(900, 85);
            this.label1.TabIndex = 5;
            this.label1.Text = "SELAMAT DATANG ADMIN";
            // 
            // btnSemester
            // 
            this.btnSemester.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSemester.Location = new System.Drawing.Point(384, 262);
            this.btnSemester.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSemester.Name = "btnSemester";
            this.btnSemester.Size = new System.Drawing.Size(180, 50);
            this.btnSemester.TabIndex = 6;
            this.btnSemester.Text = "Semester";
            this.btnSemester.UseVisualStyleBackColor = true;
            this.btnSemester.Click += new System.EventHandler(this.btnSemester_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(570, 199);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 229);
            this.button1.TabIndex = 7;
            this.button1.Text = "Koneksi ke Database";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // btnGrafik
            // 
            this.btnGrafik.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrafik.Location = new System.Drawing.Point(388, 378);
            this.btnGrafik.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnGrafik.Name = "btnGrafik";
            this.btnGrafik.Size = new System.Drawing.Size(176, 50);
            this.btnGrafik.TabIndex = 8;
            this.btnGrafik.Text = "Grafik";
            this.btnGrafik.UseVisualStyleBackColor = true;
            this.btnGrafik.Click += new System.EventHandler(this.btnGrafik_Click);
            // 
            // btnReportStok
            // 
            this.btnReportStok.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportStok.Location = new System.Drawing.Point(384, 323);
            this.btnReportStok.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReportStok.Name = "btnReportStok";
            this.btnReportStok.Size = new System.Drawing.Size(180, 50);
            this.btnReportStok.TabIndex = 9;
            this.btnReportStok.Text = "Report Stok";
            this.btnReportStok.UseVisualStyleBackColor = true;
            this.btnReportStok.Click += new System.EventHandler(this.btnReportStok_Click);
            // 
            // btnReportLog
            // 
            this.btnReportLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportLog.Location = new System.Drawing.Point(174, 378);
            this.btnReportLog.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnReportLog.Name = "btnReportLog";
            this.btnReportLog.Size = new System.Drawing.Size(180, 50);
            this.btnReportLog.TabIndex = 10;
            this.btnReportLog.Text = "Report Log";
            this.btnReportLog.UseVisualStyleBackColor = true;
            this.btnReportLog.Click += new System.EventHandler(this.btnReportLog_Click);
            // 
            // dashboardPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 562);
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Controls.Add(this.btnReportLog);
            this.Controls.Add(this.btnReportStok);
            this.Controls.Add(this.btnGrafik);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSemester);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.maintennce);
            this.Controls.Add(this.labelKaryawan);
            this.Controls.Add(this.permintaanBarang);
            this.Controls.Add(this.kelolaBarang);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "dashboardPage";
            this.Load += new System.EventHandler(this.dashboardPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button kelolaBarang;
        private System.Windows.Forms.Button permintaanBarang;
        private System.Windows.Forms.Button labelKaryawan;
        private System.Windows.Forms.Button maintennce;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSemester;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Button btnGrafik;
        private System.Windows.Forms.Button btnReportStok;
        private System.Windows.Forms.Button btnReportLog;
    }
}