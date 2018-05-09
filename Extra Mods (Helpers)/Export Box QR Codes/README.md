# Export Box QR Codes

This feature allows you to export QR codes of all PKM files within a box (can be used for QR injection)

## How to set up the mod.

- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `ExportQRCodes` folder by dragging and dropping the `ExportQRCodes` folder inside `PKHeX.WinForms` subproject.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_AutoLegality.DropDownItems.Add(EnableExportQRCodes(resources));
```