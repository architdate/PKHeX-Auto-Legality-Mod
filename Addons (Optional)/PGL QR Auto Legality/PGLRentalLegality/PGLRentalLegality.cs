using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;
using PKHeX.WinForms.Controls;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {        
        private void PGLShowdownSet(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage()) return;
            Misc.RentalTeam rentalTeam = new Misc.QRParser().decryptQRCode(Clipboard.GetImage());
            string data = "";
            foreach (Misc.Pokemon p in rentalTeam.team)
            {
                data += p.ToShowdownFormat(false) + "\n\r";
            }

            Clipboard.SetText(data.TrimEnd());
            try { ClickShowdownImportPKMModded(sender, e); }
            catch { }
            WinFormsUtil.Alert("Exported OwO");
        }
    }
}