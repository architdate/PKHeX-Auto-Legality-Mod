namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableSmogonGenner(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_SmogonGenner = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_SmogonGenner.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.smogongenner);
            this.Menu_SmogonGenner.Name = "Menu_SmogonGenner";
            this.Menu_SmogonGenner.Size = new System.Drawing.Size(231, 22);
            this.Menu_SmogonGenner.Text = "Gen Smogon Sets";
            this.Menu_SmogonGenner.Click += new System.EventHandler(this.SmogonGenner);
            return this.Menu_SmogonGenner;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_SmogonGenner;
    }
}