namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableURLGenning(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_URLGenning = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_URLGenning.Image = ((System.Drawing.Image)(resources.GetObject("Menu_ShowdownImportPKM.Image")));
            this.Menu_URLGenning.Name = "Menu_URLGenning";
            this.Menu_URLGenning.Size = new System.Drawing.Size(231, 22);
            this.Menu_URLGenning.Text = "Gen from URL";
            this.Menu_URLGenning.Click += new System.EventHandler(this.URLGen);
            return this.Menu_URLGenning;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_URLGenning;
    }
}