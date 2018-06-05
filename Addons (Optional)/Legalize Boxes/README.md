# Legalize Boxes

This feature allows you to legalize all the illegal Pokemon within the active box.

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `LegalizeBoxes` folder by dragging and dropping the `LegalizeBoxes` folder inside `PKHeX.WinForms` subproject.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_AutoLegality.DropDownItems.Add(EnableLegalizeBoxes(resources));
```