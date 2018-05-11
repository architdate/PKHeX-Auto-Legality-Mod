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
        private void ClickShowdownImportPKMModded(object sender, EventArgs e)
        {
            bool allowAPI = false; // Use true to allow experimental API usage
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

            bool checkPerGame = (PKME_Tabs.checkMode() == "game");
            int TID = -1;
            int SID = -1;
            string OT = "";
            int gender = 0;
            string Country = "";
            string SubRegion = "";
            string ConsoleRegion = "";
            if (!checkPerGame)
            {
                string[] tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV);

                TID = Convert.ToInt32(tdataVals[0]);
                SID = Convert.ToInt32(tdataVals[1]);
                OT = tdataVals[2];
                if (OT == "PKHeX") OT = "Archit(TCD)"; // Avoids secondary handler error
                gender = 0;
                if (tdataVals[3] == "F" || tdataVals[3] == "Female") gender = 1;
                Country = tdataVals[4];
                SubRegion = tdataVals[5];
                ConsoleRegion = tdataVals[6];
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

            if (result.Length > 1)
            {
                List<int> emptySlots = new List<int> { };
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
                string setsungenned = "";
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
                            setsungenned += Set.Text + "\n";
                            Blah b = new Blah();
                            b.C_SAV = C_SAV;
                            legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID, SID, OT, gender);
                        }
                        else
                        {
                            ctrapi++;
                            legal = APIGenerated;
                        }
                    }
                    else
                    {
                        Blah b = new Blah();
                        b.C_SAV = C_SAV;
                        legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID, SID, OT, gender);
                    }
                    if (checkPerGame)
                    {
                        string[] tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV, legal.Version);

                        TID = Convert.ToInt32(tdataVals[0]);
                        SID = Convert.ToInt32(tdataVals[1]);
                        OT = tdataVals[2];
                        if (OT == "PKHeX") OT = "Archit(TCD)"; // Avoids secondary handler error
                        gender = 0;
                        if (tdataVals[3] == "F" || tdataVals[3] == "Female") gender = 1;
                        Country = tdataVals[4];
                        SubRegion = tdataVals[5];
                        ConsoleRegion = tdataVals[6];
                        legal = PKME_Tabs.SetTrainerData(OT, TID, SID, legal);
                    }
                    if (int.TryParse(Country, out int n) && int.TryParse(SubRegion, out int m) && int.TryParse(ConsoleRegion, out int o))
                    {
                        legal = PKME_Tabs.SetPKMRegions(n, m, o, legal);
                        intRegions = true;
                    }
                    PKME_Tabs.PopulateFields(legal);
                    if (!intRegions)
                    {
                        PKME_Tabs.SetRegions(Country, SubRegion, ConsoleRegion);
                    }
                    PKM pk = PreparePKM();
                    PKME_Tabs.ClickSet(C_SAV.Box.SlotPictureBoxes[0], emptySlots[i]);
                }
                Console.WriteLine("API Genned Sets: " + ctrapi);
                Console.WriteLine(setsungenned);
            }
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
                Blah b = new Blah();
                b.C_SAV = C_SAV;
                PKM legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID, SID, OT, gender);
                if (checkPerGame)
                {
                    string[] tdataVals = PKME_Tabs.parseTrainerJSON(C_SAV, legal.Version);

                    TID = Convert.ToInt32(tdataVals[0]);
                    SID = Convert.ToInt32(tdataVals[1]);
                    OT = tdataVals[2];
                    if (OT == "PKHeX") OT = "Archit(TCD)"; // Avoids secondary handler error
                    gender = 0;
                    if (tdataVals[3] == "F" || tdataVals[3] == "Female") gender = 1;
                    Country = tdataVals[4];
                    SubRegion = tdataVals[5];
                    ConsoleRegion = tdataVals[6];
                    legal = PKME_Tabs.SetTrainerData(OT, TID, SID, legal);
                }
                if (int.TryParse(Country, out int n) && int.TryParse(SubRegion, out int m) && int.TryParse(ConsoleRegion, out int o))
                {
                    legal = PKME_Tabs.SetPKMRegions(n, m, o, legal);
                    Country = "";
                    SubRegion = "";
                    ConsoleRegion = "";
                }
                PKME_Tabs.PopulateFields(legal);
                if (legal.Format < 7) PKME_Tabs.LoadFieldsFromPKM2(legal, true, false);
                if (Country != "" && SubRegion != "" && ConsoleRegion != "")
                {
                    PKME_Tabs.SetRegions(Country, SubRegion, ConsoleRegion);
                }
            }
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