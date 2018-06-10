namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableLegalizeBoxes(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_LegalizeBoxes = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_LegalizeBoxes.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.legalizeboxes);
            this.Menu_LegalizeBoxes.Name = "Menu_LegalizeBoxes";
            this.Menu_LegalizeBoxes.ShowShortcutKeys = false;
            this.Menu_LegalizeBoxes.Size = new System.Drawing.Size(231, 22);
            this.Menu_LegalizeBoxes.Text = "Legalize Active Pokemon";
            this.Menu_LegalizeBoxes.Click += new System.EventHandler(this.LegalizeBoxes);
            return this.Menu_LegalizeBoxes;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_LegalizeBoxes;
    }
}