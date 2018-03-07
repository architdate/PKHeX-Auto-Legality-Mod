using System;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using PKHeX.Core;
using PKHeX.WinForms.Properties;
using PKHeX.WinForms.AutoLegality;

namespace PKHeX.WinForms.Controls
{
    public partial class PKMEditor : UserControl
    {
        // Initialize Current Save file
        private Controls.SAVEditor CurrentSAV;

        // Command to set in box (for multiple imports)
        public void ClickSet(object sender, int slot)
        {
            SlotChangeManager m = GetSenderInfo(ref sender, out SlotChange info, slot);
            if (m == null)
                return;

            var editor = m.SE.PKME_Tabs;
            var sav = m.SE.SAV;
            if (info.IsParty && editor.IsEmptyOrEgg && sav.IsPartyAllEggs(info.Slot - 30) && !m.SE.HaX)
            { WinFormsUtil.Alert("Can't have empty/egg party."); return; }
            if (m.SE.SAV.IsSlotLocked(info.Box, info.Slot))
            { WinFormsUtil.Alert("Can't set to locked slot."); return; }

            PKM pk = editor.PreparePKM();

            string[] errata = sav.IsPKMCompatible(pk);
            if (errata.Length > 0 && DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, string.Join(Environment.NewLine, errata), "Continue?"))
                return;

            if (info.Slot >= 30)
                info.Box = -1;
            if (info.Slot >= 30 && info.Slot < 36) // Party
            {
                // If info.Slot isn't overwriting existing PKM, make it write to the lowest empty PKM info.Slot
                if (sav.PartyCount < info.Slot + 1 - 30)
                {
                    info.Slot = sav.PartyCount + 30;
                    info.Offset = m.SE.GetPKMOffset(info.Slot);
                }
                m.SetPKM(pk, info, true, Resources.slotSet);
            }
            else if (info.Slot < 30 || m.SE.HaX)
            {
                if (info.Slot < 30)
                {
                    m.SE.UndoStack.Push(new SlotChange
                    {
                        Box = info.Box,
                        Slot = info.Slot,
                        Offset = info.Offset,
                        PKM = sav.GetStoredSlot(info.Offset)
                    });
                    m.SE.Menu_Undo.Enabled = true;
                }

                m.SetPKM(pk, info, true, Resources.slotSet);
            }

            editor.LastData = pk.Data;
            m.SE.RedoStack.Clear(); m.SE.Menu_Redo.Enabled = false;
        }

