# PGL Rental QR Teams Auto Legality

This feature takes a QR Code posted on PGL for a QR Rental team and automatically generates legal PKM used and sets them as the first 6 PKM in your box.
It also copies the Showdown Set for the QR team and sets it as your Clipboard text

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Right click on the `Misc` folder in `PKHeX.WinForms` sub-project and in the `Add` menu select `Existing Item`.
- Add the `CryptoUtil.cs` file from the `Addons (Optional)/PGL QR Auto Legality/PKHeX.WinForms/Misc` directory to the `Misc` folder.
- Add the `QRParser.cs` file from the `Addons (Optional)/PGL QR Auto Legality/PKHeX.WinForms/Misc` directory to the `Misc` folder.
- Add `pokemonAbilities.csv` and `pokemonFormAbilities.csv` file from `Addons (Optional)/PGL QR Auto Legality/PKHeX.WinForms/Resources/text` directory to the `Resources/text` folder.
- Click on both the above files and set them as an `Embedded Resource` in the property box below.
- Drag the whole `ZXing` folder inside `PKHeX.WinForms` sub project
- Inside `PKHeX.WinForms` sub project, right click `References` and then go to `Add Reference...` option. 
- Inside the `Assemblies` tab in `Add Reference` search for `System.Numerics` and add it as a reference
- Inside the `Browse` tab click on the Browse button and browse for `BouncyCastle.CryptoExt.dll` inside the repository and add it as a reference
- Go to `MainWindow` folder and expand `Main.cs` and open the `Main` code section.
- Create a new function after `ClickShowdownImportPKM` function for PGL Showdown Set as follows
```csharp
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
            ClickShowdownImportPKM(sender, e);
            WinFormsUtil.Alert("Exported OwO");
        }
```
- Go to `MainWindow` folder and expand `Main.cs` and open `Main.Designer.cs` and search for the following line of code in the file:
```csharp
        this.Menu_ShowdownExportBattleBox.Click += new System.EventHandler(this.ClickShowdownExportBattleBox);
```
- Replace this line with the following code line:
```csharp
        this.Menu_ShowdownExportBattleBox.Click += new System.EventHandler(this.PGLShowdownSet);
```
- This will replace the `Export Battle Box` option to generate PKM based on PGL QR codes. This option was chosen to be replaced since it is rarely used practically
- Alternatively devs can modify this file to just create a new option inside the menu and map the function to the new menu click event. This will not be covered in the scope of this README. This feature will however be included as a new menu in every subsequent release.

## Dependancies
- [Bouncy Castle](http://www.bouncycastle.org/csharp/licence.html) for their dll file that decrypts AES-CTR

## Credits
- [SciresM](https://twitter.com/sciresm?lang=en) for his research on how to [decode the QR code](https://gist.github.com/SciresM/f3d20f8c77f5514f2d142c9760939266).
- [SciresM and Kaphotics](https://github.com/kwsch/PKHeX/tree/master/PKHeX.Core/Saves/MemeCrypto) for their work on the updated USUM MemeCrypto and text resources that were used for parsing Showdown Code,
- [Phil-DS](https://github.com/Phil-DS) for his code to parse the decoded QR into a proper Showdown Set.
- [BernardoGiordano](https://github.com/BernardoGiordano) for all the help he provided me on Discord.

Without all these people to help me directly or indirectly, integrating this in PKHeX would have been an impossible task. So thanks a lot!! 
