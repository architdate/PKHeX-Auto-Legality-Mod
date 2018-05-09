namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableExportQRCodes(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_ExportQRCodes = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_ExportQRCodes.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.pglqrcode);
            this.Menu_ExportQRCodes.Name = "Menu_ExportQRCode";
            this.Menu_ExportQRCodes.Size = new System.Drawing.Size(231, 22);
            this.Menu_ExportQRCodes.Text = "Export Box QR Codes";
            this.Menu_ExportQRCodes.Click += new System.EventHandler(this.ExportQRCodes);
            return this.Menu_ExportQRCodes;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_ExportQRCodes;
    }
}