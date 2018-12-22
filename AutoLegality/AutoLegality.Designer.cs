using PKHeX.WinForms.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PKHeX.WinForms
{
    partial class Main
    {
        public System.Windows.Forms.ToolStripMenuItem DiscordLink(System.ComponentModel.ComponentResourceManager resources)
        {
            this.Menu_DiscordLink = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_DiscordLink.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.discord);
            this.Menu_DiscordLink.Name = "Menu_DiscordLink";
            this.Menu_DiscordLink.Size = new System.Drawing.Size(133, 22);
            this.Menu_DiscordLink.Text = "Discord Server";
            this.Menu_DiscordLink.Click += new System.EventHandler(this.ShowDiscordForm);
            return this.Menu_DiscordLink;
        }
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
            var Settings = Properties.Settings.Default;
            int.TryParse(Settings.Version, out int lastrev);
            int.TryParse(CurrentProgramVersion.ToString(), out int currrev);
            if (lastrev < currrev) WinFormsUtil.Alert($"Changelog: {currrev}", PKHeX.WinForms.AutoLegality.ConstData.changelog, "Keyboard Shortcuts:", PKHeX.WinForms.AutoLegality.ConstData.keyboardshortcuts, "Discord Link:", PKHeX.WinForms.AutoLegality.ConstData.discord);
            this.Menu_AutoLegality = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_AutoLegality.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.menuautolegality);
            this.Menu_AutoLegality.Name = "Menu_AutoLegality";
            this.Menu_AutoLegality.Size = new System.Drawing.Size(133, 22);
            this.Menu_AutoLegality.Text = "Auto Legality Mod";
            return this.Menu_AutoLegality;
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_AutoLegality;
        private System.Windows.Forms.ToolStripMenuItem Menu_DiscordLink;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownImportPKMModded;
    }
}