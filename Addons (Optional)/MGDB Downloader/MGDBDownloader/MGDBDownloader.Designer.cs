using System;
namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableMGDBDownloader(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_MGDBDownloader = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_MGDBDownloader.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.mgdbdownload);
            this.Menu_MGDBDownloader.Name = "Menu_MGDBDownloader";
            this.Menu_MGDBDownloader.Size = new System.Drawing.Size(231, 22);
            this.Menu_MGDBDownloader.Text = "Download MGDB";
            this.Menu_MGDBDownloader.Click += new System.EventHandler(this.DownloadMGDB);
            return this.Menu_MGDBDownloader;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_MGDBDownloader;
    }
}