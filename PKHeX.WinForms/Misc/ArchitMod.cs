using System;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Collections.Generic;

using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor : UserControl
    {
        private Controls.SAVEditor CurrentSAV;

        public void LoadShowdownSetModded(ShowdownSet Set)
        {
            List<List<string>> evoChart = generateEvoLists();
            OpenEvent(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\DONOTTOUCH\\reset.pk7");
            bool legendary = false;
            string[] legendaryList = new string[] { "Articuno", "Zapdos", "Moltres", "Mewtwo", "Mew", "Raikou", "Suicuine",
                                                    "Entei", "Lugia", "Celebi", "Regirock", "Regice", "Registeel", "Latias",
                                                    "Latios", "Kyogre", "Groudon", "Rayquaza", "Jirachi", "Deoxys", "Uxie",
                                                    "Mesprit", "Azelf", "Dialga", "Palkia", "Heatran", "Regigigas", "Giratina",
                                                    "Cresellia", "Phione", "Manaphy", "Darkrai", "Shaymin", "Arceus", "Victini",
                                                    "Cobalion", "Terrakion", "Virizion", "Thundurus", "Tornadus", "Landorus",
                                                    "Reshiram", "Zekrom", "Kyurem", "Keldeo", "Meloetta", "Genesect", "Xerneas",
                                                    "Yveltal", "Zygarde", "Diancie", "Hoopa", "Volcanion", "Tapu Koko",
                                                    "Tapu Lele", "Tapu Bulu", "Tapu Fini", "Cosmog", "Cosmoem", "Solgaleo",
                                                    "Lunala", "Nihilego", "Buzzwole", "Pheromosa", "Xurkitree", "Celesteela",
                                                    "Kartana", "Guzzlord", "Necrozma", "Magearna"};

            CB_Species.SelectedValue = Set.Species;

            // Checking for Legendary to save time in egg iterations
            foreach (string mon in legendaryList)
            {
                if (CB_Species.Text == mon)
                {
                    legendary = true;
                }
            }

            CHK_Nicknamed.Checked = Set.Nickname != null;
            if (Set.Nickname != null)
                TB_Nickname.Text = Set.Nickname;
            if (Set.Gender != null)
            {
                int Gender = PKX.GetGender(Set.Gender);
                Label_Gender.Text = gendersymbols[Gender];
                Label_Gender.ForeColor = GetGenderColor(Gender);
            }

            // Set Form
            string[] formStrings = PKX.GetFormList(Set.Species,
                Util.GetTypesList("en"),
                Util.GetFormsList("en"), gendersymbols, pkm.Format);
            int form = 0;
            for (int i = 0; i < formStrings.Length; i++)
                if (formStrings[i].Contains(Set.Form ?? ""))
                { form = i; break; }
            CB_Form.SelectedIndex = Math.Min(CB_Form.Items.Count - 1, form);

            // Error Handling for Mega and Busted forms
            if (CB_Form.Text.Contains("Mega") || CB_Form.Text == "Busted" || CB_Form.Text.Contains("Primal") || CB_Form.Text.Contains("Resolute"))
            {
                CB_Form.SelectedIndex = 0;
            }

            // Set Ability and Moves
            CB_Ability.SelectedIndex = Math.Max(0, Array.IndexOf(pkm.PersonalInfo.Abilities, Set.Ability));
            ComboBox[] m = { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            for (int i = 0; i < 4; i++) m[i].SelectedValue = Set.Moves[i];

            // Set Item and Nature
            CB_HeldItem.SelectedValue = Set.HeldItem < 0 ? 0 : Set.HeldItem;
            CB_Nature.SelectedValue = Set.Nature < 0 ? 0 : Set.Nature;

            // Set IVs
            TB_HPIV.Text = Set.IVs[0].ToString();
            TB_ATKIV.Text = Set.IVs[1].ToString();
            TB_DEFIV.Text = Set.IVs[2].ToString();
            TB_SPAIV.Text = Set.IVs[4].ToString();
            TB_SPDIV.Text = Set.IVs[5].ToString();
            TB_SPEIV.Text = Set.IVs[3].ToString();

            // Set EVs
            TB_HPEV.Text = Set.EVs[0].ToString();
            TB_ATKEV.Text = Set.EVs[1].ToString();
            TB_DEFEV.Text = Set.EVs[2].ToString();
            TB_SPAEV.Text = Set.EVs[4].ToString();
            TB_SPDEV.Text = Set.EVs[5].ToString();
            TB_SPEEV.Text = Set.EVs[3].ToString();

            // Set Level and Friendship
            TB_Level.Text = Set.Level.ToString();
            TB_Friendship.Text = Set.Friendship.ToString();

            // Reset IV/EVs
            UpdateRandomPID(null, null);
            UpdateRandomEC(null, null);
            ComboBox[] p = { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
            for (int i = 0; i < 4; i++)
                p[i].SelectedIndex = m[i].SelectedIndex != 0 ? 3 : 0; // max PP

            if (Set.Shiny) UpdateShiny(true);
            pkm = PreparePKM();
            UpdateLegality();

            // Egg based pokemon
            if (!legendary)
            {
                for (int i = 0; i < CB_GameOrigin.Items.Count - 1; i++)
                {
                    CB_GameOrigin.SelectedIndex = i;
                    TB_OT.Text = "Archit (TCD)";
                    TB_TID.Text = "24521";
                    TB_SID.Text = "42312";
                    CHK_AsEgg.Checked = true;
                    TB_MetLevel.Text = "1";
                    CB_MetLocation.SelectedIndex = 5;
                    clickMetLocationMod(null, null);
                    CB_3DSReg.SelectedIndex = 2;
                    GB_EggConditions.Visible = true;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    try
                    {
                        CB_RelearnMove1.SelectedIndex = 0;
                        CB_RelearnMove2.SelectedIndex = 0;
                        CB_RelearnMove3.SelectedIndex = 0;
                        CB_RelearnMove4.SelectedIndex = 0;
                        pkm = PreparePKM();
                        pkm.CurrentHandler = 1;
                        TB_OTt2.Text = "Archit";
                        if (Legality.Info.Relearn.Any(z => !z.Valid))
                            SetSuggestedRelearnMoves(silent: true);
                        CheckSumVerify();
                        UpdateLegality();
                        UpdateRandomPID(BTN_RerollPID, null);
                        if (Set.Shiny) UpdateShiny(true);
                        if (TB_PID.Text == "00000000")
                        {
                            UpdateRandomPID(BTN_RerollPID, null);
                            if (Set.Shiny) UpdateShiny(true);
                        }
                        if (CommonErrorHandling(pkm))
                        {
                            PKM pkmfinal = PreparePKM();
                            if (Set.Shiny && !pkmfinal.IsShiny) UpdateShiny(true);
                            WinFormsUtil.Alert("Ignore this legality");
                            return;
                        }
                        if (Legality.Valid)
                        {
                            break;
                        }
                    }
                    catch { continue; }
                }
            }

            // Legendary / Wild Pokemon
            if (!Legality.Valid)
            {
                for (int i = 0; i < CB_GameOrigin.Items.Count - 1; i++)
                {
                    CHK_AsEgg.Checked = false;
                    CB_EggLocation.SelectedIndex = CHK_AsEgg.Checked ? 1 : 0; // daycare : none
                    CB_GameOrigin.SelectedIndex = i;
                    CB_3DSReg.SelectedIndex = 2;
                    TB_OT.Text = "Archit (TCD)";
                    TB_TID.Text = "24521";
                    TB_SID.Text = "42312";
                    try
                    {
                        CB_RelearnMove1.SelectedIndex = 0;
                        CB_RelearnMove2.SelectedIndex = 0;
                        CB_RelearnMove3.SelectedIndex = 0;
                        CB_RelearnMove4.SelectedIndex = 0;
                        clickMetLocationMod(null, null);
                        pkm = PreparePKM();
                        pkm.CurrentHandler = 1;
                        TB_OTt2.Text = "Archit";
                        CheckSumVerify();
                        UpdateLegality();
                        UpdateRandomPID(BTN_RerollPID, null);
                        if (Set.Shiny) UpdateShiny(true);
                        if (TB_PID.Text == "00000000")
                        {
                            UpdateRandomPID(BTN_RerollPID, null);
                            if (Set.Shiny) UpdateShiny(true);
                        }
                        CheckSumVerify();
                        if (CommonErrorHandling(PreparePKM()))
                        {
                            PKM pkmfinal = PreparePKM();
                            if (Set.Shiny && !pkmfinal.IsShiny)
                            {
                                UpdateShiny(false);
                            }
                            WinFormsUtil.Alert("Ignore this legality");
                            return;
                        }
                        if (Legality.Valid)
                        {
                            break;
                        }
                    }
                    catch { continue; }
                }
            }

            // Event Pokemon
            if (!Legality.Valid)
            {
                string pkmnName = CB_Species.Text;
                string move1 = CB_Move1.Text;
                string move2 = CB_Move2.Text;
                string move3 = CB_Move3.Text;
                string move4 = CB_Move4.Text;
                OpenEvent(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\DONOTTOUCH\\reset.pk7");

                //bool fastSearch = true;
                if (legendary)
                {
                    List<string> fileList = new List<string>();
                    string fpath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\mgdb";
                    foreach (string file in System.IO.Directory.GetFiles(fpath, "*.*", System.IO.SearchOption.AllDirectories))
                    {
                        if (file.Contains(pkmnName))
                        {
                            fileList.Add(file);
                            Console.WriteLine(file);
                        }
                    }
                    foreach (string eventfile in fileList)
                    {
                        try
                        {
                            OpenEvent(eventfile);
                            ShowdownData(Set);
                            bool ignoreLegality = false;
                            if (clickLegality(ignoreLegality)) return;
                            UpdateLegality();
                            if (Legality.Valid) break;
                        }
                        catch { }
                    }
                    ShowdownData(Set);
                }
                else
                {
                    List<string> chain = new List<string>();
                    foreach(List<string> a in evoChart)
                    {
                        foreach(string b  in a)
                        {
                            if (b == pkmnName)
                            {
                                chain = a;
                            }
                        }
                    }
                    List<string> fileList = new List<string>();
                    string fpath = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\mgdb";
                    foreach (string file in System.IO.Directory.GetFiles(fpath, "*.*", System.IO.SearchOption.AllDirectories))
                    {
                        foreach (string mon in chain)
                        {
                            if (file.Contains(mon))
                            {
                                fileList.Add(file);
                                Console.WriteLine(file);
                            }
                        }
                    }
                    foreach (string eventfile in fileList)
                    {
                        try
                        {
                            OpenEvent(eventfile);
                            ShowdownData(Set);
                            if (Legality.Info.Relearn.Any(z => !z.Valid))
                                SetSuggestedRelearnMoves(silent: true);
                            CheckSumVerify();
                            UpdateLegality();
                            
                            if (Legality.Valid) break;
                        }
                        catch { }
                    }
                    ShowdownData(Set);
                }
            }

            if (!Legality.Valid)
            {
                WinFormsUtil.Alert("Set Not supported for autolegality yet, please DM thecommondude with the set details");
            }

        }

        private void ShowdownData(ShowdownSet Set)
        {
            CB_Species.SelectedValue = Set.Species;
            if (Set.Nickname != null)
                TB_Nickname.Text = Set.Nickname;
            if (Set.Gender != null)
            {
                int Gender = PKX.GetGender(Set.Gender);
                Label_Gender.Text = gendersymbols[Gender];
                Label_Gender.ForeColor = GetGenderColor(Gender);
            }

            // Set Form
            string[] formStrings = PKX.GetFormList(Set.Species,
                Util.GetTypesList("en"),
                Util.GetFormsList("en"), gendersymbols, pkm.Format);
            int form = 0;
            for (int i = 0; i < formStrings.Length; i++)
                if (formStrings[i].Contains(Set.Form ?? ""))
                { form = i; break; }
            CB_Form.SelectedIndex = Math.Min(CB_Form.Items.Count - 1, form);

            // Set Ability and Moves
            CB_Ability.SelectedIndex = Math.Max(0, Array.IndexOf(pkm.PersonalInfo.Abilities, Set.Ability));
            ComboBox[] m = { CB_Move1, CB_Move2, CB_Move3, CB_Move4 };
            for (int i = 0; i < 4; i++) m[i].SelectedValue = Set.Moves[i];

            // Set Item and Nature
            CB_HeldItem.SelectedValue = Set.HeldItem < 0 ? 0 : Set.HeldItem;
            CB_Nature.SelectedValue = Set.Nature < 0 ? 0 : Set.Nature;

            // Set IVs
            TB_HPIV.Text = Set.IVs[0].ToString();
            TB_ATKIV.Text = Set.IVs[1].ToString();
            TB_DEFIV.Text = Set.IVs[2].ToString();
            TB_SPAIV.Text = Set.IVs[4].ToString();
            TB_SPDIV.Text = Set.IVs[5].ToString();
            TB_SPEIV.Text = Set.IVs[3].ToString();

            // Set EVs
            TB_HPEV.Text = Set.EVs[0].ToString();
            TB_ATKEV.Text = Set.EVs[1].ToString();
            TB_DEFEV.Text = Set.EVs[2].ToString();
            TB_SPAEV.Text = Set.EVs[4].ToString();
            TB_SPDEV.Text = Set.EVs[5].ToString();
            TB_SPEEV.Text = Set.EVs[3].ToString();

            // Set Level and Friendship
            TB_Level.Text = Set.Level.ToString();
            TB_Friendship.Text = Set.Friendship.ToString();

            // Reset IV/EVs
            UpdateRandomPID(null, null);
            UpdateRandomEC(null, null);
            ComboBox[] p = { CB_PPu1, CB_PPu2, CB_PPu3, CB_PPu4 };
            for (int i = 0; i < 4; i++)
                p[i].SelectedIndex = m[i].SelectedIndex != 0 ? 3 : 0; // max PP

            if (Set.Shiny) UpdateShiny(true);
        }

        private bool CommonErrorHandling(PKM pk)
        {
            string hp = TB_HPIV.Text;
            string atk = TB_ATKIV.Text;
            string def = TB_DEFIV.Text;
            string spa = TB_SPAIV.Text;
            string spd = TB_SPDIV.Text;
            string spe = TB_SPEIV.Text;
            LegalityAnalysis la = new LegalityAnalysis(pk);
            if (pk.Slot < 0)
                UpdateLegality(la);
            var report = la.Report(false);
            PKM pknew;
            var updatedReport = report;

            if (report.Contains("Ability mismatch for encounter"))
            {
                bool legalized = false;
                int abilityIndex = CB_Ability.SelectedIndex;
                int legalizedIndex = abilityIndex;

                for (int i = 0; i < CB_Ability.Items.Count; i++)
                {
                    CB_Ability.SelectedIndex = i;
                    pknew = PreparePKM();
                    LegalityAnalysis updatedLA = new LegalityAnalysis(pknew);
                    updatedReport = updatedLA.Report(false);
                    if (!updatedReport.Contains("Ability mismatch for encounter"))
                    {
                        UpdateLegality();
                        report = updatedReport;
                        //legalized = true;
                        legalizedIndex = i;
                        break;
                    }
                }
                CB_Ability.SelectedIndex = abilityIndex;
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                if (!updatedReport.Contains("Ability mismatch for encounter"))
                {
                    UpdateLegality();
                    report = updatedReport;
                }
                else if (legalized)
                {
                    CB_Ability.SelectedIndex = legalizedIndex;
                    UpdateLegality();
                }
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("Invalid Met Location, expected Transporter."))
            {
                CB_MetLocation.SelectedIndex = 0;
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("Can't have ball for encounter type."))
            {
                CB_Ball.SelectedIndex = 6;
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("Non japanese Mew from Faraway Island. Unreleased event."))
            {
                CB_Language.SelectedIndex = 0;
                CHK_Fateful.Checked = true;
                UpdateRandomPID(BTN_RerollPID, null);
                UpdateLegality();
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("PID should be equal to EC [with top bit flipped]!"))
            {
                UpdateRandomPID(BTN_RerollPID, null);
                if (pk.IsShiny) UpdateShiny(false);
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("PID-Gender mismatch."))
            {
                ClickGender(null, null);
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("Missing Ribbons: National"))
            {
                pknew = PreparePKM();
                ReflectUtil.SetValue(pknew, "RibbonNational", -1);
                PopulateFields(pknew);
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("Special ingame Fateful Encounter flag missing"))
            {
                CHK_Fateful.Checked = true;
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                CheckSumVerify();
                UpdateLegality();
            }
            if (report.Contains("Invalid: Encounter Type PID mismatch."))
            {
                if(CB_GameOrigin.Text == "Colosseum/XD")
                { setPIDSID(pk.IsShiny, true); }
                else setPIDSID(pk.IsShiny);
                if (Legality.Valid)
                {
                    return false;
                }
                CheckSumVerify();
                UpdateLegality();
                pknew = PreparePKM();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
                if (report.Equals("Invalid: Encounter Type PID mismatch."))
                {
                    return true;
                }
                else if(report.Contains("PID-Gender mismatch."))
                {
                    ClickGender(null, null);
                    pknew = PreparePKM();
                    recheckLA = new LegalityAnalysis(pknew);
                    updatedReport = recheckLA.Report(false);
                    report = updatedReport;
                    CheckSumVerify();
                    UpdateLegality();
                    if (Legality.Valid)
                    {
                        return false;
                    }
                }
            }
            pknew = PreparePKM();
            pknew.HT_HP = false;
            pknew.HT_ATK = false;
            pknew.HT_DEF = false;
            pknew.HT_SPA = false;
            pknew.HT_SPD = false;
            pknew.HT_SPE = false;
            PopulateFields(pk);
            TB_HPIV.Text = hp;
            TB_ATKIV.Text = atk;
            TB_DEFIV.Text = def;
            TB_SPAIV.Text = spa;
            TB_SPDIV.Text = spd;
            TB_SPEIV.Text = spe;
            CheckSumVerify();
            UpdateLegality();
            return false;
        }

        private void CheckSumVerify()
        {
            if (!VerifiedPKM())
            { SystemSounds.Asterisk.Play(); return; }

            var pk = PreparePKM();

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Asterisk.Play(); return; }
        }

        private void clickMetLocationMod(object sender, EventArgs e)
        {
            if (HaX)
                return;

            pkm = PreparePKM();
            UpdateLegality(skipMoveRepop: true);
            if (Legality.Valid)
                return;

            var encounter = Legality.GetSuggestedMetInfo();
            if (encounter == null || (pkm.Format >= 3 && encounter.Location < 0))
            {
                //WinFormsUtil.Alert("Unable to provide a suggestion.");
                return;
            }

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(pkm, encounter.Species);
            if (minlvl == 0)
                minlvl = level;

            if (pkm.CurrentLevel >= minlvl && pkm.Met_Level == level && pkm.Met_Location == location)
                return;
            if (minlvl < level)
                minlvl = level;

            var suggestion = new List<string> { "Suggested:" };
            if (pkm.Format >= 3)
            {
                var met_list = GameInfo.GetLocationList((GameVersion)pkm.Version, pkm.Format, egg: false);
                var locstr = met_list.FirstOrDefault(loc => loc.Value == location).Text;
                suggestion.Add($"Met Location: {locstr}");
                suggestion.Add($"Met Level: {level}");
            }
            if (pkm.CurrentLevel < minlvl)
                suggestion.Add($"Current Level: {minlvl}");

            if (suggestion.Count == 1) // no suggestion
                return;

            string suggest = string.Join(Environment.NewLine, suggestion);
            //if (WinFormsUtil.Prompt(MessageBoxButtons.YesNo, suggest) != DialogResult.Yes)
            //return;

            if (pkm.Format >= 3)
            {
                TB_MetLevel.Text = level.ToString();
                CB_MetLocation.SelectedValue = location;
            }

            if (pkm.CurrentLevel < minlvl)
                TB_Level.Text = minlvl.ToString();

            pkm = PreparePKM();
            UpdateLegality();
        }

        private void OpenFile(byte[] input, string path, string ext, SaveFile currentSaveFile)
        {

            if (TryLoadPKM(input, path, ext, currentSaveFile))
                return;
            if (TryLoadMysteryGift(input, path, ext, currentSaveFile))
                return;

            //WinFormsUtil.Error("Attempted to load an unsupported file type/size.",
            //  $"File Loaded:{Environment.NewLine}{path}",
            //$"File Size:{Environment.NewLine}{input.Length} bytes (0x{input.Length:X4})");
        }

        private bool TryLoadPKM(byte[] input, string path, string ext, SaveFile SAV)
        {
            var temp = PKMConverter.GetPKMfromBytes(input, prefer: ext.Length > 0 ? (ext.Last() - 0x30) & 7 : SAV.Generation);
            if (temp == null)
                return false;

            var type = CurrentPKM.GetType();
            PKM pk = PKMConverter.ConvertToType(temp, type, out string c);
            if (pk == null)
            {
                //WinFormsUtil.Alert("Conversion failed.", c);
                return false;
            }
            if (SAV.Generation < 3 && ((pk as PK1)?.Japanese ?? ((PK2)pk).Japanese) != SAV.Japanese)
            {
                var strs = new[] { "International", "Japanese" };
                var val = SAV.Japanese ? 0 : 1;
                WinFormsUtil.Alert($"Cannot load {strs[val]} {pk.GetType().Name}s to {strs[val ^ 1]} saves.");
                return false;
            }

            PopulateFields(pk);
            Console.WriteLine(c);
            return true;
        }

        private bool TryLoadMysteryGift(byte[] input, string path, string ext, SaveFile SAV)
        {
            var tg = MysteryGift.GetMysteryGift(input, ext);
            if (tg == null)
                return false;
            if (!tg.IsPokémon)
            {
                //WinFormsUtil.Alert("Mystery Gift is not a Pokémon.", path);
                return true;
            }

            var temp = tg.ConvertToPKM(SAV);
            PKM pk = PKMConverter.ConvertToType(temp, SAV.PKMType, out string c);

            if (pk == null)
            {
                //WinFormsUtil.Alert("Conversion failed.", c);
                return true;
            }

            PopulateFields(pk);
            Console.WriteLine(c);
            return true;
        }

        private bool clickLegality(bool ignoreLegality)
        {
            if (!VerifiedPKM())
            { SystemSounds.Asterisk.Play(); return false; }

            var pk = PreparePKM();

            if (pk.Species == 0 || !pk.ChecksumValid)
            { SystemSounds.Asterisk.Play(); return false; }

            return ShowLegality(pk, ignoreLegality);
        }

        private bool ShowLegality(PKM pk, bool ignoreLegality)
        {
            LegalityAnalysis la = new LegalityAnalysis(pk);
            if (pk.Slot < 0)
                UpdateLegality(la);
            bool verbose = ModifierKeys == Keys.Control;
            var report = la.Report(verbose);
            if (verbose)
            {
                var dr = WinFormsUtil.Prompt(MessageBoxButtons.YesNo, report, "Copy report to Clipboard?");
                if (dr == DialogResult.Yes)
                    Clipboard.SetText(report);
                return false;
            }
            else if (report.Equals("Invalid: Encounter Type PID mismatch."))
            {
                WinFormsUtil.Alert("Ignore this legality check");
                return true;
            }
            else

                //WinFormsUtil.Alert(report);
                return false;
        }

        private void OpenEvent(string path, bool force = false)
        {
            this.CurrentSAV = new PKHeX.WinForms.Controls.SAVEditor();
            this.CurrentSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentSAV.Location = new System.Drawing.Point(292, 26);
            this.CurrentSAV.Name = "C_SAV";
            this.CurrentSAV.Size = new System.Drawing.Size(310, 326);
            this.CurrentSAV.TabIndex = 104;

            if (!(CanFocus || force))
            {
                SystemSounds.Asterisk.Play();
                return;
            }

            string ext = System.IO.Path.GetExtension(path);
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            if (fi.Length > 0x10009C && fi.Length != 0x380000 && !SAV3GCMemoryCard.IsMemoryCardSize(fi.Length))
                WinFormsUtil.Error("Input file is too large." + Environment.NewLine + $"Size: {fi.Length} bytes", path);
            else if (fi.Length < 32)
                WinFormsUtil.Error("Input file is too small." + Environment.NewLine + $"Size: {fi.Length} bytes", path);
            else
            {
                byte[] input; try { input = System.IO.File.ReadAllBytes(path); }
                catch (Exception e) { WinFormsUtil.Error("Unable to load file.  It could be in use by another program.\nPath: " + path, e); return; }

#if DEBUG
                OpenFile(input, path, ext, CurrentSAV.SAV);
#else
                try { openFile(input, path, ext); }
                catch (Exception e) { WinFormsUtil.Error("Unable to load file.\nPath: " + path, e); }
#endif
            }
        }

        private List<List<string>> generateEvoLists()
        {
            int counter = 0;
            string line;
            List<List<string>> evoList = new List<List<string>>();

            List<string> blankList = new List<string>();
            // Read the file and display it line by line.
            System.IO.StreamReader file =
               new System.IO.StreamReader(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\DONOTTOUCH\\evolutions.txt");
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim() == "")
                {
                    evoList.Add(blankList);
                    blankList = new List<string>();
                }
                else
                {
                    blankList.Add(line.Trim());
                }
                //Console.WriteLine(line);
                counter++;
            }

            file.Close();
            return evoList;
        }

        private void setPIDSID(bool shiny, bool XD = false)
        {
            uint hp = uint.Parse(TB_HPIV.Text);
            uint atk = uint.Parse(TB_ATKIV.Text);
            uint def = uint.Parse(TB_DEFIV.Text);
            uint spa = uint.Parse(TB_SPAIV.Text);
            uint spd = uint.Parse(TB_SPDIV.Text);
            uint spe = uint.Parse(TB_SPEIV.Text);
            string natText = CB_Nature.Text;
            uint nature = 0;
            if (natText == "Adamant") { nature = 3; }
            else if (natText == "Bold") { nature = 5; }
            else if (natText == "Brave") { nature = 2; }
            else if (natText == "Calm") { nature = 20; }
            else if (natText == "Careful") { nature = 23; }
            else if (natText == "Hasty") { nature = 11; }
            else if (natText == "Impish") { nature = 8; }
            else if (natText == "Jolly") { nature = 13; }
            else if (natText == "Lonely") { nature = 1; }
            else if (natText == "Mild") { nature = 16; }
            else if (natText == "Modest") { nature = 15; }
            else if (natText == "Naive") { nature = 14; }
            else if (natText == "Naughty") { nature = 4; }
            else if (natText == "Quiet") { nature = 17; }
            else if (natText == "Rash") { nature = 19; }
            else if (natText == "Relaxed") { nature = 7; }
            else if (natText == "Sassy") { nature = 22; }
            else if (natText == "Timid") { nature = 10; }
            else if (natText == "Gentle") { nature = 21; }
            else if (natText == "Lax") { nature = 9; }
            else if (natText == "Bashful") { nature = 18; }
            else if (natText == "Docile") { nature = 6; }
            else if (natText == "Hardy") { nature = 0; }
            else if (natText == "Quirky") { nature = 24; }
            else { nature = 12; }
            uint tid = uint.Parse(TB_TID.Text);
            string[] pidsid = { "", "" };
            if (XD) {
                pidsid = Misc.IVtoPIDGenerator.XDPID(hp, atk, def, spa, spd, spe, nature, 0);
            }
            else { pidsid = Misc.IVtoPIDGenerator.M1PID(hp, atk, def, spa, spd, spe, nature, 0); }
            TB_PID.Text = pidsid[0];
            TB_SID.Text = pidsid[1];
            if (shiny) UpdateShiny(false);
            PKM pk = PreparePKM();
            LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
            string updatedReport = recheckLA.Report(false);
            CheckSumVerify();
            UpdateLegality();
            if (updatedReport.Contains("Invalid: Encounter Type PID mismatch."))
            {
                List<List<string>> ivspreads = new List<List<string>>();
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "7" }); // Hardy
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "1" }); // Lonely
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "3" }); // Brave
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "2" }); // Adamant
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "7" }); // Naughty
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "19" }); // Bold
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "0" }); // Docile
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "0" }); // Relaxed
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "3" }); // Impish
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "8" }); // Lax
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "10" }); // Timid
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "1" }); // Hasty
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "1" }); // Serious
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "16" }); // Jolly
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "8" }); // Naive
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "0" }); // Modest
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "0" }); // Mild
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "22" }); // Quiet
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "5" }); // Bashful
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "5" }); // Rash
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "1" }); // Calm
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "1" }); // Gentle
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "1", "14" }); // Sassy
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "2" }); // Careful
                ivspreads.Add(new List<string> { "0", "0", "0", "0", "0", "6" }); // Quirky
                TB_HPIV.Text = ivspreads[Convert.ToInt32(nature)][0];
                TB_ATKIV.Text = ivspreads[Convert.ToInt32(nature)][1];
                TB_DEFIV.Text = ivspreads[Convert.ToInt32(nature)][2];
                TB_SPAIV.Text = ivspreads[Convert.ToInt32(nature)][3];
                TB_SPDIV.Text = ivspreads[Convert.ToInt32(nature)][4];
                TB_SPEIV.Text = ivspreads[Convert.ToInt32(nature)][5];
                pidsid = Misc.IVtoPIDGenerator.M1PID(uint.Parse(TB_HPIV.Text), uint.Parse(TB_ATKIV.Text), uint.Parse(TB_DEFIV.Text), uint.Parse(TB_SPAIV.Text), uint.Parse(TB_SPDIV.Text), uint.Parse(TB_SPEIV.Text), nature, 0);
                TB_PID.Text = pidsid[0];
                TB_SID.Text = pidsid[1];
                if (pidsid[0] == "0" && pidsid[1] == "0")
                {
                    UpdateRandomPID(BTN_RerollPID, null);
                    TB_HPIV.Text = hp.ToString();
                    TB_ATKIV.Text = atk.ToString();
                    TB_DEFIV.Text = def.ToString();
                    TB_SPAIV.Text = spa.ToString();
                    TB_SPDIV.Text = spd.ToString();
                    TB_SPEIV.Text = spe.ToString();
                }
                PKM pknew = PreparePKM();
                pknew.HT_HP = true;
                pknew.HT_ATK = true;
                pknew.HT_DEF = true;
                pknew.HT_SPA = true;
                pknew.HT_SPD = true;
                pknew.HT_SPE = true;
                PopulateFields(pknew);
                if (shiny) UpdateShiny(false);
                pknew = PreparePKM();
                recheckLA = new LegalityAnalysis(pknew);
                updatedReport = recheckLA.Report(false);
                CheckSumVerify();
                UpdateLegality();
                if (updatedReport.Contains("Invalid: Encounter Type PID mismatch."))
                {
                    pknew = PreparePKM();
                    pknew.HT_HP = true;
                    pknew.HT_ATK = true;
                    pknew.HT_DEF = true;
                    pknew.HT_SPA = true;
                    pknew.HT_SPD = true;
                    pknew.HT_SPE = true;
                    PopulateFields(pknew);
                    TB_HPIV.Text = hp.ToString();
                    TB_ATKIV.Text = atk.ToString();
                    TB_DEFIV.Text = def.ToString();
                    TB_SPAIV.Text = spa.ToString();
                    TB_SPDIV.Text = spd.ToString();
                    TB_SPEIV.Text = spe.ToString();
                }
            }
        }

    }

}
