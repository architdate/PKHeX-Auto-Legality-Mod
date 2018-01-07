# PGL Rental QR Teams Auto Legality

This feature takes a QR Code posted on PGL for a QR Rental team and automatically generates legal PKM used and sets them as the first 6 PKM in your box.
It also copies the Showdown Set for the QR team and sets it as your Clipboard text

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `PGLRentalLegality` folder by dragging and dropping the `PGLRentalLegality` folder inside `PKHeX.WinForms` subproject.
- Navigate to `PGLRentalLegality/Resources/text` folder and add `pokemonAbilities.csv` and `pokemonFormAbilities.csv` as an Embedded Resource.
- Inside `PKHeX.WinForms` sub project, right click `References` and then go to `Add Reference...` option. 
- Inside the `Assemblies` tab in `Add Reference` search for `System.Numerics` and add it as a reference
- Inside the `Browse` tab click on the Browse button and browse for `BouncyCastle.CryptoExt.dll` inside the repository and add it as a reference
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_Showdown.DropDownItems.Add(EnablePGLRentalLegality(resources));
```

## Dependencies
- [Bouncy Castle](http://www.bouncycastle.org/csharp/licence.html) for their dll file that decrypts AES-CTR
- The QR decoder is from [ZXing.Net](https://www.nuget.org/packages/ZXing.Net/). It is licensed under the [Apache 2.0 License](http://www.apache.org/licenses/LICENSE-2.0)

## Credits
- [SciresM](https://twitter.com/sciresm?lang=en) for his research on how to [decode the QR code](https://gist.github.com/SciresM/f3d20f8c77f5514f2d142c9760939266).
- [SciresM and Kaphotics](https://github.com/kwsch/PKHeX/tree/master/PKHeX.Core/Saves/MemeCrypto) for their work on the updated USUM MemeCrypto and text resources that were used for parsing Showdown Code,
- [Phil-DS](https://github.com/Phil-DS) for his code to parse the decoded QR into a proper Showdown Set.
- [BernardoGiordano](https://github.com/BernardoGiordano) for all the help he provided me on Discord.

Without all these people to help me directly or indirectly, integrating this in PKHeX would have been an impossible task. So thanks a lot!! 
