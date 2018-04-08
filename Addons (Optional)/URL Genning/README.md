# URL Genning

This feature allows you to automatically gen the showdown set that is contained in a [pastebin](https://pastebin.com/) or [pokepast.es](http://pokepast.es/) URL
Simply copy the URL Address and Click `Gen from URL` in the `Tools > Showdown` menu.

## How to set up the mod.

- First of all have the basic mod setup.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- Add the `URLGenning` folder by dragging and dropping the `URLGenning` folder inside `PKHeX.WinForms` subproject.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
        this.Menu_Showdown.DropDownItems.Add(EnableURLGenning(resources));
```