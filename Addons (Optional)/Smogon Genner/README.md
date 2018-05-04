# Smogon Genning

This feature allows you to generate every single recommended [Smogon](https://www.smogon.com/) set for a particular pokemon.
To use this, simply open PKHeX. In the Pokemon Editor, set the Pokemon and a Form of the Pokemon if necessary. Go to `Tools > Auto Legality Mod > Gen Smogon Sets` and it should gen all available sets in the `SM` dex

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `SmogonGenner` folder by dragging and dropping the `SmogonGenner` folder inside `PKHeX.WinForms` subproject.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_AutoLegality.DropDownItems.Add(EnableSmogonGenner(resources));
```