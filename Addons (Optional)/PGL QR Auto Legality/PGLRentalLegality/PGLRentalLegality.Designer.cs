namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnablePGLRentalLegality(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_PGLShowdownSet = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_PGLShowdownSet.Image = ((System.Drawing.Image)(resources.GetObject("Menu_ShowdownExportPKM.Image")));
            this.Menu_PGLShowdownSet.Name = "Menu_PGLShowdownSet";
            this.Menu_PGLShowdownSet.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Q)));
            this.Menu_PGLShowdownSet.ShowShortcutKeys = false;
            this.Menu_PGLShowdownSet.Size = new System.Drawing.Size(231, 22);
            this.Menu_PGLShowdownSet.Text = "Import PGL QR code";
            this.Menu_PGLShowdownSet.Click += new System.EventHandler(this.PGLShowdownSet);
            return this.Menu_PGLShowdownSet;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_PGLShowdownSet;
    }
}