using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {        
        private void ClickShowdownImportPKMModded(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsText())
            { WinFormsUtil.Alert("Clipboard does not contain text."); return; }

            if (!Directory.Exists(MGDatabasePath)) Directory.CreateDirectory(MGDatabasePath);

            int TID = -1;
            int SID = -1;
            string OT = "";
            int gender = 0;
            string Country = "";
            string SubRegion = "";
            string ConsoleRegion = "";
            if (File.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\trainerdata.txt"))
            {
                string text = File.ReadAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\trainerdata.txt", System.Text.Encoding.UTF8);
                string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                TID = Convert.ToInt32(lines[0].Split(':')[1].Trim());
                SID = Convert.ToInt32(lines[1].Split(':')[1].Trim());
                OT = lines[2].Split(':')[1].Trim();
                if (lines[3].Split(':')[1].Trim() == "F" || lines[3].Split(':')[1].Trim() == "Female") gender = 1;
                try
                {
                    Country = lines[4].Split(':')[1].Trim();
                    SubRegion = lines[5].Split(':')[1].Trim();
                    ConsoleRegion = lines[6].Split(':')[1].Trim();
                }
                catch { }
            }

            string source = Clipboard.GetText().TrimEnd();
            string[] stringSeparators = new string[] { "\n\r" };
            string[] result;

            // ...
            result = source.Split(stringSeparators, StringSplitOptions.None);
            Console.WriteLine(result.Length);

            if (result.Length > 1)
            {
                List<int> emptySlots = new List<int> { };
                for (int i = 0; i < C_SAV.Box.BoxSlotCount; i++)
                {
                    if ((C_SAV.Box.SlotPictureBoxes[i] as PictureBox)?.Image == null) emptySlots.Add(i);
                }
                if (emptySlots.Count < result.Length)
                {
                    WinFormsUtil.Alert("Not enough space in the box");
                    return;
                }
                for (int i = 0; i < result.Length; i++)
                {
                    ShowdownSet Set = new ShowdownSet(result[i]);
                    if (Set.InvalidLines.Any())
                        WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

                    // Set Species & Nickname
                    bool resetForm = false;
                    PKME_Tabs.hardReset();
                    if (Set.Form == null) { }
                    else if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")
                    {
                        resetForm = true;
                        Console.WriteLine(Set.Species);
                    }
                    PKME_Tabs.LoadShowdownSet(Set);
                    PKM p = PreparePKM();
                    p.Version = (int)GameVersion.MN;
                    Blah b = new Blah();
                    PKM legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID, SID, OT, gender);
                    PKME_Tabs.PopulateFields(legal);
                    if (Country != "" && SubRegion != "" && ConsoleRegion != "")
                    {
                        PKME_Tabs.SetRegions(Country, SubRegion, ConsoleRegion);
                    }
                    PKM pk = PreparePKM();
                    PKME_Tabs.ClickSet(C_SAV.Box.SlotPictureBoxes[0], emptySlots[i]);
                }
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
                PKME_Tabs.hardReset();
                if (Set.Form == null) { }
                else if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")
                {
                    resetForm = true;
                    Console.WriteLine(Set.Species);
                }
                PKME_Tabs.LoadShowdownSet(Set);
                PKM p = PreparePKM();
                p.Version = (int)GameVersion.MN;
                Blah b = new Blah();
                PKM legal = b.LoadShowdownSetModded_PKSM(p, Set, resetForm, TID, SID, OT, gender);
                PKME_Tabs.PopulateFields(legal);
                if (Country != "" && SubRegion != "" && ConsoleRegion != "")
                {
                    PKME_Tabs.SetRegions(Country, SubRegion, ConsoleRegion);
                }
            }
        }
    }
}