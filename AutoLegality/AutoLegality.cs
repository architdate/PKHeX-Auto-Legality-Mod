using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;
using System.Net;
using System.IO.Compression;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        int TID_ALM = -1;
        int SID_ALM = -1;
        string OT_ALM = "";
        int gender_ALM = 0;
        string Country_ALM = "";
        string SubRegion_ALM = "";
        string ConsoleRegion_ALM = "";
        bool APILegalized = false;

        public void ClickShowdownImportPKMModded(object sender, EventArgs e)
        {
            #region Initial Setup

            bool allowAPI = true; // Use true to allow experimental API usage
            APILegalized = false; // Initialize to false everytime command is used
            if (!showdownData() || (ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (WinFormsUtil.OpenSAVPKMDialog(new string[] { "txt" }, out string path))
                {
                    Clipboard.SetText(File.ReadAllText(path).TrimEnd());
                    if (!showdownData())
                    {
                        WinFormsUtil.Alert("Text file with invalid data provided. Please provide a text file with proper Showdown data");
                        return;
                    }
                }
                else
                {
                    WinFormsUtil.Alert("No data provided.");
                    return;
                }
            }

            if (!Directory.Exists(MGDatabasePath)) Directory.CreateDirectory(MGDatabasePath);
            
            if (PKME_Tabs.checkMode() != "game")
            {
                LoadTrainerData();
            }

            string source = Clipboard.GetText().TrimEnd();
            string[] stringSeparators = new string[] { "\n\r" };
            string[] result;

            // ...
            result = source.Split(stringSeparators, StringSplitOptions.None);
            if (allowAPI)
            {
                List<string> resList = result.OfType<string>().ToList();
                resList.RemoveAll(r => r.Trim() == "");
                result = resList.ToArray();
            }
            Console.WriteLine(result.Length);

            #endregion
            #region Multiple Pokemon Import
            if (result.Length > 1)
            {
                List<int> emptySlots = new List<int> { };
                IList<PKM> BoxData = C_SAV.SAV.BoxData;
                if ((ModifierKeys & Keys.Control) == Keys.Control) // Hold Ctrl while clicking to replace
                {
                    for (int i = 0; i < result.Length; i++) emptySlots.Add(i);
                }
                else
                {
                    for (int i = 0; i < C_SAV.Box.BoxSlotCount; i++)
                    {
                        if ((C_SAV.Box.SlotPictureBoxes[i] as PictureBox)?.Image == null) emptySlots.Add(i);
                    }
                    if (emptySlots.Count < result.Length)
                    {
                        WinFormsUtil.Alert("Not enough space in the box");
                        return;
                    }
                }
                int ctrapi = 0;
                List<string> setsungenned = new List<string>();
                for (int i = 0; i < result.Length; i++)
                {
                    ShowdownSet Set = new ShowdownSet(result[i]);
                    bool intRegions = false;
                    if (Set.InvalidLines.Any())
                        WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

                    // Set Species & Nickname
                    bool resetForm = false;
                    PKME_Tabs.hardReset(C_SAV.SAV);
                    if (Set.Form == null) { }
                    else if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")
                    {
                        resetForm = true;
                        Console.WriteLine(Set.Species);
                    }
                    PKME_Tabs.LoadShowdownSet(Set);
                    PKM p = PreparePKM();
                    p.Version = (int)GameVersion.MN;
                    PKM legal;
                    if (allowAPI)
                    {
                        AutoLegalityMod mod = new AutoLegalityMod();
                        mod.SAV = C_SAV.SAV;
                        bool satisfied = false;
                        PKM APIGenerated = C_SAV.SAV.BlankPKM;
                        try { APIGenerated = mod.APILegality(p, Set, out satisfied); }
                        catch { satisfied = false; }
                        if (!satisfied)
                        {
                            setsungenned.Add(Set.Text);
                            Blah b = new Blah();
                            b.C_SAV = C_SAV;
                            legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID_ALM, SID_ALM, OT_ALM, gender_ALM);
                            APILegalized = false;
                        }
                        else
                        {
                            ctrapi++;
                            legal = APIGenerated;
                            APILegalized = true;
                        }
                    }
                    else
                    {
                        Blah b = new Blah();
                        b.C_SAV = C_SAV;
                        legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID_ALM, SID_ALM, OT_ALM, gender_ALM);
                        APILegalized = false;
                    }
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
                    PKM pk = PreparePKM();
                    BoxData[C_SAV.CurrentBox * C_SAV.SAV.BoxSlotCount + emptySlots[i]] = pk;
                }
                C_SAV.SAV.BoxData = BoxData;
                C_SAV.ReloadSlots();
#if DEBUG
                WinFormsUtil.Alert("API Genned Sets: " + ctrapi + Environment.NewLine + Environment.NewLine + "Number of sets not genned by the API: " + setsungenned.Count);
                Console.WriteLine(String.Join("\n\n", setsungenned));
#endif
            }
            #endregion
            #region Single Pokemon Import
            else
            {
                // Get Simulator Data
                ShowdownSet Set = new ShowdownSet(Clipboard.GetText());

                if (Set.Species < 0)
                { WinFormsUtil.Alert("Set data not found in clipboard."); return; }

                if (Set.Nickname?.Length > C_SAV.SAV.NickLength)
                    Set.Nickname = Set.Nickname.Substring(0, C_SAV.SAV.NickLength);

                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Import this set?", Set.Text))
                    return;

                if (Set.InvalidLines.Any())
                    WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

                // Set Species & Nickname
                //PKME_Tabs.LoadShowdownSet(Set);
                bool resetForm = false;
                PKME_Tabs.hardReset(C_SAV.SAV);
                
                if (Set.Form == null) { }
                else if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")
                {
                    Set = new ShowdownSet(Set.Text.Replace("-" + Set.Form, ""));
                    resetForm = true;
                    Console.WriteLine(Set.Species);
                }
                PKME_Tabs.LoadShowdownSet(Set);
                PKM p = PreparePKM();
                p.Version = (int)GameVersion.MN;
                PKM legal;
                if (allowAPI)
                {
                    AutoLegalityMod mod = new AutoLegalityMod();
                    mod.SAV = C_SAV.SAV;
                    bool satisfied = false;
                    PKM APIGenerated = C_SAV.SAV.BlankPKM;
                    try { APIGenerated = mod.APILegality(p, Set, out satisfied); }
                    catch { satisfied = false; }
                    if (!satisfied)
                    {
                        Blah b = new Blah();
                        b.C_SAV = C_SAV;
                        legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID_ALM, SID_ALM, OT_ALM, gender_ALM);
                        APILegalized = false;
#if DEBUG
                        WinFormsUtil.Alert("Set was not genned by the API");
#endif
                    }
                    else
                    {
                        legal = APIGenerated;
                        APILegalized = true;
                    }
                }
                else
                {
                    Blah b = new Blah();
                    b.C_SAV = C_SAV;
                    legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID_ALM, SID_ALM, OT_ALM, gender_ALM);
                    APILegalized = false;
                }
                LoadTrainerData(legal);
                if (int.TryParse(Country_ALM, out int n) && int.TryParse(SubRegion_ALM, out int m) && int.TryParse(ConsoleRegion_ALM, out int o))
                {
                    legal = PKME_Tabs.SetPKMRegions(n, m, o, legal);
                    Country_ALM = "";
                    SubRegion_ALM = "";
                    ConsoleRegion_ALM = "";
                }
                PKME_Tabs.PopulateFields(legal);
                if (legal.Format < 7) PKME_Tabs.LoadFieldsFromPKM2(legal, true, false);
                if (Country_ALM != "" && SubRegion_ALM != "" && ConsoleRegion_ALM != "")
                {
                    PKME_Tabs.SetRegions(Country_ALM, SubRegion_ALM, ConsoleRegion_ALM);
                }
            }
            #endregion
        }

        private void LoadTrainerData(PKM legal = null)
        {
            bool checkPerGame = (PKME_Tabs.checkMode() == "game");
            // If mode is not set as game: (auto or save)
            string[] tdataVals;
            if(!checkPerGame || legal == null) tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV);

            else tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV, legal.Version);
            TID_ALM = Convert.ToInt32(tdataVals[0]);
            SID_ALM = Convert.ToInt32(tdataVals[1]);
            OT_ALM = tdataVals[2];
            if (OT_ALM == "PKHeX") OT_ALM = "Archit(TCD)"; // Avoids secondary handler error
            gender_ALM = 0;
            if (tdataVals[3] == "F" || tdataVals[3] == "Female") gender_ALM = 1;
            Country_ALM = tdataVals[4];
            SubRegion_ALM = tdataVals[5];
            ConsoleRegion_ALM = tdataVals[6];
            if((checkPerGame && legal != null) || APILegalized)
                legal = PKME_Tabs.SetTrainerData(OT_ALM, TID_ALM, SID_ALM, legal, APILegalized);
        }

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
    }
}