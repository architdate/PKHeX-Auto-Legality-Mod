# PKHeX Auto Legality Mod Feature List

This markdown file is meant to contain the most up-to-date documentation when it comes to this repository. The information that is contained in this documentation is to be taken as official in regards to this Mod. I do not take any responsibility for external feature lists that may or may not contradict said official documentation. If you believe I may have missed out on any feature or made a mistake in the documentation, feel free to make a pull request to the repository and if deemed fit, your contributions shall be added and acknowledged. Alternatively, you can feel free to contact me on discord at @thecommondude#8240

## Auto Legality

### Introduction to the feature:

Auto Legality is the core feature of this mod. The agenda of this feature is to make genning as easy as possible for the layman user by automatically legalizing any sort of Pokemon Showdown set which can be made easily on [this website](https://play.pokemonshowdown.com/teambuilder). A further detailed explanation of the features is given below.

**NOTE:** If you wish to gen event pokemon as well, download the MGDB database latest release by going to `Tools` then `Auto Legality Mod` and then `Download MGDB`. Once it is done downloading, it will give you a notification saying that the download has finished

### Usage:

- **Single Pokemon** : To legalize a single pokemon set, select the showdown set for that pokemon, Go to `Tools` then `Auto Legality Mod` and then `Import with Auto-Legality Mod`. This will ask for a confirmation, just like importing a showdown set regularly. Once confirmed, it will generate a legalized Pokemon set for you. There is also a shortcut to do the following: After selecting the showdown set, click `Ctrl + I`

- **Multiple Pokemon (Upto 30 at a time)** : Copy a team or a box of Showdown pokemon sets. Make sure that your current active box in PKHeX has enough space to accomodate the number of Pokemon selected. 

Now if you want the Pokemon in the box to be replaced with your new selected sets: Use the shortcut `Ctrl + I` as before. Alternatively you can click `Ctrl` while clicking `Import with Auto-Legality Mod`. A confirmation will NOT be asked in the case of multiple Pokemon, and the first n slots of the currently active box will be filled. (Here n is the number of Pokemon selected/copied in the clipboard)

However if you would like the Multiple pokemon paste to just fill up empty spaces within the box, you can do that too! Just click `Import with Auto-Legality Mod` normally from the `Tools > Auto Legality Mod` menu and it will fill up the empty spaces only and not erase your current Pokemon. No confirmation will be asked for as stated before.

- **Importing from a text file** : You can import Showdown sets stored in a `.txt` file as well. A dialogue box asking for the text file will trigger if there are no contents in the clipboard / if the contents in the clipboard do not correspond to a Showdown Set. Alternatively, you can trigger the dialogue box by clicking `Shift` while clicking `Import with Auto-Legality Mod` or using the shortcut `Ctrl + Shift + I`

### Feature List:

- All Pokemon sets are legalized automatically from any game (atleast I am unaware of any that are not legalized)
- TID, SID, OT, Region, Sub Region, 3DS Region, Gender are all customizable and can be set to be automatically be read from the save file. Refer to `trainerdata.txt` features below
- Happiness is dynamically allocated to 0 in case of Frustration and 255 in case of Return for maximum base power of the respective moves.
- The Marking is automatically done. Markings are based on IV spreads (Also now implemented in PKHeX base code)
- Female gender is assigned by default unless specified in the Showdown Set to have the least maximum likelyhood of being attracted by Pokemon who carry attract (higher female ratio statistically)
- IV's are smartly hypertrained. (0 IV and 1 IV stats are not hypertrained because of intentional IVing. Anything above that is hypertrained)
- Previous Generation of Pokemon (Gen 3, Gen 4 and Gen 5) automatically get PIDIV combos assigned to them to satisfy their nature and their hidden power. The IVs are then hypertrained if needed automatically.
- Ingame encounters of Pokemon and PKHeX inbuilt mystery gifts are given priority over external events to reduce the dependancy on wondercard files as much as possible
- The priority of the games used for legality is in the descending order in which games are released.
- In the occasion that there is no possible Ingame encounter for the Showdown Set, it checks for possible events that can generate a legal Pokemon from the `mgdb` folder in the same directory. If it finds such an event, it will automatically generate a legal Pokemon using that event and the provided Showdown Set.

## Trainer Data Preferences

### Introduction to this feature:

This is an optional `.txt` file that is to be kept in the same directory as the PKHeX executable. This allows you to change preferences of TID, SID, OT, Region, Sub Region, 3DS Region and Gender of your trainer when automatically genning the Pokemon. Start off by creating a file called `trainerdata.txt` in the same folder as your PKHeX

### Usage:

- If you would like the Pokemon to be generated from your save file data, then open `trainerdata.txt` and write `auto` in it. Save the file and you are good to go!
- If you would want specific details to be used, then follow the following format in `trainerdata.txt`:
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

### Trainer Data JSON:
- Used to create trainerdata for different games
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

### Features:

- If you have auto on the first line of the file and then the specific details after that, then it will use the specific details as a fallback mechanism incase auto fails.
- Alternatively there is a preset for the OT, TID, SID, Gender, and Location already coded inside with my own details. So even if you do not use `trainerdata.txt`, I have got you covered :]
- The code smartly reads the file. Different order within the format should not matter anymore.

