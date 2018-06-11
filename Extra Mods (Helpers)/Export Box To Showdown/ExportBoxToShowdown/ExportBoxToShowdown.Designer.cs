namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableExportBoxToShowdown(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_ExportBoxToShowdown = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_ExportBoxToShowdown.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.exporttrainerdata);
            this.Menu_ExportBoxToShowdown.Name = "Menu_ExportBoxToShowdown";
            this.Menu_ExportBoxToShowdown.Size = new System.Drawing.Size(231, 22);
            this.Menu_ExportBoxToShowdown.Text = "Export Box to Showdown";
            this.Menu_ExportBoxToShowdown.Click += new System.EventHandler(this.ExportBoxToShowdown);
            return this.Menu_ExportBoxToShowdown;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_ExportBoxToShowdown;
    }
}