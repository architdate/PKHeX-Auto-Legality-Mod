using System.Windows.Forms;
using System.Collections.Generic;

using PKHeX.Core;

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
            satisfied = false;
            int[] moves = SSet.Moves;
            List<PKM> possiblePKMs = new List<PKM>();
            PKM retval = SAV.BlankPKM;
            var f = EncounterMovesetGenerator.GeneratePKMs(roughPK, SAV, moves);
            foreach(PKM pk in f)
            {
                possiblePKMs.Add(pk); // All possible PKMs
            }
            foreach (PKM pk in possiblePKMs)
            {
                if (pk != null)
                    retval = PKMConverter.ConvertToType(pk, SAV.PKMType, out _);
                else continue;
                retval.TID = 34567;
                retval.SID = 0;
                retval.OT_Name = "TCD";
                retval.HeldItem = SSet.HeldItem;
                retval.Ability = SSet.Ability;
                retval.EVs = SSet.EVs;
                retval.IVs = SSet.IVs;
                retval.Moves = SSet.Moves;
                if (!SSet.Shiny)
                {
                    retval.SetUnshiny();
                    LegalityAnalysis la = new LegalityAnalysis(retval);
                    if (la.Valid) satisfied = true;
                    if (satisfied) return retval;
                }
                else
                {
                    PKM tempPK = retval;
                    tempPK.SetShinySID();
                    LegalityAnalysis la = new LegalityAnalysis(tempPK);
                    if (la.Valid)
                    {
                        satisfied = true;
                        return tempPK;
                    }
                    retval.SetShiny();
                    LegalityAnalysis la2 = new LegalityAnalysis(retval);
                    if (la2.Valid)
                    {
                        satisfied = true;
                        return retval;
                    }
                }
            }
            return SAV.BlankPKM;
        }
    }
}