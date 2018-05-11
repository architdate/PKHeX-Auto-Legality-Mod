using System.Windows.Forms;
using System.Collections.Generic;

using PKHeX.Core;
using System;

namespace PKHeX.WinForms.Controls
{
    public partial class AutoLegalityMod : UserControl
    {
        public SaveFile SAV;

        /// <summary>
        /// Main function that auto legalizes based on the legality
        /// </summary>
        /// <param name="roughPK">rough pkm that has all the SSet values entered</param>
        /// <param name="SSet">Showdown set object</param>
        /// <param name="satisfied">If the final result is satisfactory, otherwise use current auto legality functionality</param>
        /// <returns></returns>
        public PKM APILegality(PKM roughPK, ShowdownSet SSet, out bool satisfied)
        {
            bool changedForm = false;
            if(SSet.Form != null) changedForm = FixFormes(SSet, out SSet);
            satisfied = false; // true when all features of the PKM are satisfied
            int Form = roughPK.AltForm;
            if (changedForm)
            {
                Form = SSet.FormIndex;
                roughPK.ApplySetDetails(SSet);
            }

            // List of candidate PKM files

            int[] moves = SSet.Moves;
            var f = EncounterMovesetGenerator.GeneratePKMs(roughPK, SAV, moves);
            foreach (PKM pkmn in f)
            {
                if (pkmn != null)
                {
                    PKM pk = PKMConverter.ConvertToType(pkmn, SAV.PKMType, out _); // All Possible PKM files
                    SetSpeciesLevel(pk, SSet, Form);
                    SetMovesEVsItems(pk, SSet);
                    SetTrainerDataAndMemories(pk);
                    SetNatureAbility(pk, SSet);
                    SetIVsPID(pk, SSet);
                    pk.SetSuggestedHyperTrainingData(pk.IVs); // Hypertrain
                    SetEncryptionConstant(pk);
                    SetShinyBoolean(pk, SSet.Shiny);
                    LegalityAnalysis la = new LegalityAnalysis(pk);
                    if (la.Valid) satisfied = true;
                    if (satisfied)
                        return pk;
                    else Console.WriteLine(la.Report());
                }
            }
            return roughPK;
        }

        public bool ValidateLegality(PKM pk, out string report)
        {
            LegalityAnalysis la = new LegalityAnalysis(pk);
            report = la.Report();
            return la.Valid;
        }

