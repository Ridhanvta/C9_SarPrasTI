namespace ManajemenSarPras
{
    partial class grafik
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea6 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend6 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series6 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTahunAjaran = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chartBarang = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.satprasDBDataSet = new ManajemenSarPras.satprasDBDataSet();
            this.semesterBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.semesterTableAdapter = new ManajemenSarPras.satprasDBDataSetTableAdapters.semesterTableAdapter();
            this.semesterBindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.chartBarang)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.satprasDBDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.semesterBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.semesterBindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(382, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(365, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Rekap Pengeluaran Barang";
            // 
            // cmbTahunAjaran
            // 
            this.cmbTahunAjaran.FormattingEnabled = true;
            this.cmbTahunAjaran.Location = new System.Drawing.Point(205, 66);
            this.cmbTahunAjaran.Name = "cmbTahunAjaran";
            this.cmbTahunAjaran.Size = new System.Drawing.Size(219, 24);
            this.cmbTahunAjaran.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(64, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(108, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tahun Ajaran";
            // 
            // chartBarang
            // 
            chartArea6.Name = "ChartArea1";
            this.chartBarang.ChartAreas.Add(chartArea6);
            legend6.Name = "Legend1";
            this.chartBarang.Legends.Add(legend6);
            this.chartBarang.Location = new System.Drawing.Point(68, 126);
            this.chartBarang.Name = "chartBarang";
            series6.ChartArea = "ChartArea1";
            series6.Legend = "Legend1";
            series6.Name = "Series1";
            this.chartBarang.Series.Add(series6);
            this.chartBarang.Size = new System.Drawing.Size(976, 395);
            this.chartBarang.TabIndex = 3;
            this.chartBarang.Text = "Pengeluaran Barang";
            // 
            // btnLoad
            // 
            this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoad.Location = new System.Drawing.Point(589, 66);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(108, 34);
            this.btnLoad.TabIndex = 4;
            this.btnLoad.Text = "Load";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(703, 65);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(108, 35);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnDashboard
            // 
            this.btnDashboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDashboard.Location = new System.Drawing.Point(936, 64);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(108, 35);
            this.btnDashboard.TabIndex = 6;
            this.btnDashboard.Text = "Dashboard";
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // satprasDBDataSet
            // 
            this.satprasDBDataSet.DataSetName = "satprasDBDataSet";
            this.satprasDBDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // semesterBindingSource
            // 
            this.semesterBindingSource.DataMember = "semester";
            this.semesterBindingSource.DataSource = this.satprasDBDataSet;
            // 
            // semesterTableAdapter
            // 
            this.semesterTableAdapter.ClearBeforeFill = true;
            // 
            // semesterBindingSource1
            // 
            this.semesterBindingSource1.DataMember = "semester";
            this.semesterBindingSource1.DataSource = this.satprasDBDataSet;
            // 
            // grafik
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1102, 549);
            this.Controls.Add(this.btnDashboard);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.chartBarang);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbTahunAjaran);
            this.Controls.Add(this.label1);
            this.Name = "grafik";
            this.Text = "grafik";
            this.Load += new System.EventHandler(this.grafik_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chartBarang)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.satprasDBDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.semesterBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.semesterBindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTahunAjaran;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartBarang;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnDashboard;
        private satprasDBDataSet satprasDBDataSet;
        private System.Windows.Forms.BindingSource semesterBindingSource;
        private satprasDBDataSetTableAdapters.semesterTableAdapter semesterTableAdapter;
        private System.Windows.Forms.BindingSource semesterBindingSource1;
    }
}