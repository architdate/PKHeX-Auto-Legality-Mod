namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableAutoLegality(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_ShowdownImportPKMModded = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_ShowdownImportPKMModded.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.autolegalitymod);
            this.Menu_ShowdownImportPKMModded.Name = "Menu_ShowdownImportPKMModded";
            this.Menu_ShowdownImportPKMModded.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.Menu_ShowdownImportPKMModded.ShowShortcutKeys = false;
            this.Menu_ShowdownImportPKMModded.Size = new System.Drawing.Size(231, 22);
            this.Menu_ShowdownImportPKMModded.Text = "Import with Auto-Legality Mod";
            this.Menu_ShowdownImportPKMModded.Click += new System.EventHandler(this.ClickShowdownImportPKMModded);
            return this.Menu_ShowdownImportPKMModded;
        }
        public System.Windows.Forms.ToolStripMenuItem EnableMenu(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_AutoLegality = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_AutoLegality.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.menuautolegality);
            this.Menu_AutoLegality.Name = "Menu_AutoLegality";
            this.Menu_AutoLegality.Size = new System.Drawing.Size(133, 22);
            this.Menu_AutoLegality.Text = "Auto Legality Mod";
            return this.Menu_AutoLegality;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_AutoLegality;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownImportPKMModded;
    }
}