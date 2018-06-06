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
            int.TryParse(Resources.ProgramVersion, out int currrev);
            if (lastrev < currrev) WinFormsUtil.Alert($"Changelog: {currrev}", PKHeX.WinForms.AutoLegality.ConstData.changelog, "Keyboard Shortcuts:", PKHeX.WinForms.AutoLegality.ConstData.keyboardshortcuts, "Discord Link:", PKHeX.WinForms.AutoLegality.ConstData.discord);
            this.Menu_AutoLegality = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_AutoLegality.Image = ((System.Drawing.Image)AutoLegality.AutoLegalityMod.menuautolegality);
            this.Menu_AutoLegality.Name = "Menu_AutoLegality";
            this.Menu_AutoLegality.Size = new System.Drawing.Size(133, 22);
            this.Menu_AutoLegality.Text = "Auto Legality Mod";
            return this.Menu_AutoLegality;
        }
        public void CheckALMUpdate()
        {
            L_UpdateAvailable.Click += (sender, e) => Process.Start("https://github.com/architdate/PKHeX-Auto-Legality-Mod/releases/latest");
            new Task(() =>
            {
                string data = GetPage("https://api.github.com/repos/architdate/pkhex-auto-legality-mod/releases/latest");
                int latestVersion = ParseTagAsVersion(data.Split(new string[] { "\"tag_name\":\"" }, System.StringSplitOptions.None)[1].Split('"')[0]);
                if (data == null || latestVersion == -1)
                    return;
                if (int.TryParse(Resources.ProgramVersion, out var cur) && latestVersion <= cur)
                    return;

                Invoke((MethodInvoker)(() =>
                {
                    L_UpdateAvailable.Visible = true;
                    L_UpdateAvailable.Text = $"New Auto Legality Mod update available! {latestVersion:d}";
                }));
            }).Start();
        }
        public int ParseTagAsVersion(string v)
        {
            string[] date = v.Split('.');
            if (date.Length != 3) return -1;
            int.TryParse(date[0], out int a);
            int.TryParse(date[1], out int b);
            int.TryParse(date[2], out int c);
            return (a + 2000) * 10000 + b * 100 + c;
        }
        private string GetPage(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "AutoLegalityMod";
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (Exception e)
            {
                WinFormsUtil.Alert("An error occured while trying to obtain the contents of the URL. This is most likely an issue with your Internet Connection. The exact error is as follows: " + e.ToString() + "\nURL tried to access: " + url);
                return "Error :" + e.ToString();
            }
        }
        private System.Windows.Forms.ToolStripMenuItem Menu_AutoLegality;
        private System.Windows.Forms.ToolStripMenuItem Menu_DiscordLink;
        private System.Windows.Forms.ToolStripMenuItem Menu_ShowdownImportPKMModded;
    }
}