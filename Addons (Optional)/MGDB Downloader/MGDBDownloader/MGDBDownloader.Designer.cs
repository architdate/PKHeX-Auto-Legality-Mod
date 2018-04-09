using System;
namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableMGDBDownloader(System.ComponentModel.ComponentResourceManager resources, bool LatestCommit = false)
        {
            this.Menu_MGDBDownloader = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_MGDBDownloader.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.mgdbdownload);
            this.Menu_MGDBDownloader.Name = "Menu_MGDBDownloader";
            this.Menu_MGDBDownloader.Size = new System.Drawing.Size(231, 22);
            this.Menu_MGDBDownloader.Text = "Download MGDB";
            this.Menu_MGDBDownloader.Click += delegate(object sender, EventArgs e) { this.DownloadMGDB(sender, e, LatestCommit); };
            return this.Menu_MGDBDownloader;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_MGDBDownloader;
    }
}