using System;
using System.Linq;
using System.Media;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

using PKHeX.Core;
using PKHeX.WinForms.Properties;

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

        public void hardReset()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "PKHeX.WinForms.AutoLegality.Resources.byte.reset.pk7";
            System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
            System.IO.StreamReader filestr = new System.IO.StreamReader(stream);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            stream.CopyTo(ms);
            byte[] pk7reset = ms.ToArray();
            CurrentSAV = new PKHeX.WinForms.Controls.SAVEditor();
            CurrentSAV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Right)));
            CurrentSAV.Location = new System.Drawing.Point(292, 26);
            CurrentSAV.Name = "C_SAV";
            CurrentSAV.Size = new System.Drawing.Size(310, 326);
            CurrentSAV.TabIndex = 104;
            if (TryLoadPKM(pk7reset, "", "pk7", CurrentSAV.SAV))
            {
                return;
            }
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

    }

}
