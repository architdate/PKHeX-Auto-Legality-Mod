using System;
using System.Drawing;
using System.Windows.Forms;
using static PKHeX.Core.MessageStrings;
using PKHeX.Core;
using PKHeX.WinForms.Properties;
using System.Collections.Generic;
using System.IO;

namespace PKHeX.WinForms
{
    public partial class Main : Form
    {

        private void ExportQRCodes(object sender, EventArgs e)
        {
            SaveFile SAV = C_SAV.SAV;
            var boxdata = SAV.BoxData;
            if (boxdata == null)
            {
                WinFormsUtil.Alert("Box Data is null");
            }
            int ctr = 0;
            Dictionary<string, Image> qrcodes = new Dictionary<string, Image>();
            foreach (PKM pk in boxdata)
            {
                if (pk.Species == 0 || !pk.Valid || (pk.Box - 1) != C_SAV.Box.CurrentBox)
                    continue;
                ctr++;
                Image qr;
                switch (pk.Format)
                {
                    case 7:
                        qr = QR.GenerateQRCode7((PK7)pk);
                        break;
                    default:
                        if (pk.Format == 6 && !QR6Notified) // users should not be using QR6 injection
                        {
                            WinFormsUtil.Alert(MsgQRDeprecated, MsgQRAlternative);
                            QR6Notified = true;
                        }
                        qr = QR.GetQRImage(pk.EncryptedBoxData, QR.GetQRServer(pk.Format));
                        break;
                }
                if (qr == null) continue;
                var sprite = dragout.Image;
                var la = new LegalityAnalysis(pk, C_SAV.SAV.Personal);
                if (la.Parsed && pk.Species != 0)
                {
                    var img = la.Valid ? Resources.valid : Resources.warn;
                    sprite = ImageUtil.LayerImage(sprite, img, 24, 0, 1);
                }
                string[] r = pk.QRText;
                string refer = "PKHeX Auto Legality Mod";
                qrcodes.Add(Util.CleanFileName(pk.FileName), RefreshImage(qr, sprite, r[0], r[1], r[2], $"{refer} ({pk.GetType().Name})"));
            }
            if (!Directory.Exists(Path.Combine(WorkingDirectory, "qrcodes")))
                Directory.CreateDirectory(Path.Combine(WorkingDirectory, "qrcodes"));
            int counter = 0;
            foreach (KeyValuePair<string, Image> qrcode in qrcodes)
            {
                Console.WriteLine(counter);
                counter++;
                qrcode.Value.Save(Path.Combine(WorkingDirectory, "qrcodes", qrcode.Key + ".png"));
            }
        }

        private Image RefreshImage(Image qr, Image icon, params string[]lines)
        {
            Font font = FontUtil.GetPKXFont((float)8.25);
            Image preview = new Bitmap(45, 45);
            using (Graphics gfx = Graphics.FromImage(preview))
            {
                gfx.FillRectangle(new SolidBrush(Color.White), 0, 0, preview.Width, preview.Height);
                gfx.DrawImage(icon, preview.Width / 2 - icon.Width / 2, preview.Height / 2 - icon.Height / 2);
            }
            // Layer on Preview Image
            Image pic = ImageUtil.LayerImage(qr, preview, qr.Width / 2 - preview.Width / 2, qr.Height / 2 - preview.Height / 2, 1);

            Image newpic = new Bitmap(405, 455);
            using (Graphics g = Graphics.FromImage(newpic))
            {
                g.FillRectangle(new SolidBrush(Color.White), 0, 0, newpic.Width, newpic.Height);
                g.DrawImage(pic, 0, 0);

                g.DrawString(GetLine(0, lines), font, Brushes.Black, new PointF(18, qr.Height - 5));
                g.DrawString(GetLine(1, lines), font, Brushes.Black, new PointF(18, qr.Height + 8));
                g.DrawString(GetLine(2, lines).Replace(Environment.NewLine, "/").Replace("//", "   ").Replace(":/", ": "), font, Brushes.Black, new PointF(18, qr.Height + 20));
                g.DrawString(GetLine(3, lines), font, Brushes.Black, new PointF(18, qr.Height + 32));
            }
            Image BackgroundImage = newpic;
            return BackgroundImage;
        }

        private string GetLine(int line, string[] Lines) => Lines.Length <= line ? string.Empty : Lines[line];
    }
}
