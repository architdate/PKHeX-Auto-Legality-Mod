# PKHeX Automatic Legality Mod

PKHeX Automatic Legality Mod is for being able to make instant legal Pokémon on [PKHeX](https://github.com/kwsch/PKHeX) using just a [Pokémon Showdown](https://github.com/Zarel/Pokemon-Showdown) teambuilder template.

What is a mod exactly? In this case a mod is somewhat like a macro being used in the given framework without being greatly changed from the original and also making the Pokemon legal.

This repository contains the files needed to set up your own PKHeX build with all the amazing stuff that it has as well as the Pokemon Showdown Mod in it.

For more information on building and usage, contact me on Discord at thecommondude#8240

The teambuilder for the Pokemon templates can be found on:
[Pokémon Showdown](http://play.pokemonshowdown.com/teambuilder)

## Features

- Legality for all egg based Pokemon
- Legality for all legendary Pokemon from all generations
- Auto Legality support for all Pokemon in all generations except Colosseum and XD
- Mystery Gift Legality based on `mgdb` database provided
- Supports HaX easter egg in PKHeX
- Supports box/team imports (Sets it to the current box)
- Supports custom OT, TID, SID setting whereever possible.
- Supports error handling for -Mega and -Busted pokemon (All megas and mimikyu)

## Known Issues

- ~~Issues with legalizing Shiny Groudon and Shiny Kyogre. This is because of PID mismatches in Gen 4 Games. (Its being resolved)~~ [RESOLVED]
- ~~No proper error handling for -Mega or -Busted forms. (This should be added in the next commit)~~ [RESOLVED]

(Requires a C# IDE such as Visual Studio 17 or Mono Develop)

## How to quick set-up the mod.

- First of all download PKHeX by doing the following.

```
$ git clone https://github.com/kwsch/PKHeX.git
```
- Copy the files in this repository to the PKHeX repository while following the directory structure.
- Open the `.sln` solution file using Visual Studio 17 / Mono Develop.
- It is always a good practice to build the solution first to resolve any unresolved errors with the solution.
- To do this, right click on the main PKHeX project and click on Rebuild all. Wait for the whole process to finish.
- Right click on the `Misc` folder in `PKHeX.WinForms` sub-project and in the `Add` menu select `Existing Item`.
- Add the `ArchitMod.cs` file from the `PKHeX.WinForms/Misc` directory to the `Misc` folder.
- Add the `IVtoPIDGenerator.cs` file from the `PKHeX.WinForms/Misc` directory to the `Misc` folder.
- Add the `PKSMAutoLegality.cs` file from the `PKHeX.WinForms/Misc` directory to the `Misc` folder.
- Similarly add `reset.pk7` file from `PKHeX.Winforms/Resources/byte` directory to the `Resources/byte` folder.
- Click on `reset.pk7` file and set it as an `Embedded Resource` in the property box below.
- Similarly add `evolutions.txt` file from `PKHeX.Winforms/Resources/text` directory to the `Resources/text` folder.
- Click on `evolutions.txt` file and set it as an `Embedded Resource` in the property box below.
- Go to `MainWindow` folder and expand `Main.cs` and open the `Main` code section.
- Search for the function `ClickShowdownImportPKM` using `Ctrl + F`.
- Replace **ALL THE CONTENTS** in that function with 
```csharp
            if (!Clipboard.ContainsText())
            { WinFormsUtil.Alert("Clipboard does not contain text."); return; }

            int TID = -1;
            int SID = -1;
            string OT = "";
            if(File.Exists(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\trainerdata.txt"))
            {
                string text = File.ReadAllText(System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\trainerdata.txt", System.Text.Encoding.UTF8);
                string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                TID = Convert.ToInt32(lines[0].Split(':')[1].Trim());
                SID = Convert.ToInt32(lines[1].Split(':')[1].Trim());
                OT = lines[2].Split(':')[1].Trim();
            }

            string source = Clipboard.GetText();
            string[] stringSeparators = new string[] { "\n\r" };
            string[] result;

            // ...
            result = source.Split(stringSeparators, StringSplitOptions.None);
            Console.WriteLine(result.Length);

            if (result.Length > 1)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    ShowdownSet Set = new ShowdownSet(result[i]);
                    if (Set.InvalidLines.Any())
                        WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

                    // Set Species & Nickname
                    bool resetForm = false;
                    PKME_Tabs.hardReset();
                    if (Set.Form == null) { }
                    else if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")
                    {
                        resetForm = true;
                        Console.WriteLine(Set.Species);
                    }
                    PKME_Tabs.LoadShowdownSet(Set);
                    PKM p = PreparePKM();
                    p.Version = (int)GameVersion.MN;
                    Blah b = new Blah();
                    PKM legal = b.LoadShowdownSetModded_PKSM(p, resetForm, TID, SID, OT);
                    PKME_Tabs.PopulateFields(legal);
                    if (!new LegalityAnalysis(legal).Valid)
                    {
                        PKME_Tabs.LoadShowdownSetModded(Set, true);
                    }
                    PKM pk = PreparePKM();
                    PKME_Tabs.ClickSet(C_SAV.Box.SlotPictureBoxes[0], i);
                }
            }
            else
            {
                // Get Simulator Data
                ShowdownSet Set = new ShowdownSet(Clipboard.GetText());

                if (Set.Species < 0)
                { WinFormsUtil.Alert("Set data not found in clipboard."); return; }

                if (Set.Nickname?.Length > C_SAV.SAV.NickLength)
                    Set.Nickname = Set.Nickname.Substring(0, C_SAV.SAV.NickLength);

                if (DialogResult.Yes != WinFormsUtil.Prompt(MessageBoxButtons.YesNo, "Import this set?", Set.Text))
                    return;

                if (Set.InvalidLines.Any())
                    WinFormsUtil.Alert("Invalid lines detected:", string.Join(Environment.NewLine, Set.InvalidLines));

                // Set Species & Nickname
                //PKME_Tabs.LoadShowdownSet(Set);
                bool resetForm = false;
                PKME_Tabs.hardReset();
                if (Set.Form == null) { }
                else if (Set.Form.Contains("Mega") || Set.Form == "Primal" || Set.Form == "Busted")
                {
                    resetForm = true;
                    Console.WriteLine(Set.Species);
                }
                PKME_Tabs.LoadShowdownSet(Set);
                PKM p = PreparePKM();
                p.Version = (int)GameVersion.MN;
                Blah b = new Blah();
                PKM legal = b.LoadShowdownSetModded_PKSM(p, resetForm, TID, SID, OT);
                PKME_Tabs.PopulateFields(legal);
                if (!new LegalityAnalysis(legal).Valid)
                {
                    PKME_Tabs.LoadShowdownSetModded(Set, true);
                }
            }
```
- Right click on the main PKHeX project and click Rebuild all.
- The output of the PKHeX file should be in `PKHeX\PKHeX.WinForms\bin\Debug` folder.

## [OPTIONAL] Custom TID, SID, OT settings.

- Create a new text file called `trainerdata.txt` in the same directory as `PKHeX.exe`
- Inside the directory paste your TID, SID and OT based on the sample given below.
- Note: Follow the format of the sample given below. DO NOT change the format. Just edit the values.
- The `trainerdata.txt` format should be as follows:
```
TID:12345
SID:54321
OT:PKHeX
```

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
- My good friend TORNADO for helping me with Test Cases
- Speed Improvement ideas by [Bernardo Giordano](https://github.com/BernardoGiordano)
- kwsch for the original PKHeX repository.
