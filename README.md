# GatorRando: Randomizer Client for Lil Gator Game
Lil Gator Game is a game about being an adorable gator who is getting all of their friends involved in their giant hero quest in order to convince their big sister to stop working on her college assignment and instead play with them.

This project is a randomizer mod for [Lil Gator Game](https://store.steampowered.com/app/1586800/Lil_Gator_Game/) that works with [Archipelago](https://archipelago.gg/) (via a custom [APWorld](https://github.com/natronium/GatorArchipelago)) to take the things your character would receive throughout the game and randomize them, potentially between multiple games. If you are not familiar with Archipelago, we recommend reading Archipelago's introduction documents starting with the [FAQ](https://archipelago.gg/faq/en/).

## Installation Instructions
1. Download [BepInEx_win_x64_5.4.23.2.zip](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.2) for Windows and extract the contents into the root of your Lil Gator Game folder (you should now have a `BepInEx` folder in the same folder as `Lil Gator Game.exe`)
2. Run the game and make sure that a plugins folder was generated inside of the BepInEx folder
3. Download the GatorRando zip from the [Releases page](https://github.com/natronium/GatorRando/releases) or build the GatorRando dll from source ([see below](#building-from-source)) or 
4. Add the GatorRando.dll, Archipelago.MultiClient.Net.dll, and Newtonsoft.Json.dll into the `plugins` folder inside the `BepInEx` folder. These dlls can be obtained either by extracting the zip from the [Releases page](https://github.com/natronium/GatorRando/releases) or from "GatorRando\bin\Debug\netstandard2.0" if built from source.
5. See the [APWorld Instructions](https://github.com/natronium/GatorArchipelago) for how to generate and host a game

## Building from Source
These instructions assume familiarity with command line tools. Please ask for help if you are not already familiar with command line tools as you may need to install additional packages.
1. Download and install [.NET version 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or higher
2. In your command line interface of choice, navigate to the directory where you would like to store this project's source files.
3. `git clone` this repository to that location
4. Copy Assembly-CSharp.dll, UnityEngine.dll, and UnityEngine.UI.dll from the game's folder in `Lil Gator Game\Lil Gator Game_Data\Managed` into your local copy of GatorRando's `lib` folder
5. Navigate into the repository and run the command `dotnet build`

## Randomizer Details
### Connecting to the server
1. Enter Connection Details:
	- New Worlds: For each new Archipelago world, you'll need a new save file. Play through the prologue (we recommend speedrun mode on to reduce mashing) and the initial cutscene, then pause the game. The Settings menu will appear and you'll have to enter in connection details. Your character's name is your slot name.  You can copy-paste or type your server address:port into its field in the menu. If you are hosting locally, the server address is localhost
	- Continuing Worlds: When resuming an in-progress save, your connection details will have been saved and the Settings Menu should automatically appear. If your server port has changed (rarely occurs), update it before continuing.
2. Connect to Server: Click "connect to server" once and wait for it to connect. When the quest in the upper right corner is complete, then you have successfully connected and you will be allowed to leave the Settings menu and start playing!

### Recommendations for Better Play Experiences
- For at least your first few plays, we recommend you set `start_with_checkfinders` to be true and go into the Settings Menu and switch the Checkfinder setting to `Logic`. Using the Megaphone and Texting With Jill in game will then point you towards the nearest locations/people that are in logic (available with the items you currently have)
- Download [Universal Tracker](https://github.com/FarisTheAncient/Archipelago/releases) and use it to track which locations are in logic and your position on the map.
- If you are having trouble finding a specific location, you can search the map available at https://natronium.github.io/GatorMap/ to find all the pots, chests, and races with their locations labeled as they are in the Archipelago.
- Use the text client (which you can also open from the Archipelago Launcher) alongside your game. Using the text client will allow you to see what items you are sending and receiving, as only some sent items currently display in game
- If you get stuck somewhere (i.e. without a bracelet), there's a handy "reset position" button in the Settings Menu!

### Features added to the original game
- New Tab in Item Inventory that displays all quest items (ex. Retainer) that you have received in order to be able to keep track of who you need to talk to

### What Things are Randomized?
- Items: what you get from players doing things
	- Craft ideas
	- Inventory Items (swords/shields/hats/special items received directly rather than as crafts)
	- Quest Items (things needed to complete quests)
	- Friends (the resource used to build the playground)
	- Craft Stuff bundles
- Locations: what you do to give players things
	- Quest-related
		- NPC items given during quests
		- NPC craft/item rewards from quests
		- Friends (when the population counter would be incremented) received through quests
	- Found objects (Retainer and Broken Scooter)
	- Pots
	- Chests
	- Races

## Goal
Right now, the goal is to finish the playground (takes 35 Friends worth of Friend items and completing all three main quests), play through the flashback, and watch the credits.

### Quest Item List and Uses
- 3x Thrown Pencil for each stage of Sam (Clumsy Jackal)'s giant pencil dropping quest
- Grass Clippings and Water for Jada (Cool Boar)
- Retainer for Becca (Retainer Shark)
- Sorbet for Esme (Vampire Bat)
- Magic Ore for Susanne (Paleolithic Gazelle)
- Cheese Sandwich for Gene (Merchant Beaver)
- Pot? for Martin (Horse)'s Tutorial quest

### Requirements to do specific checks
In addition to the quest items above needing to be received to progress in their relevant quest, below are some requirements for quests that may not be obvious from base game (because you would have these items by default at the relevant point in the game)
- You must craft Pointy Floppy Thing (Craft) to finish Avery's Tutorial Quest
- Jill's Tutorial Quest must be completed for Franny (Stick Duck) to appear
- Bug Net is required to try to catch the Beetle in Antone (Beetle Iguana)'s quest
- A sword is required to collect Grass Clippings for Jada (Cool Boar)'s quest
- Bucket is required to collect Water for Jada (Cool Boar)'s quest
- Either a sword or a ranged weapon is required to process the ore in Susanne (Paleolithic Gazelle)'s quest (shield doesn't seem to hit fast enough)
- The Rock is required for Zhu (Skip Fox)'s quest (but you can skip anywhere on the map, Zhu is watching)

## Future plans
See the [issues with the "feature" tag](https://github.com/natronium/GatorRando/issues?q=is%3Aissue+is%3Aopen+label%3Afeature) for things that we believe are doable and are planning to implement in the short-term in this mod (see apworld for logic related issues). We may decide to scrap or change these proposals depending on feasibility, user experience during testing, or other constraints. We do not guarantee that we will ever get around to doing everything in the issues, especially those tagged as "idea". If you are interested, please feel free to work on ideas and suggest modifications.
