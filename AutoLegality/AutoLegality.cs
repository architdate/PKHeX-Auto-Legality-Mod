using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using PKHeX.WinForms.Properties;
using AutoLegalityModMain;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        /// <summary>
        /// Main function to be called by the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClickShowdownImportPKMModded(object sender, EventArgs e)
        {
            CheckALMUpdate(); // Check for Auto Legality Mod Updates
            AutomaticLegality.C_SAV = C_SAV;
            AutomaticLegality.PKME_Tabs = PKME_Tabs;
            AutomaticLegality.ImportModded(); // Finish the job
        }

        /// <summary>
        /// Method to show the discord invite with a banner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowDiscordForm(object sender, EventArgs e)
        {
            Form DiscordForm = new Form();
            DiscordForm.Text = "Join the Discord Server";
            DiscordForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            DiscordForm.AutoSize = true;
            PictureBox discord = new PictureBox();
            discord.Size = new System.Drawing.Size(320, 270);
            discord.SizeMode = PictureBoxSizeMode.StretchImage;
            discord.Load("https://canary.discordapp.com/api/guilds/401014193211441153/widget.png?style=banner4");
            discord.MouseClick += new MouseEventHandler((o, a) => Process.Start(AutoLegality.ConstData.discord));
            DiscordForm.Controls.Add(discord);
            DiscordForm.ShowDialog();
        }

        /// <summary>
        /// Method to check for updates to AutoLegalityMod
        /// </summary>
        public void CheckALMUpdate()
        {
            L_UpdateAvailable.Click += (sender, e) => Process.Start("https://github.com/architdate/PKHeX-Auto-Legality-Mod/releases/latest");
            try
            {
                new Task(() =>
                {
                    string data = AutomaticLegality.GetPage("https://api.github.com/repos/architdate/pkhex-auto-legality-mod/releases/latest");
                    if (data.StartsWith("Error")) return;
                    int latestVersion = AutomaticLegality.ParseTagAsVersion(data.Split(new string[] { "\"tag_name\":\"" }, System.StringSplitOptions.None)[1].Split('"')[0]);
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
            catch { }
        }
    }
}