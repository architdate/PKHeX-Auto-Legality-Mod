# Export Box To Showdown

This feature allows you to export the showdown set of every single pokemon in the active box

## How to set up the mod.

- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `ExportBoxToShowdown` folder by dragging and dropping the `ExportBoxToShowdown` folder inside `PKHeX.WinForms` subproject.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_AutoLegality.DropDownItems.Add(EnableExportBoxToShowdown(resources));
```