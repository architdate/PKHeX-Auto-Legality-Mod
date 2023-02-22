# PKHeX Automatic Legality Mod

**DISCONTINUATION NOTICE (MOD MOVED TO [THIS REPOSITORY](https://github.com/architdate/PKHeX-Plugins))**
The development on this repository has been discontinued since 23/11/2018.
The project is being continued in the form of plugins, which is a much more elegant way of handling mods. It is also easier to build and distribute to users.
The plugins are being developed and released in the [PKHeX-Plugins](https://github.com/architdate/PKHeX-Plugins) repository
Thank you for your continued support with this mod.

[![Build status](https://ci.appveyor.com/api/projects/status/s3py0ymkh22sq3ok/branch/master?svg=true)](https://ci.appveyor.com/project/architdate/pkhex-auto-legality-mod/branch/master)

PKHeX Automatic Legality Mod is for being able to make instant legal Pokémon on [PKHeX](https://github.com/kwsch/PKHeX) using just a [Pokémon Showdown](https://github.com/Zarel/Pokemon-Showdown) teambuilder template.

What is a mod exactly? In this case a mod is somewhat like a macro being used in the given framework without being greatly changed from the original and also making the Pokemon legal.

This repository contains the files needed to set up your own PKHeX build with all the amazing stuff that it has as well as the Pokemon Showdown Mod in it.

For more information on building and usage, contact me on Discord at thecommondude#8240

The teambuilder for the Pokemon templates can be found on:
[Pokémon Showdown](http://play.pokemonshowdown.com/teambuilder)

## Latest Builds Testing [May be unstable]

The latest builds can be tested by users. They are available for download in the #builds channel in the [Discord](https://discord.gg/9ptDkpV) server. These builds may be unstable sometimes so there might be some bugs. For a more stable experience with tested features, use the latest releases!

## Features

**A COMPLETE LIST OF FEATURES CAN BE SEEN [HERE](https://github.com/architdate/PKHeX-Auto-Legality-Mod/blob/master/Features.md)**

- Legality for all egg based Pokemon
- Legality for all legendary Pokemon from all generations
- Auto Legality support for all Pokemon in all generations except Colosseum and XD
- Mystery Gift Legality based on `mgdb` database provided
- Supports HaX easter egg in PKHeX
- Supports multiple Pokemon import to the first `n` available slots in the current box (`n` is the number of pokemon being imported)
- Hold `Ctrl` key while mass importing to replace the first `n` slots in the box
- Supports custom OT, TID, SID, Gender, Country, SubRegion and 3DSRegion setting wherever possible.
- Also allows automatic filling of the above settings if detected in a save file
- Supports error handling for -Mega and -Busted pokemon (All megas and mimikyu)
- Custom import short cut for Auto-mod: `Ctrl + I`
- Ability to gen Pokemon from a `txt` file as long as it is properly formatted.
- Additional features from the Addon mods

## How to quick set-up the mod.

- If you plan to build the mod with all of its sub modules, the instructions below are more helpful (Installing the Mod + Addons)
- (Requires a C# IDE such as Visual Studio 17 or Mono Develop)

- First of all download PKHeX by doing the following.

```
$ git clone https://github.com/kwsch/PKHeX.git
```
- Copy the files in this repository to the PKHeX repository while following the directory structure.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- It is always a good practice to build the solution first to resolve any unresolved errors with the solution.
- To do this, right click on the main PKHeX project and click on Rebuild all. Wait for the whole process to finish.
- Add the `AutoLegality` folder by dragging and dropping the `AutoLegality` folder inside `PKHeX.WinForms` subproject.
- Inside the `AutoLegality/Resources/text` folder click on `evolutions.txt` file and set it as an `Embedded Resource` in the property box below.
- Go to `MainWindow` folder and open the `Main.Designer.cs` file.
- Search for the line `this.Menu_Showdown.DropDownItems` using `Ctrl + F`.
- After the semicolon (`;`) write this line of code on the next line

```csharp
            this.Menu_Tools.DropDownItems.Insert(0, EnableMenu(resources));
            this.Menu_AutoLegality.DropDownItems.Add(EnableAutoLegality(resources));
```

- Right click on the main PKHeX project and click Rebuild all.
- The output of the PKHeX file should be in `PKHeX\PKHeX.WinForms\bin\Debug` folder.

## Installing the Mod + Addons

- Make sure you have Python 3.x installed. It can be downloaded from [Python's website](https://www.python.org/downloads/) (Make sure to set python in PATH)
- If you do not have `lxml` installed, use the following command to install `lxml`
```
            pip3 install lxml
```
- Once you have made sure that you have Python 3.x and lxml installed, copy over the `AutoLegality` folder, the `Addons (Optional)\PGL QR Auto Legality\PGLRentalLegality` folder and the `Addons (Optional)\MGDB Downloader\MGDBDownloader` folder over to `PKHeX.WinForms` folder in your PKHeX directory
- Also copy over `build-mod.py` to `PKHeX.WinForms`
- Open Command Line in `PKHeX.WinForms` and type the following command
```
            py -3 build-mod.py
```
- Once the patching is complete, open the solution in Visual Studio 2017 and build the project.

## Tutorial Video

- Thanks to [ZMarotrix](https://www.youtube.com/user/zmarotrix) for the updated video tutorial.

https://www.youtube.com/watch?v=Yak_eNAUO7I&feature=youtu.be

## [OPTIONAL] TID, SID, OT, Country, Sub Region, Console Region settings.

- Create a new text file called `trainerdata.txt` in the same directory as `PKHeX.exe`

**Automatic TID, SID, OT, Country, Sub Region, Console Region settings**
- Inside `trainerdata.txt` write `auto` and save.
- This will try and automatically pull Trainer and Region values from your loaded save file

**Specific/Fallback TID, SID, OT, Country, Sub Region, Console Region settings**
- Inside the directory paste your TID, SID ,OT, Gender, Country, SubRegion and 3DSRegion based on the sample given below.
- Note: Follow the format of the sample given below. DO NOT change the format. Just edit the values.
- The `trainerdata.txt` format should be as follows:
```
TID:12345
SID:54321
OT:PKHeX
Gender:M
Country:Canada
SubRegion:Alberta
3DSRegion:Americas (NA/SA)
```
- Gender can be specified as `M` or `F` or `Male` or `Female`
- Country, SubRegion and 3DSRegion have to be spelt exactly as one of the options on PKHeX. Any spelling errors WILL fail.
- To ensure proper legality in regards to Country, SubRegion and 3DSRegion, make sure that the SubRegion actually belongs to the Country, and the Country actually lies in the 3DSRegion.
- **Note**: If the first line of the `trainerdata.txt` is `auto`, it will check for the above values in the SAV file first. If it cannot find those values, it will use the values specified below in the rest of the file

*Credits to the several people who requested this in GitHub Issues*

**Specific trainerdata settings for specific games using trainerdata.json**
- Create a new JSON file called `trainerdata.json`
- Write the json file in the following format:
```
{
    <Game ID string>: {
        "TID":"",
        "SID":"",
        "OT":"",
        "Gender":"",
        "Country":"",
        "SubRegion":"",
        "3DSRegion":""
    }
}
```
- The game ID values are as follows:
```
33: Ultra Moon
32: Ultra Sun
31: Moon
30: Sun
26: Alpha Sapphire
27: Omega Ruby
24: X
25: Y
21: Black
23: Black 2
20: White
22: White 2
10: Diamond
11: Pearl
12: Platinum
7: Heart Gold
8: Soul Silver
2: Ruby
1: Sapphire
3: Emerald
4: Fire Red
5: Leaf Green
15: Colosseum/XD
35: Red
36: Green
37: Blue
38: Yellow
39: Gold
40: Silver
41: Crystal
```
- Example of a `trainerdata.json` file would be (This is specific data for Ultra Sun and Omega Ruby. This JSON can be extended to include games you like. If the JSON does not have the game, it will fall back on `trainerdata.txt` or the hardcoded default values):
```json
{
    "32":{
        "TID":"12345",
        "SID":"61234",
        "OT":"PKHeXer",
        "Gender":"M",
        "Country":"Canada",
        "SubRegion":"Alberta",
        "3DSRegion":"Americas (NA/SA)"
    },
    "27":{
        "TID":"23455",
        "SID":"23444",
        "OT":"ORAS",
        "Gender":"F",
        "Country":"Singapore",
        "SubRegion":"Singapore",
        "3DSRegion":"Americas (NA/SA)"
    }
}
```

## [OPTIONAL] Addon Legality mods:

The instructions for each one of these will be located within their own folders within the `Addons (Optional)` folder in the repository
Current Addons:
- PGL Rental QR Teams auto legal genning. Also copies the Showdown spread to clipboard for convenience.
- MGDB Downloader. Downloads and updates the latest MGDB file. Requires an internet connection.

## Adding Priority to event searches.

- If you would like to have certain events to be scanned first (eg Gen7 events or Gen6 events etc.) do the following:
- Set an order of priority to your folders in the `mgdb` folder.
- For example if you want `Gen7` folder to be scanned first, you can rename the folder as `01_Gen7` to set its priority as 1 so it will be scanned first. You can do the same for any folder you wish to have second/third/fourth priority and so on.
- This will help reduce time in event scanning since with all events in the folder, a full top to bottom scan can take upto a good five minutes.
- The time complexity right now is O(n) and is planned on being reduced for faster searches, but this is a nifty trick for now.

## Maintainers

Just me right now, but if you want to contribute, feel free to contact me on Discord at thecommondude#8240

## Credits
- IV to PID conversion code taken [RNGReporter](https://github.com/Admiral-Fish/RNGReporter) by [Admiral-Fish](https://github.com/Admiral-Fish) which is under the GNU General Public License v2.0.
- [chenzw95](https://github.com/chenzw95) for helping with the continuous integration
- My good friend TORNADO for helping me with Test Cases
- Speed Improvement ideas by [Bernardo Giordano](https://github.com/BernardoGiordano)
- [Project Pokémon](https://github.com/projectpokemon/) for their Event Gallery
- kwsch for the original PKHeX repository and with plugin structures.
- All the other users who have helped improve this via GitHub issues
