namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem EnableExportTrainingData(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_ExportTrainerData = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_ExportTrainerData.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.exporttrainerdata);
            this.Menu_ExportTrainerData.Name = "Menu_ExportTrainerData";
            this.Menu_ExportTrainerData.Size = new System.Drawing.Size(231, 22);
            this.Menu_ExportTrainerData.Text = "Export Trainer Data";
            this.Menu_ExportTrainerData.Click += new System.EventHandler(this.ExportTrainingData);
            return this.Menu_ExportTrainerData;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_ExportTrainerData;
    }
}