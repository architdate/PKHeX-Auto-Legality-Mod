using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PKHeX.WinForms.AutoLegality
{
    public class ConstData
    {
        public byte[] resetpk7 = new byte[] { 0, 0, 0, 0, 0, 0, 205, 56, 34, 3, 0, 0, 57, 48, 49, 212, 0, 0, 0, 0, 101, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 77, 0, 97, 0, 114, 0, 115, 0, 104, 0, 97, 0, 100, 0, 111, 0, 119, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 80, 0, 75, 0, 72, 0, 101, 0, 88, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 17, 6, 29, 0, 0, 0, 0, 0, 4, 0, 0, 31, 64, 0, 0, 2, 0, 0, 0, 0, };
        public byte[] resetpk6 = new byte[] { 0, 0, 0, 0, 0, 0, 16, 48, 209, 2, 0, 0, 57, 48, 49, 212, 0, 0, 0, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 86, 0, 111, 0, 108, 0, 99, 0, 97, 0, 110, 0, 105, 0, 111, 0, 110, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 35, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 80, 0, 75, 0, 72, 0, 101, 0, 88, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 18, 3, 7, 0, 0, 0, 0, 0, 4, 0, 0, 26, 64, 0, 0, 2, 0, 0, 0, 0, };
        public static string changelog = @"
- Added Legalize Pokemon/ Legalize Boxes addon
- Added a discord banner within the application
- Added update checker for the latest update
- Added a changelog window that displays on boot for a new version
- Fix SmogonGenner issue of not reading multiple sets
- Extreme speed improvements (Genning is 20.3x faster with the mod now)
- Just for reference, a whole box of Pokemon gets genned in 5 milliseconds on my machine.
- Current base PKHeX commit [e0aa193](https://github.com/kwsch/PKHeX/commit/e0aa1934e7be00955dede723c75dfc555472dc7c)
";
        public static string keyboardshortcuts = @"
- `Ctrl + I` : Auto Legality import from clipboard
- `Shift + Click Import from Auto Legality Mod` : Auto Legality import from a `.txt` file
- `Alt + Q` : PGL QR code genning. Also saves the showdown import to your clipboard!
- `Ctrl + Mass Import` replaces the first Pokemon in the box. Otherwise it sets them in empty places!
";
        public static string discord = @"https://discord.gg/9ptDkpV";
    }
}
