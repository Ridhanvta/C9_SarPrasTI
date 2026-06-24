namespace ManajemenSarPras
{
    partial class cetakDataLog
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
            this.crvLog = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.SuspendLayout();
            // 
            // crvLog
            // 
            this.crvLog.ActiveViewIndex = -1;
            this.crvLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.crvLog.Cursor = System.Windows.Forms.Cursors.Default;
            this.crvLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.crvLog.Location = new System.Drawing.Point(0, 0);
            this.crvLog.Name = "crvLog";
            this.crvLog.ShowCloseButton = false;
            this.crvLog.ShowCopyButton = false;
            this.crvLog.ShowGroupTreeButton = false;
            this.crvLog.ShowParameterPanelButton = false;
            this.crvLog.ShowRefreshButton = false;
            this.crvLog.Size = new System.Drawing.Size(1069, 599);
            this.crvLog.TabIndex = 0;
            this.crvLog.ToolPanelView = CrystalDecisions.Windows.Forms.ToolPanelViewType.None;
            // 
            // cetakDataLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1069, 599);
            this.Controls.Add(this.crvLog);
            this.Name = "cetakDataLog";
            this.Text = "cetakDataLog";
            this.ResumeLayout(false);

        }

        #endregion

        public CrystalDecisions.Windows.Forms.CrystalReportViewer crvLog;
        private rptLog reportLog1;
    }
}