## Addon Features

## PGL QR Auto Legality:

This feature takes a QR Code posted on [Pokemon Global Link](https://3ds.pokemon-gl.com/rentalteam/usum/) for a QR Rental team and automatically generates legal PKM used and sets them as the first 6 PKM in your box. It also copies the Showdown Set for the QR team and sets it as your Clipboard text for convenience

### Usage:

- Go to the [Pokemon Global Link](https://3ds.pokemon-gl.com/rentalteam/usum/) website. 
- Select any team you would like.
- View the QR code.
- Right click and copy the QR code to your clipboard
- Inside PKHeX, go to `Tools > Auto Legality Mod > Import PGL QR Code` or use the Shortcut: `Alt + Q`
- It will automatically gen all 6 Pokemon for you (Similar to Multiple Pokemon Importing) and copy the team to your clipboard for easy usage on websites such as Pokemon Showdown

## MGDB Updater/ Downloader:

This feature allows you to automatically download the MGDB database in the same directory as PKHeX. If there already exists an MGDB database, it gives you the option to update the database from the latest [Events Gallery Github](https://github.com/projectpokemon/EventsGallery)

### Usage:

- Inside PKHeX, go to `Tools > Showdown > Download MGDB`
- If there is no `mgdb` folder in the same location as the PKHeX executable, it will download the latest events from the latest events gallery release and make the folder for you. You will be notified once the download is done.
- If there already exists an `mgdb` folder, it will give you an option to update it if needed or to cancel the operation. If you choose to update, it will check for the latest release and update your saved events database to the latest release.

## URL Genning:

This feature allows you to automatically gen the showdown set that is contained in a [pastebin](https://pastebin.com/) or [pokepast.es](http://pokepast.es/) URL
Simply copy the URL Address and Click `Gen from URL` in the `Tools > Showdown` menu.

### Usage:

- Go to any pokepast.es paste or to any pastebin and copy the URL to your clipboard
- Go to `Tools > Auto Legality Mod > Gen from URL`
- The Mod will gen all the Pokemon sets within that URL
- Once the genning is done, it will display the information about the Title / Author / Description or Date of the generated pastebin or pokepast.es URL for the ease of the user.

## Export Trainer Data:

This feature allows you to export a `trainerdata.txt` file based on the active Pokemon. (It exports the Pokemons TID, SID, OT, OT gender, Pokemon Country, Pokemon Sub Region and Pokemon 3DS Region)

### Usage:

- Right click on the Pokemon whose trainerdata you want to export.
- Click on View to set it as the active Pokemon for editing
- Go to `Tools > Auto Legality Mod > Export Trainer Data`
- The `trainerdata.txt` file will be exported and ready to use by the auto legality mod.
- You will recieve a confirmation once it's done.

## Smogon Sets Genning:

This feature allows you to generate every single recommended [Smogon](https://www.smogon.com/) set for a particular pokemon.

### Usage:

- In the Pokemon Editor, set the Pokemon and a Form of the Pokemon if necessary.
- Go to `Tools > Auto Legality Mod > Gen Smogon Sets`
- It should gen all available sets in the `SM` dex
- You will recieve a confirmation about how many sets were genned at the end


## Shortcuts:

- **Ctrl + I** : Single Import / Mass Import with replacing Pokemon in a box
- **Ctrl + Shift + I** : Import from a text file
- **Alt + Q** : Import a PGL QR code


**THESE ADDON FEATURES WILL BE PERIODICALLY UPDATED AS AND WHEN NEW ONES ARE ADDED.**

Thank you everyone who has helped me develop the mod so far, be it via GitHub issues, or Discord DMs. I never imagined the mod to have come this far when I first made it. And all I hope is that it keeps growing and people have an easier time Genning! Happy Genning :]

[<img src="https://canary.discordapp.com/api/guilds/401014193211441153/widget.png?style=banner2">](https://discord.gg/9ptDkpV)
