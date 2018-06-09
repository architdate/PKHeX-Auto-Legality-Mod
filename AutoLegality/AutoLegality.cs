using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net;
using PKHeX.WinForms.Properties;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {

        /// <summary>
        /// Global Variables for Auto Legality Mod
        /// </summary>
        int TID_ALM = -1;
        int SID_ALM = -1;
        string OT_ALM = "";
        int gender_ALM = 0;
        string Country_ALM = "";
        string SubRegion_ALM = "";
        string ConsoleRegion_ALM = "";
        bool APILegalized = false;

        /// <summary>
        /// Main function to be called by the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ClickShowdownImportPKMModded(object sender, EventArgs e)
        {
            CheckALMUpdate(); // Check for Auto Legality Mod Updates
            bool allowAPI = true; // Use true to allow experimental API usage
            APILegalized = false; // Initialize to false everytime command is used

            // Check for lack of showdown data provided
            CheckLoadFromText(out bool valid);
            if (!valid) return;

            // Make a blank MGDB directory and initialize trainerdata
            if (!Directory.Exists(MGDatabasePath)) Directory.CreateDirectory(MGDatabasePath);
            if (PKME_Tabs.checkMode() != "game") LoadTrainerData();

            // Get Text source from clipboard and convert to ShowdownSet(s)
            string source = Clipboard.GetText().TrimEnd();
            List<ShowdownSet> Sets = ShowdownSets(source);

            // Import Showdown Sets and alert user of any messages intended
            ImportSets(Sets, (ModifierKeys & Keys.Control) == Keys.Control, out string message, allowAPI);
            if (message.StartsWith("[DEBUG]")) Debug.WriteLine(message);
            else WinFormsUtil.Alert(message);
        }

        /// <summary>
        /// Check whether the showdown text is supposed to be loaded via a text file. If so, set the clipboard to its contents.
        /// </summary>
        /// <param name="valid">output boolean that tells if the data provided is valid or not</param>
        private void CheckLoadFromText(out bool valid)
        {
            valid = true;
            if (!showdownData() || (ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (WinFormsUtil.OpenSAVPKMDialog(new string[] { "txt" }, out string path))
                {
                    Clipboard.SetText(File.ReadAllText(path).TrimEnd());
                    if (!showdownData())
                    {
                        WinFormsUtil.Alert("Text file with invalid data provided. Please provide a text file with proper Showdown data");
                        valid = false;
                        return;
                    }
                }
                else
                {
                    WinFormsUtil.Alert("No data provided.");
                    valid = false;
                    return;
                }
            }
        }

        /// <summary>
        /// Loads the trainerdata variables into the global variables for AutoLegalityMod
        /// </summary>
        /// <param name="legal">Optional legal PKM for loading trainerdata on a per game basis</param>
        private void LoadTrainerData(PKM legal = null)
        {
            bool checkPerGame = (PKME_Tabs.checkMode() == "game");
            // If mode is not set as game: (auto or save)
            string[] tdataVals;
            if(!checkPerGame || legal == null) tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV);

            else tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV, legal.Version);
            TID_ALM = Convert.ToInt32(tdataVals[0]);
            SID_ALM = Convert.ToInt32(tdataVals[1]);
            if(legal != null)
                SID_ALM = legal.VC ? 0 : SID_ALM;
            OT_ALM = tdataVals[2];
            if (OT_ALM == "PKHeX") OT_ALM = "Archit(TCD)"; // Avoids secondary handler error
            gender_ALM = 0;
            if (tdataVals[3] == "F" || tdataVals[3] == "Female") gender_ALM = 1;
            Country_ALM = tdataVals[4];
            SubRegion_ALM = tdataVals[5];
            ConsoleRegion_ALM = tdataVals[6];
            if((checkPerGame && legal != null) || APILegalized)
                legal = PKME_Tabs.SetTrainerData(OT_ALM, TID_ALM, SID_ALM, gender_ALM, legal, APILegalized);
        }

        /// <summary>
        /// Function that generates legal PKM objects from ShowdownSets and views them/sets them in boxes
        /// </summary>
        /// <param name="sets">A list of ShowdownSet(s) that need to be genned</param>
        /// <param name="replace">A boolean that determines if current pokemon will be replaced or not</param>
        /// <param name="message">Output message to be displayed for the user</param>
        /// <param name="allowAPI">Use of generators before bruteforcing</param>
        private void ImportSets(List<ShowdownSet> sets, bool replace, out string message, bool allowAPI = true)
        {
            message = "[DEBUG] Commencing Import";
            List<int> emptySlots = new List<int> { };
            IList<PKM> BoxData = C_SAV.SAV.BoxData;
            int BoxOffset = C_SAV.CurrentBox * C_SAV.SAV.BoxSlotCount;
            if (replace) emptySlots = Enumerable.Range(0, sets.Count).ToList();
            else emptySlots = PopulateEmptySlots(BoxData, C_SAV.CurrentBox);
            if (emptySlots.Count < sets.Count) { message = "Not enough space in the box"; return; }
            int apiCounter = 0;
            List<ShowdownSet> invalidAPISets = new List<ShowdownSet>();
            for (int i = 0; i < sets.Count; i++)
            {
                ShowdownSet Set = sets[i];
                if (sets.Count == 1 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Import this set?", Set.Text))
                    return;
                if (Set.InvalidLines.Count > 0)
                    WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));
                bool resetForm = false;
                PKME_Tabs.hardReset(C_SAV.SAV);
                if (Set.Form != null && (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")) resetForm = true;
                PKM roughPKM = PreparePKM();
                roughPKM.ApplySetDetails(Set);
                roughPKM.Version = (int)GameVersion.MN; // Avoid the blank version glitch
                PKM legal = C_SAV.SAV.BlankPKM;
                bool satisfied = false;
                if (allowAPI)
                {
                    AutoLegalityMod mod = new AutoLegalityMod();
                    mod.SAV = C_SAV.SAV;
                    PKM APIGeneratedPKM = C_SAV.SAV.BlankPKM;
                    try { APIGeneratedPKM = mod.APILegality(roughPKM, Set, out satisfied); }
                    catch { satisfied = false; }
                    if (satisfied) {
                        legal = APIGeneratedPKM;
                        apiCounter++;
                        APILegalized = true;
                    }
                }
                if (!allowAPI || !satisfied)
                {
                    invalidAPISets.Add(Set);
                    Blah b = new Blah { C_SAV = C_SAV };
                    legal = b.LoadShowdownSetModded_PKSM(roughPKM, Set, resetForm, TID_ALM, SID_ALM, OT_ALM, gender_ALM);
                    APILegalized = false;
                }
                PKM pk = SetTrainerData(legal);
                BoxData[BoxOffset + emptySlots[i]] = pk;
            }
            if (sets.Count > 1)
            {
                C_SAV.SAV.BoxData = BoxData;
                C_SAV.ReloadSlots();
                message = "[DEBUG] API Genned Sets: " + apiCounter + Environment.NewLine + Environment.NewLine + "Number of sets not genned by the API: " + invalidAPISets.Count;
                foreach (ShowdownSet i in invalidAPISets) Debug.WriteLine(i.Text);
            }
            else message = "[DEBUG] Set Genning Complete";
        }

        /// <summary>
        /// Set trainer data for a legal PKM
        /// </summary>
        /// <param name="legal">Legal PKM for setting the data</param>
        /// <returns>PKM with the necessary values modified to reflect trainerdata changes</returns>
        private PKM SetTrainerData(PKM legal)
        {
            bool intRegions = false;
            LoadTrainerData(legal);
            if (int.TryParse(Country_ALM, out int n) && int.TryParse(SubRegion_ALM, out int m) && int.TryParse(ConsoleRegion_ALM, out int o))
            {
                legal = PKME_Tabs.SetPKMRegions(n, m, o, legal);
                intRegions = true;
            }
            PKME_Tabs.PopulateFields(legal);
            if (!intRegions)
            {
                PKME_Tabs.SetRegions(Country_ALM, SubRegion_ALM, ConsoleRegion_ALM);
            }
            return PreparePKM();
        }

        /// <summary>
        /// Method to find all empty slots in a current box
        /// </summary>
        /// <param name="BoxData">Box Data of the SAV file</param>
        /// <param name="CurrentBox">Index of the current box</param>
        /// <returns>A list of all indices in the current box that are empty</returns>
        private List<int> PopulateEmptySlots(IList<PKM> BoxData, int CurrentBox)
        {
            List<int> emptySlots = new List<int>();
            int BoxCount = C_SAV.SAV.BoxSlotCount;
            for (int i = 0; i < BoxCount; i++)
            {
                if (BoxData[CurrentBox * BoxCount + i].Species < 1) emptySlots.Add(i);
            }
            return emptySlots;
        }

        /// <summary>
        /// A method to get a list of ShowdownSet(s) from a string paste
        /// Needs to be extended to hold several teams
        /// </summary>
        /// <param name="paste"></param>
        /// <returns></returns>
        private List<ShowdownSet> ShowdownSets(string paste)
        {
            paste = paste.Trim(); // Remove White Spaces
            string[] lines = paste.Split(new string[] { "\n" }, StringSplitOptions.None);
            return ShowdownSet.GetShowdownSets(lines).ToList();
        }

        /// <summary>
        /// Checks the input text is a showdown set or not
        /// </summary>
        /// <returns>boolean of the summary</returns>
        private bool showdownData()
        {
            if (!Clipboard.ContainsText()) return false;
            string source = Clipboard.GetText().TrimEnd();
            string[] stringSeparators = new string[] { "\n\r" };
            string[] result;

            // ...
            result = source.Split(stringSeparators, StringSplitOptions.None);
            if (new ShowdownSet(result[0]).Species < 0) return false;
            return true;
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
            catch { }
        }

        /// <summary>
        /// Parse release GitHub tag into a PKHeX style version
        /// </summary>
        /// <param name="v">Tag String</param>
        /// <returns>PKHeX style version int</returns>
        public int ParseTagAsVersion(string v)
        {
            string[] date = v.Split('.');
            if (date.Length != 3) return -1;
            int.TryParse(date[0], out int a);
            int.TryParse(date[1], out int b);
            int.TryParse(date[2], out int c);
            return (a + 2000) * 10000 + b * 100 + c;
        }

        /// <summary>
        /// GET request to a url with UserAgent Header being AutoLegalityMod
        /// </summary>
        /// <param name="url">URL on which the GET request is to be executed</param>
        /// <returns>GET Response</returns>
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
    }
}