        public void hardReset(SaveFile SAV = null)
        {
            SaveFile CURRSAV = new SAVEditor().SAV;
            if (SAV != null) CURRSAV = SAV;
            else
            {
                CurrentSAV = new PKHeX.WinForms.Controls.SAVEditor();
                CurrentSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Right)));
                CurrentSAV.Location = new System.Drawing.Point(292, 26);
                CurrentSAV.Name = "C_SAV";
                CurrentSAV.Size = new System.Drawing.Size(310, 326);
                CurrentSAV.TabIndex = 104;
                CURRSAV = CurrentSAV.SAV;
            }
            if (CURRSAV.USUM || CURRSAV.SM)
            {
                if (TryLoadPKM(new ConstData().resetpk7, "", "pk7", CURRSAV))
                {
                    return;
                }
            }
            else if (CURRSAV.ORAS)
            {
                if (TryLoadPKM(new ConstData().resetpk6, "", "pk6", CURRSAV))
                {
                    return;
                }
            }
        }

        public void PrintByteArray(byte[] bytes)
        {
            var sb = new System.Text.StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            Console.WriteLine(sb.ToString());
        }

        public void LoadFieldsFromPKM2(PKM pk, bool focus = true, bool skipConversionCheck = true)
        {
            if (pk == null) { WinFormsUtil.Error("Attempted to load a null file."); return; }
            if (focus)
                Tab_Main.Focus();

            if (!skipConversionCheck && !PKMConverter.TryMakePKMCompatible(pk, CurrentPKM, out string c, out pk))
            { WinFormsUtil.Alert(c); return; }

            bool oldInit = fieldsInitialized;
            fieldsInitialized = fieldsLoaded = false;

            pkm = pk.Clone();

            try { GetFieldsfromPKM(); }
            finally { fieldsInitialized = oldInit; }

            UpdateIVs(null, null);
            UpdatePKRSInfected(null, null);
            UpdatePKRSCured(null, null);

            if (HaX) // Load original values from pk not pkm
            {
                MT_Level.Text = (pk.Stat_HPMax != 0 ? pk.Stat_Level : PKX.GetLevel(pk.Species, pk.EXP)).ToString();
                TB_EXP.Text = pk.EXP.ToString();
                MT_Form.Text = pk.AltForm.ToString();
                if (pk.Stat_HPMax != 0) // stats present
                {
                    Stat_HP.Text = pk.Stat_HPCurrent.ToString();
                    Stat_ATK.Text = pk.Stat_ATK.ToString();
                    Stat_DEF.Text = pk.Stat_DEF.ToString();
                    Stat_SPA.Text = pk.Stat_SPA.ToString();
                    Stat_SPD.Text = pk.Stat_SPD.ToString();
                    Stat_SPE.Text = pk.Stat_SPE.ToString();
                }
            }
            fieldsLoaded = true;

            SetMarkings();
            UpdateLegality();
            UpdateSprite();
            LastData = PreparePKM()?.Data;
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

        private static SlotChangeManager GetSenderInfo(ref object sender, out SlotChange loc, int slot)
        {
            loc = new SlotChange();
            var ctrl = WinFormsUtil.GetUnderlyingControl(sender);
            var obj = ctrl.Parent.Parent;
            if (obj is BoxEditor b)
            {
                loc.Box = b.CurrentBox;
                loc.Slot = slot;
                loc.Offset = b.GetOffset(loc.Slot, loc.Box);
                loc.Parent = b.FindForm();
                sender = ctrl;
                return b.M;
            }
            obj = obj.Parent.Parent;
            if (obj is SAVEditor z)
            {
                loc.Box = z.Box.CurrentBox;
                loc.Slot = slot;
                loc.Offset = z.GetPKMOffset(loc.Slot, loc.Box);
                loc.Parent = z.FindForm();
                sender = ctrl;
                return z.M;
            }
            return null;
        }

        public void SetRegions(string Country, string SubRegion, string ConsoleRegion)
        {
            CB_Country.Text = Country;
            CB_SubRegion.Text = SubRegion;
            CB_3DSReg.Text = ConsoleRegion;
        }

        public PKM SetPKMRegions(int Country, int SubRegion, int ConsoleRegion, PKM pk)
        {
            pk.Country = Country;
            pk.Region = SubRegion;
            pk.ConsoleRegion = ConsoleRegion;
            return pk;
        }

        public string[] parseTrainerData(SAVEditor C_SAV)
        {
            // Defaults
            string TID = "23456";
            string SID = "34567";
            string OT = "Archit";
            string Gender = "M";
            string Country = "";
            string SubRegion = "";
            string ConsoleRegion = "";
            if (!System.IO.File.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\trainerdata.txt"))
            {
                return new string[] { TID, SID, OT, Gender, Country, SubRegion, ConsoleRegion}; // Default No trainerdata.txt handling
            }
            string[] trainerdataLines = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\trainerdata.txt", System.Text.Encoding.UTF8)
                .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            List<string> lstlines = trainerdataLines.OfType<string>().ToList();
            int count = lstlines.Count;
            for (int i =0; i < count; i++)
            {
                string item = lstlines[0];
                if (item.TrimEnd() == "" || item.TrimEnd() == "auto") continue;
                string key = item.Split(':')[0].TrimEnd();
                string value = item.Split(':')[1].TrimEnd();
                lstlines.RemoveAt(0);
                if (key == "TID") TID = value;
                else if (key == "SID") SID = value;
                else if (key == "OT") OT = value;
                else if (key == "Gender") Gender = value;
                else if (key == "Country") Country = value;
                else if (key == "SubRegion") SubRegion = value;
                else if (key == "3DSRegion") ConsoleRegion = value;
                else continue;
            }
            // Automatic loading
            if (trainerdataLines[0] == "auto")
            {
                try
                {
                    int ct = PKMConverter.Country;
                    int sr = PKMConverter.Region;
                    int cr = PKMConverter.ConsoleRegion;
                    if (C_SAV.SAV.Gender == 1) Gender = "F";
                    return new string[] { C_SAV.SAV.TID.ToString("00000"), C_SAV.SAV.SID.ToString("00000"),
                                          C_SAV.SAV.OT, Gender, ct.ToString(),
                                          sr.ToString(), cr.ToString()};
                }
                catch
                {
                    return new string[] { TID, SID, OT, Gender, Country, SubRegion, ConsoleRegion };
                }
            }
            return new string[] { TID, SID, OT, Gender, Country, SubRegion, ConsoleRegion };
        }

    }

}
