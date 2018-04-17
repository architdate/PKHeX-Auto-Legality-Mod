# Export Trainer Data

This feature allows you to export a `trainerdata.txt` file based on the active Pokemon. (It exports the Pokemons TID, SID, OT, OT gender, Pokemon Country, Pokemon Sub Region and Pokemon 3DS Region)

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `ExportTrainerData` folder by dragging and dropping the `ExportTrainerData` folder inside `PKHeX.WinForms` subproject.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_Showdown.DropDownItems.Add(EnableExportTrainerData(resources));
```