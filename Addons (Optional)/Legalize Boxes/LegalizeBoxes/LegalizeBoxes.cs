using System;
using System.Collections.Generic;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        private void LegalizeBoxes(object sender, EventArgs e)
        {
            IList<PKM> BoxData = C_SAV.SAV.BoxData;
            for (int i=0; i<30; i++)
            {
                PKM illegalPK = PreparePKM();
                bool box = false;
                if ((ModifierKeys & Keys.Control) == Keys.Control) {
                    box = true;
                }
                if (box) illegalPK = BoxData[C_SAV.CurrentBox * C_SAV.SAV.BoxSlotCount + i];
                if (illegalPK.Species > 0 && !new LegalityAnalysis(illegalPK).Valid)
                {
                    ShowdownSet Set = new ShowdownSet(ShowdownSet.GetShowdownText(illegalPK));
                    bool resetForm = false;
                    if (Set.Form != null)
                    {
                        if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted") resetForm = true;
                    }
                    AutoLegalityMod mod = new AutoLegalityMod();
                    mod.SAV = C_SAV.SAV;
                    PKM legal;
                    PKM APIGenerated = C_SAV.SAV.BlankPKM;
                    bool satisfied = false;
                    try { APIGenerated = mod.APILegality(illegalPK, Set, out satisfied); }
                    catch { satisfied = false; }
                    if (!satisfied)
                    {
                        Blah b = new Blah();
                        b.C_SAV = C_SAV;
                        legal = b.LoadShowdownSetModded_PKSM(illegalPK, Set, resetForm, illegalPK.TID, illegalPK.SID, illegalPK.OT_Name, illegalPK.OT_Gender);
                    }
                    else legal = APIGenerated;
                    legal = PKME_Tabs.SetTrainerData(illegalPK.OT_Name, illegalPK.TID, illegalPK.SID, illegalPK.OT_Gender, legal, satisfied);
                    if (box) BoxData[C_SAV.CurrentBox * C_SAV.SAV.BoxSlotCount + i] = legal;
                    else
                    {
                        PKME_Tabs.PopulateFields(legal);
                        WinFormsUtil.Alert("Legalized Active Pokemon.");
                        return;
                    }
                }
            }
            C_SAV.SAV.BoxData = BoxData;
            C_SAV.ReloadSlots();
            WinFormsUtil.Alert("Legalized Box Pokemon");
        }
    }
}