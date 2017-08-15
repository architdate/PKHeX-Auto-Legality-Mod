using System;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using PKHeX.Core;

namespace PKHeX.WinForms.Controls
{
    public partial class Blah : UserControl
    {

        public PKM LoadShowdownSetModded_PKSM(PKM Set)
        {
            List<List<string>> evoChart = generateEvoLists2();
            int abilitynum = Set.AbilityNumber < 6 ? Set.AbilityNumber >> 1 : 0;
            Console.WriteLine(abilitynum);
            bool shiny = Set.IsShiny;
            bool legendary = false;
            bool eventMon = false;
            int[] legendaryList = new int[] { 150, 151 };
            //string[] legendaryList = new string[] { "Articuno", "Zapdos", "Moltres", "Mewtwo", "Mew", "Raikou", "Suicuine",
            //                                        "Entei", "Lugia", "Celebi", "Regirock", "Regice", "Registeel", "Latias",
            //                                        "Latios", "Kyogre", "Groudon", "Rayquaza", "Jirachi", "Deoxys", "Uxie",
            //                                        "Mesprit", "Azelf", "Dialga", "Palkia", "Heatran", "Regigigas", "Giratina",
            //                                        "Cresellia", "Phione", "Manaphy", "Darkrai", "Shaymin", "Arceus", "Victini",
            //                                        "Cobalion", "Terrakion", "Virizion", "Thundurus", "Tornadus", "Landorus",
            //                                        "Reshiram", "Zekrom", "Kyurem", "Keldeo", "Meloetta", "Genesect", "Xerneas",
            //                                        "Yveltal", "Zygarde", "Diancie", "Hoopa", "Volcanion", "Tapu Koko",
            //                                        "Tapu Lele", "Tapu Bulu", "Tapu Fini", "Cosmog", "Cosmoem", "Solgaleo",
            //                                        "Lunala", "Nihilego", "Buzzwole", "Pheromosa", "Xurkitree", "Celesteela",
            //                                        "Kartana", "Guzzlord", "Necrozma", "Magearna"};

            int[] eventList = new int[] { 721 };
            //string[] eventList = new string[] { "Celebi", "Diancie", "Genesect", "Hoopa", "Jirachi", "Keldeo", "Manaphy",
            //                                    "Meloetta", "Volcanion", "Magearna", "Marshadow" };

            int[] GameVersionList = new int[] { (int)GameVersion.MN, (int)GameVersion.SN, (int)GameVersion.AS, (int)GameVersion.OR, (int)GameVersion.X,
                                                (int)GameVersion.Y, (int)GameVersion.B, (int)GameVersion.B2, (int)GameVersion.W, (int)GameVersion.W2,
                                                (int)GameVersion.D, (int)GameVersion.P, (int)GameVersion.Pt, (int)GameVersion.HG, (int)GameVersion.SS,
                                                (int)GameVersion.R, (int)GameVersion.S, (int)GameVersion.E, (int)GameVersion.FR, (int)GameVersion.LG,
                                                (int)GameVersion.CXD, (int)GameVersion.RD, (int)GameVersion.GN, (int)GameVersion.BU, (int)GameVersion.YW };
            // Checking for Legendary to save time in egg iterations
            foreach (int mon in legendaryList)
            {
                if (Set.Species == mon)
                {
                    legendary = true;
                }
            }

            foreach (int mon in eventList)
            {
                if (Set.Species == mon)
                {
                    eventMon = true;
                }
            }

            // Egg based pokemon
            if (!legendary && !eventMon)
            {
                for (int i = 0; i < GameVersionList.Length; i++)
                {
                    Set.Version = GameVersionList[i];
                    Set.Language = 2;
                    Set.OT_Name = "Archit (TCD)";
                    Set.TID = 24521;
                    Set.SID = 42312;
                    Set.EggMetDate = new DateTime(2000, 1, 1);
                    Set.Egg_Location = 60002;
                    if (Set.Version == (int)GameVersion.D || Set.Version == (int)GameVersion.P || Set.Version == (int)GameVersion.Pt) Set.Egg_Location = 2002;
                    Set.Met_Level = 1;
                    Set.ConsoleRegion = 2;
                    Set = clickMetLocationModPKSM(Set);
                    if (Set.GenNumber > 4) Set.Met_Level = 1;
                    try
                    {
                        Set.CurrentHandler = 1;
                        Set.HT_Name = "Archit";
                        Set = SetSuggestedRelearnMoves_PKSM(Set);
                        Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                        if (shiny) Set.SetShinyPID();
                        if (Set.PID == 0)
                        {
                            Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                            if (shiny) Set.SetShinyPID();
                        }
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (CommonErrorHandling2(Set))
                        {
                            if (shiny) Set.SetShinyPID();
                            return Set;
                        }
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (new LegalityAnalysis(Set).Valid)
                        {
                            return Set;
                        }
                    }
                    catch { continue; }
                }
            }
            if (!new LegalityAnalysis(Set).Valid && !eventMon)
            {
                for (int i = 0; i < GameVersionList.Length ; i++)
                {
                    Set.AbilityNumber = abilitynum;
                    Set.WasEgg = false;
                    Set.EggMetDate = null;
                    Set.Egg_Location = 0;
                    Set.Version = GameVersionList[i];
                    Set.Language = 2;
                    Set.ConsoleRegion = 2;
                    Set.OT_Name = "Archit (TCD)";
                    Set.TID = 24521;
                    Set.SID = 42312;
                    try
                    {
                        Set.RelearnMove1 = 0;
                        Set.RelearnMove2 = 0;
                        Set.RelearnMove3 = 0;
                        Set.RelearnMove4 = 0;
                        clickMetLocationModPKSM(Set);
                        Set.CurrentHandler = 1;
                        Set.HT_Name = "Archit";
                        Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                        if (shiny) Set.SetShinyPID();
                        if (Set.PID == 0)
                        {
                            Set.PID = PKX.GetRandomPID(Set.Species, Set.Gender, Set.Version, Set.Nature, Set.Format, (uint)(Set.AbilityNumber * 0x10001));
                            if (shiny) Set.SetShinyPID();
                        }
                        Set.RefreshAbility(abilitynum);
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (CommonErrorHandling2(Set))
                        {
                            if (shiny) Set.SetShinyPID();
                            return Set;
                        }
                        Set.RefreshAbility(abilitynum);
                        if (Set.GenNumber < 6) Set.EncryptionConstant = Set.PID;
                        if (new LegalityAnalysis(Set).Valid || Set.Valid)
                        {
                            return Set;
                        }
                    }
                    catch { continue; }
                    if (Set.B2W2) return Set;
                }
            }

            return Set;
        }

