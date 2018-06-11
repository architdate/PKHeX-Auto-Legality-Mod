using System;
using System.Collections.Generic;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {
        private void ExportBoxToShowdown(object sender, EventArgs e)
        {
            try
            {
                IList<PKM> BoxData = C_SAV.SAV.BoxData;
                List<PKM> BoxList = new List<PKM>(BoxData);
                List<PKM> CurrBox = BoxList.GetRange(C_SAV.CurrentBox * C_SAV.SAV.BoxSlotCount, C_SAV.SAV.BoxSlotCount);
                var str = ShowdownSet.GetShowdownSets(CurrBox, Environment.NewLine + Environment.NewLine);
                if (string.IsNullOrWhiteSpace(str)) return;
                Clipboard.SetText(str);
            }
            catch { }
            WinFormsUtil.Alert("Exported the active box to Showdown format");
        }
    }
}