        public bool FixFormes(ShowdownSet SSet, out ShowdownSet changedSet)
        {
            changedSet = SSet;
            if (SSet.Form.Contains("Mega") || SSet.Form == "Primal" || SSet.Form == "Busted") {
                SSet = new ShowdownSet(SSet.Text.Replace("-" + SSet.Form, ""));
                changedSet = SSet;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Set Species and Level with nickname (Helps with PreEvos)
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="SSet">SSet to modify</param>
        public void SetSpeciesLevel(PKM pk, ShowdownSet SSet, int Form)
        {
            pk.Species = SSet.Species;
            pk.SetAltForm(Form);
            pk.IsNicknamed = (SSet.Nickname != null);
            pk.Nickname = SSet.Nickname != null ? SSet.Nickname : PKX.GetSpeciesNameGeneration(pk.Species, pk.Language, SAV.Generation);
            pk.CurrentLevel = SSet.Level;
            if (pk.CurrentLevel == 50) pk.CurrentLevel = 100; // VGC Override
        }

        /// <summary>
        /// Set Moves, EVs and Items for a specific PKM. These should not affect legality after being vetted by GeneratePKMs
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="SSet">Showdown Set to refer</param>
        public void SetMovesEVsItems(PKM pk, ShowdownSet SSet)
        {
            pk.SetMoves(SSet.Moves, true);
            pk.EVs = SSet.EVs;
            pk.CurrentFriendship = SSet.Friendship;
            pk.ApplyHeldItem(SSet.HeldItem, SSet.Format);
            var legal = new LegalityAnalysis(pk);
            if (legal.Parsed && CheckInvalidRelearn(legal.Info.Relearn) && !pk.WasEvent)
                pk.RelearnMoves = pk.GetSuggestedRelearnMoves(legal);
        }

        /// <summary>
        /// Check for invalid relearn moves
        /// </summary>
        /// <param name="RelearnInfo">CheckResult List of relearn moves</param>
        /// <returns></returns>
        public bool CheckInvalidRelearn(CheckResult[] RelearnInfo)
        {
            foreach(CheckResult r in RelearnInfo)
            {
                if (!r.Valid) return false;
            }
            return true;
        }

        /// <summary>
        /// Set Trainer data (TID, SID, OT) for a given PKM
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        public void SetTrainerDataAndMemories(PKM pk)
        {
            if (!pk.WasEvent)
            {
                // Hardcoded a generic one for now, trainerdata.json implementation here later
                pk.CurrentHandler = 1;
                pk.HT_Name = "ARCH";
                pk.TID = 34567;
                pk.SID = 0;
                pk.OT_Name = "TCD";
                pk = FixMemoriesPKM(pk);
            }
        }

        /// <summary>
        /// Memory fix if needed
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <returns></returns>
        private PKM FixMemoriesPKM(PKM pk)
        {
            if (SAV.PKMType == typeof(PK7))
            {
                ((PK7)pk).FixMemories();
                return pk;
            }
            else if (SAV.PKMType == typeof(PK6))
            {
                ((PK6)pk).FixMemories();
                return pk;
            }
            return pk;
        }

        /// <summary>
        /// Set Nature and Ability of the pokemon
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="SSet">Showdown Set to refer</param>
        public void SetNatureAbility(PKM pk, ShowdownSet SSet)
        {
            // Values that are must for showdown set to work, IVs should be adjusted to account for this
            pk.Nature = SSet.Nature;
            pk.SetAbility(SSet.Ability);
        }

        /// <summary>
        /// Sets shiny value to whatever boolean is specified
        /// </summary>
        /// <param name="pk">PKM to modify</param>
        /// <param name="isShiny">Shiny value that needs to be set</param>
        public void SetShinyBoolean(PKM pk, bool isShiny)
        {
            if (!isShiny)
            {
                pk.SetUnshiny();
            }
            if (isShiny)
            {
                if (pk.GenNumber > 5 || pk.VC) pk.SetShiny();
                else pk.SetShinySID();
            }
        }

        /// <summary>
        /// Set IV Values for the pokemon
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="SSet"></param>
        public void SetIVsPID(PKM pk, ShowdownSet SSet)
        {
            // Useful Values for computation
            int Species = pk.Species;
            int Nature = pk.Nature;
            int Gender = pk.Gender;
            int AbilityNumber = pk.AbilityNumber; // 1,2,4 (HA)
            int Ability = pk.Ability;

            if (pk.GenNumber > 4 || pk.VC)
            {
                pk.IVs = SSet.IVs;
                pk.PID = PKX.GetRandomPID(Species, Gender, pk.Version, Nature, pk.Format, (uint)(AbilityNumber * 0x10001));
            }
            else
            {
                // Fuck My Life
            }
        }

        /// <summary>
        /// Set Encryption Constant based on PKM GenNumber
        /// </summary>
        /// <param name="pk"></param>
        public void SetEncryptionConstant(PKM pk)
        {
            if (pk.GenNumber > 5 || pk.VC)
            {
                int wIndex = Array.IndexOf(Legal.WurmpleEvolutions, pk.Species);
                uint EC = wIndex < 0 ? Util.Rand32() : PKX.GetWurmpleEC(wIndex / 2);
                pk.EncryptionConstant = EC;
            }
            else pk.EncryptionConstant = pk.PID; // Generations 3 to 5
        }

    }
}