        private PKM SetSuggestedRelearnMoves_PKSM(PKM Set)
        {
            Set.RelearnMove1 = 0;
            Set.RelearnMove2 = 0;
            Set.RelearnMove3 = 0;
            Set.RelearnMove4 = 0;
            LegalityAnalysis Legality = new LegalityAnalysis(Set);
            if (Set.Format < 6)
                return Set;

            int[] m = Legality.GetSuggestedRelearn();
            if (m.All(z => z == 0))
                if (!Set.WasEgg && !Set.WasEvent && !Set.WasEventEgg && !Set.WasLink)
                {
                    var encounter = Legality.GetSuggestedMetInfo();
                    if (encounter != null)
                        m = encounter.Relearn;
                }

            if (Set.RelearnMoves.SequenceEqual(m))
                return Set;

            Set.RelearnMove1 = m[0];
            Set.RelearnMove2 = m[1];
            Set.RelearnMove3 = m[2];
            Set.RelearnMove4 = m[3];
            return Set;
        }

        private bool CommonErrorHandling2(PKM pk)
        {
            string hp = pk.IV_HP.ToString();
            string atk = pk.IV_ATK.ToString();
            string def = pk.IV_DEF.ToString();
            string spa = pk.IV_SPA.ToString();
            string spd = pk.IV_SPD.ToString();
            string spe = pk.IV_SPE.ToString();
            LegalityAnalysis la = new LegalityAnalysis(pk);
            var report = la.Report(false);
            var updatedReport = report;

            if (report.Contains("Ability mismatch for encounter"))
            {
                pk.RefreshAbility(pk.AbilityNumber);
            }
            if (report.Contains("Invalid Met Location, expected Transporter."))
            {
                pk.Met_Location = 30001;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Can't have ball for encounter type."))
            {
                if (pk.B2W2)
                {
                    pk.Ball = 25; //Dream Ball
                    LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                    updatedReport = recheckLA.Report(false);
                    report = updatedReport;
                }
                else
                {
                    pk.Ball = 0;
                    LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                    updatedReport = recheckLA.Report(false);
                    report = updatedReport;
                }
            }
            if (report.Contains("Non japanese Mew from Faraway Island. Unreleased event."))
            {
                pk.Language = 1;
                pk.FatefulEncounter = true;
                pk.Nickname = PKX.GetSpeciesNameGeneration(pk.Species, pk.Language, 3);
                pk.PID = PKX.GetRandomPID(pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Format, (uint)(pk.AbilityNumber * 0x10001)); 
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("PID should be equal to EC!"))
            {
                pk.EncryptionConstant = pk.PID;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("PID should be equal to EC [with top bit flipped]!"))
            {
                pk.PID = PKX.GetRandomPID(pk.Species, pk.Gender, pk.Version, pk.Nature, pk.Format, (uint)(pk.AbilityNumber * 0x10001));
                if (pk.IsShiny) pk.SetShinyPID();
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("PID-Gender mismatch."))
            {
                if(pk.Gender == 0)
                {
                    pk.Gender = 1;
                }
                else
                {
                    pk.Gender = 0;
                }
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Missing Ribbons: National"))
            {
                ReflectUtil.SetValue(pk, "RibbonNational", -1);
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Contains("Special ingame Fateful Encounter flag missing"))
            {
                pk.FatefulEncounter = true;
                LegalityAnalysis recheckLA = new LegalityAnalysis(pk);
                updatedReport = recheckLA.Report(false);
                report = updatedReport;
            }
            if (report.Equals("Invalid: Encounter Type PID mismatch."))
            {
                return true;
            }
            return false;
        }

        private PKM clickMetLocationModPKSM(PKM p)
        {
            LegalityAnalysis Legality = new LegalityAnalysis(p);

            var encounter = Legality.GetSuggestedMetInfo();
            if (encounter == null || (p.Format >= 3 && encounter.Location < 0))
            {
                return p;
            }

            int level = encounter.Level;
            int location = encounter.Location;
            int minlvl = Legal.GetLowestLevel(p, encounter.Species);
            if (minlvl == 0)
                minlvl = level;

            if (p.CurrentLevel >= minlvl && p.Met_Level == level && p.Met_Location == location)
                return p;
            if (minlvl < level)
                minlvl = level;
            p.Met_Location = location;
            p.Met_Level = level;
            return p;
        }

        private List<List<string>> generateEvoLists2()
        {
            int counter = 0;
            string line;
            List<List<string>> evoList = new List<List<string>>();
            List<string> blankList = new List<string>();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PKHeX.WinForms.Resources.text.evolutions.txt";
            System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
            System.IO.StreamReader file = new System.IO.StreamReader(stream);
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
                counter++;
            }
            file.Close();
            return evoList;
        }

    }
}
