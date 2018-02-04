# MGDB Downloader

This feature allows you to automatically download the MGDB database in the same directory as PKHeX.
If there already exists an MGDB database, it gives you the option to update the database from the latest Events [Gallery Github](https://github.com/projectpokemon/EventsGallery)

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `MGDBDownloader` folder by dragging and dropping the `MGDBDownloader` folder inside `PKHeX.WinForms` subproject.
- Inside `PKHeX.WinForms` sub project, right click `References` and then go to `Add Reference...` option. 
- Inside the `Assemblies` tab in `Add Reference` search for `System.IO.Compression.FileSystem` and add it as a reference
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_Showdown.DropDownItems.Add(EnableMGDBDownloader(resources));
```

## Credits
- [Sabersite / kamronbatman](https://github.com/kamronbatman) and [Project Pokemon](https://github.com/projectpokemon) for his [Event Gallery](https://github.com/projectpokemon/EventsGallery).

