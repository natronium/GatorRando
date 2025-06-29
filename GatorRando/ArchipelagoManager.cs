using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Data;
using System;
using System.Linq;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json.Linq;
using GatorRando.UIMods;

namespace GatorRando;

public static class ArchipelagoManager
{
    public const string APVersion = "0.5.1";
    public static ArchipelagoSession Session;
    public static LoginSuccessful LoginInfo;
    private static ConcurrentQueue<Items.Entry> ItemQueue = new();
    private static readonly Dictionary<long, ItemInfo> LocationLookup = [];
    private static readonly Dictionary<string, Action> SpecialItemFunctions = [];
    private static readonly Dictionary<string, Action> SpecialLocationFunctions = [];
    public static bool LocationAutoCollect = true;
    private static readonly string LocationKeyPrefix = "AP ID: ";
    private static List<long> AccessibleLocations;
    private static readonly string[] excludedNPCs = ["NPC_LunchSwapCardinal", "NPC_Bee", "NPC_Ninja_Tiger", "NPC_SwimSheep", "Dialogue Actor No Longer Exists"];
    public static void RegisterItemListener(string itemName, Action listener) => SpecialItemFunctions[itemName] = listener;
    public static void RegisterLocationListener(string locationName, Action listener) => SpecialLocationFunctions[locationName] = listener;

    public static bool IsItemUnlocked(string item) =>
        Session.Items.AllItemsReceived.Select(info => info.ItemId).Contains(GetItemApId(item));

    public static bool IsLocationCollected(string location)
    {
        if (LocationAutoCollect)
        {
            try
            {
                return Session.Locations.AllLocationsChecked.Contains(GetLocationApId(location));
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }
        else
        {
            return CheckIfAPLocationInSave(GetLocationApId(location));
        }
    }

    public static int GetItemUnlockCount(string itemName) =>
        Session.Items.AllItemsReceived.Where(itemInfo => itemInfo.ItemId == GetItemApId(itemName)).Count();

    public static bool CheckIfAPLocationInSave(long id) => Util.FindBoolKeysByPrefix(LocationKeyPrefix).Contains(id.ToString());

    private static Dictionary<string, int> GetObtainedItems() =>
        Items.Entries.ToDictionary(entry => entry.shortName, entry => GetItemUnlockCount(entry.clientNameId));

    private static LocationsAccess.RequiredFunctions functions = new()
    {
        cardboard_destroyer = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Cardboard_Destroyer),
        can_clear_tutorial = (obtainedItems, options, functions) => (functions.cardboard_destroyer(obtainedItems, options, functions)
                                                                    && LocationsAccess.has(obtainedItems, "starter_hat")
                                                                    && LocationsAccess.has(obtainedItems, "pot_q"))
                                                                    || (options.TryGetValue("start_with_freeplay", out bool val) && val),
        ranged = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Ranged),
        shield = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Shield),
        sword = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Sword),
        hard = (obtainedItems, options, functions) => options.TryGetValue("harder_ranged_quests", out bool val) && val,
        shield_jump = (obtainedItems, options, functions) => options.TryGetValue("shield_jump", out bool val) && val && HasAny(obtainedItems, Items.ItemGroup.Shield),
    };

    private static void UpdateAccessibleLocations() => AccessibleLocations = LocationsAccess.GetAccessibleLocations(GetObtainedItems(), GetOptions(), functions);
    public static bool IsLocationAccessible(string gatorName)
    {
        if (excludedNPCs.Contains(gatorName) || IsLocationCollected(gatorName))
        {
            return false;
        }
        try
        {
            return AccessibleLocations.Contains(GetLocationApId(gatorName));
        }
        catch (InvalidOperationException)
        {
            Plugin.LogWarn($"Tried to check accessibility of location {gatorName}, which did not correspond to an AP location.");
            return false;
        }
    }

    public static bool IsLocationACheck(string gatorName)
    {
        if (excludedNPCs.Contains(gatorName) || IsLocationCollected(gatorName))
        {
            return false;
        }
        try
        {
            Locations.Entries.First(entry => entry.clientNameId == gatorName);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        return true;
    }

    public static string ConvertDAToGatorName(DialogueActor dialogueActor)
    {
        try
        {
            string gatorName = dialogueActor.profile.id switch
            {
                "NPC_Tut_Dog" => DetectJillVariant(dialogueActor),
                "NPC_Tut_Frog" => DetectAveryVariant(dialogueActor),
                "NPC_Tut_Horse" => DetectMartinVariant(dialogueActor),
                "NPC_Cool_Goose" when dialogueActor.transform.parent.name != "Goose Quest" => "Unhandled Duke",
                "NPC_Cool_Boar" when dialogueActor.transform.parent.name != "Boar Quest" => "Unhandled Jada",
                "NPC_Cool_Wolf" when dialogueActor.transform.parent.name != "Wolf Quest" => "Unhandled Lucas",
                "NPC_Susanne" when dialogueActor.transform.parent.name != "Character" => "Unhandled Susanne",
                "NPC_Gene" when dialogueActor.transform.parent.name != "Character" => "Unhandled Gene",
                "NPC_Antone" when dialogueActor.transform.parent.name != "Character" => "Unhandled Antone",
                "NPC_Theatre_Cowboy" when dialogueActor.transform.parent.name != "Cowfolk" => "Unhandled Velma",
                "NPC_Theatre_Space" when dialogueActor.transform.parent.name != "Space!!!" => "Unhandled Andromeda",
                "NPC_Theatre_Bat" when dialogueActor.transform.parent.name != "Vampire" => "Unhandled Esme",
                string name => name
            };
            return gatorName;
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    private static string DetectMartinVariant(DialogueActor dialogueActor)
    {
        try
        {
            return dialogueActor.transform.parent.name switch
            {
                "Martin Quest" => "Tutorial Martin",
                "Cool CoolKids" => "Cool Kid Martin",
                _ => "Unhandled Martin"
            };
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    private static string DetectAveryVariant(DialogueActor dialogueActor)
    {
        try
        {
            return dialogueActor.transform.parent.name switch
            {
                "Avery Quest" => "Tutorial Avery",
                "Introduction" => "Theatre Introduction Avery",
                "Unhappy" => "Theatre Finale Avery",
                _ => "Unhandled Avery"
            };
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    private static string DetectJillVariant(DialogueActor dialogueActor)
    {
        try
        {
            return dialogueActor.transform.parent.name switch
            {
                "Fantasy" => "Tutorial Stick Jill",
                "Studying" => "Tutorial End Jill",
                "Intro Actors" => "Prep Introduction Jill",
                "Sad Jill" => "Prep Finale Jill",
                _ => "Unhandled Jill"
            };
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    public static bool IsMainQuestAccessible(DialogueActor dialogueActor)
    {
        if (IsMainQuestACheck(dialogueActor))
        {
            string gatorName = ConvertDAToGatorName(dialogueActor);
            string[] checks = ChecksForNPC(gatorName);
            return checks.Select(IsLocationAccessible).Any(b => b);
        }
        return false;
    }

    public static bool IsMainQuestACheck(DialogueActor dialogueActor)
    {
        string gatorName = ConvertDAToGatorName(dialogueActor);
        // filter out non-main quest NPCs from actors pulled to populate the Main Quest list
        string[] filteredNPCs = ["NPC_WannaBeHawk", "NPC_BigSis", "NPC_Destroy_Elephant", "NPC_Chess_Eagle", "NPC_Warrior_Beaver", "NPC_Obstacle_Ostrich",
            "NPC_Theatre_Cow1", "NPC_Theatre_Cow2", "NPC_Theatre_Cow3", "NPC_FetchVulture", "NPC_SurfOpossum", "NPC_TripLizard", "Signs"];
        if (gatorName == "" || gatorName.Contains("Unhandled") || filteredNPCs.Contains(gatorName))
        {
            return false;
        }
        string[] checks = ChecksForNPC(gatorName);
        return checks.Select(IsLocationACheck).Any(b => b);
    }

    public static bool IsNPCACheck(DialogueActor dialogueActor)
    {
        string gatorName = dialogueActor.profile.id;
        if (gatorName == "NPC_BraceletMonkey")
        {
            gatorName = WhichBraceletMonkey(dialogueActor);
        }
        string[] checks = ChecksForNPC(gatorName);
        return checks.Select(IsLocationACheck).Any(b => b);
    }

    public static bool IsNPCAccessible(DialogueActor dialogueActor)
    {
        string gatorName = dialogueActor.profile.id;
        if (gatorName == "NPC_BraceletMonkey")
        {
            gatorName = WhichBraceletMonkey(dialogueActor);
        }
        if (gatorName == "Dialogue Actor No Longer Exists")
        {
            return false;
        }
        string[] checks = ChecksForNPC(gatorName);
        return checks.Select(IsLocationAccessible).Any(b => b);
    }

    private static string WhichBraceletMonkey(DialogueActor dialogueActor)
    {
        try
        {
            if (dialogueActor.transform.parent.parent.name == "North (Mountain)")
            {
                return "BraceletShop3";
            }
            else
            {
                return dialogueActor.transform.parent.parent.parent.name switch
                {
                    "NorthWest (Tutorial Island)" => "BraceletShop2",
                    "East (Creeklands)" => "BraceletShop1",
                    "West (Forest)" => "BraceletShop0",
                    _ => "Unhandled"
                };
            }
        }
        catch (NullReferenceException)
        {
            // When a Bracelet Monkey has been purchased, they need to be excluded from the check
            return "AlreadyPurchased";
        }
    }

    public static bool IsLocationAccessible(PersistentObject gatorObject)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(gatorObject);
        if (persistentObjectType == Util.PersistentObjectType.Pot || persistentObjectType == Util.PersistentObjectType.Chest || persistentObjectType == Util.PersistentObjectType.Race)
        {
            try
            {
                return AccessibleLocations.Contains(GetLocationApId(gatorObject.id));
            }
            catch (InvalidOperationException)
            {
                // Plugin.LogWarn($"Tried to check accessibility of location {gatorID}, which did not correspond to an AP location.");
                return false;
            }
        }
        else if (persistentObjectType == Util.PersistentObjectType.Cardboard)
        {
            return false;
        }
        else
        {
            Plugin.LogWarn($"Tried to check accessibility of location {gatorObject.id}, which is not a Pot, Race, Chest, or a BreakableObject.");
            return false;
        }
    }
    public static bool IsLocationACheck(PersistentObject gatorObject)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(gatorObject);
        if (persistentObjectType == Util.PersistentObjectType.Pot || persistentObjectType == Util.PersistentObjectType.Chest || persistentObjectType == Util.PersistentObjectType.Race)
        {
            return true;
        }
        else if (persistentObjectType == Util.PersistentObjectType.Cardboard)
        {
            return false;
        }
        else
        {
            Plugin.LogWarn($"Tried to check accessibility of location {gatorObject.id}, which is not a Pot, Race, Chest, or a BreakableObject.");
            return false;
        }
    }

    public static string[] ChecksForNPC(string gatorName)
    {
        return gatorName switch
        {
            "NPC_TutIsland_Duck" => ["NPC_TutIsland_Duck", "Sword_Wood"],
            "NPC_TutIsland_Bear" => ["NPC_TutIsland_Bear", "Item_Ragdoll"],
            "NPC_TutIsland_Giraffe" => ["NPC_TutIsland_Giraffe", "Hat_Slime"],
            "NPC_Balloon_Owl" => ["NPC_Balloon_Owl", "Item_Balloon"],
            "NPC_FetchVulture" => ["NPC_FetchVulture", "BROKEN WHEELIE THINGY", "Shield_ScooterBoardBlue"],
            "NPC_SurfOpossum" => ["NPC_SurfOpossum", "Shield_Tube"],
            "NPC_Chess_Eagle" => ["NPC_Chess_Eagle", "Shield_Chessboard"],
            "NPC_Clumsy_Jackal" => ["NPC_Clumsy_Jackal", "Sword_Pencil", "Thrown_Pencil_1", "Thrown_Pencil_2", "Thrown_Pencil_3"],
            "NPC_Old_Man_Sloth" => ["NPC_Old_Man_Sloth", "Item_Gum"],
            "NPC_Skate_Pug" => ["NPC_Skate_Pug", "Shield_Skateboard"],
            "NPC_Rescue_Robin" => ["NPC_Rescue_Robin", "Shield_Palette"],
            "NPC_Fetch_Shark" => ["NPC_Fetch_Shark", "QuestItem_Retainer"],
            "NPC_Destroy_Rabbit" => ["NPC_Destroy_Rabbit", "Sword_Paintbrush"],
            "NPC_Ninja_Crane" => ["NPC_Ninja_Crane", "Item_Shuriken"],
            "NPC_Ninja_Anteater" => ["NPC_Ninja_Anteater", "Sword_Nunchucks", "Hat_Ninja"],
            "NPC_Obstacle_Ostrich" => ["NPC_Obstacle_Ostrich", "Hat_SkateHelmet"],
            "NPC_LunchSwapSkink" => ["NPC_LunchSwapSkink", "Hat_Princess"],
            "NPC_Warrior_Beaver" => ["NPC_Warrior_Beaver", "Shield_TowerShield"],
            "NPC_Shy_Cat" => ["NPC_Shy_Cat", "Item_Camera"],
            "NPC_TutuCat" => ["NPC_TutuCat", "Sword_Wand"],
            "NPC_BombBowlMole" => ["NPC_BombBowlMole", "Item_Bomb"],
            "NPC_BigWhale" => ["NPC_BigWhale", "Hat_Space"],
            "NPC_Rock_Fox" => ["NPC_Rock_Fox", "Rock"],
            "NPC_Mistakes_Rat" => ["NPC_Mistakes_Rat", "Hat_Beret"],
            "NPC_TripLizard" => ["NPC_TripLizard", "Sword_CBSpear"],
            "NPC_TrashPanda" => ["NPC_TrashPanda", "Item_StickyHand", "Shield_Stretch", "Shield_TrashCanLid", "Sword_Wrench", "Item_PaintGun", "Sword_Grabby"],
            "Tutorial Stick Jill" => ["Sword_Stick"],
            "Tutorial End Jill" => ["NPC_TutIsland_Duck", "Sword_Wood"],
            "Prep Introduction Jill" => ["BEACH ROCK", "Sword_RockHammer", "HALF A CHEESE SANDWICH", "Shield_Platter", "Sword_Net", "NPC_Tut_Dog"],
            "Prep Finale Jill" => ["NPC_Tut_Dog"],
            "NPC_Susanne" => ["BEACH ROCK", "Sword_RockHammer"],
            "NPC_Gene" => ["HALF A CHEESE SANDWICH", "Shield_Platter"],
            "NPC_Antone" => ["Sword_Net"],
            "Tutorial Avery" => ["Hat_Basic", "Shirt"],
            "Theatre Introduction Avery" => ["NPC_Tut_Frog", "Item_SpaceGun", "Sword_Laser", "Hat_Western", "ICE CREAM", "Hat_Vampire", "NPC_Part-Timer"],
            "Theatre Finale Avery" => ["NPC_Tut_Frog"],
            "NPC_Theatre_Cowboy" => ["Hat_Western"],
            "NPC_Theatre_Space" => ["Item_SpaceGun", "Sword_Laser"],
            "NPC_Theatre_Bat" => ["ICE CREAM", "Hat_Vampire", "NPC_Part-Timer"],
            "NPC_Part-Timer" => ["ICE CREAM"],
            "Tutorial Martin" => ["POT?", "Shield_PotLid"],
            "Cool Kid Martin" => ["NPC_Tut_Horse", "Shield_Martin", "Hat_DetectiveCowl", "CLIPPINGS", "BUCKET", "WATER", "Shield_Leaf"],
            "NPC_Cool_Goose" => ["Hat_DetectiveCowl"],
            "NPC_Cool_Boar" => ["CLIPPINGS", "BUCKET", "WATER", "Shield_Leaf"],
            "NPC_Cool_Wolf" => ["Shield_Martin"],
            "BraceletShop0" => ["BraceletShop0", "NPC_BraceletMonkey"],
            "BraceletShop1" => ["BraceletShop1", "NPC_BraceletMonkey"],
            "BraceletShop2" => ["BraceletShop2", "NPC_BraceletMonkey"],
            "BraceletShop3" => ["BraceletShop3", "NPC_BraceletMonkey"],
            _ => [gatorName]
        };
    }

    static List<string> ItemsInItemGroup(Items.ItemGroup itemGroup) =>
            Items.Entries
                .Where(entry => entry.itemGroups.Contains(itemGroup))
                .Select(entry => entry.shortName).ToList();

    static bool HasAny(Dictionary<string, int> obtainedItems, Items.ItemGroup itemGroup) =>
        obtainedItems
            .Where(kv => ItemsInItemGroup(itemGroup).Contains(kv.Key))
            .Where(kv => kv.Value > 0)
            .ToArray().Length > 0;

    public static bool IsFullyConnected
    {
        get => Session is not null
                && LoginInfo is not null
                && Session.Socket.Connected
                && Session.Items.Index >= GameData.g.ReadInt("LastAPItemIndex", 0);
    }

    public static void ProcessItemQueue()
    {
        while (ItemQueue.TryDequeue(out Items.Entry entry))
        {
            ReceiveItem(entry);
            UpdateAccessibleLocations();
            var lastIndex = GameData.g.ReadInt("LastAPItemIndex", 0);
            GameData.g.Write("LastAPItemIndex", lastIndex + 1);
        }
    }

    private static bool Connect(string server, int port, string user, string password)
    {
        if (Session is not null && Session.Socket.Connected)
        {
            return false;
        }

        Session = ArchipelagoSessionFactory.CreateSession(server, port);
        LoginResult result;
        try
        {
            //TODO: report more useful fields
            result = Session.TryConnectAndLogin("Lil Gator Game", user, ItemsHandlingFlags.AllItems, new Version(APVersion), null, null, password);
        }
        catch (Exception e)
        {
            result = new LoginFailure(e.GetBaseException().Message);
        }

        if (!result.Successful)
        {
            var failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to {server} as {user}:";
            foreach (string error in failure.Errors)
            {
                errorMessage += $"\n    {error}";
            }
            foreach (ConnectionRefusedError error in failure.ErrorCodes)
            {
                errorMessage += $"\n    {error}";
            }

            Plugin.LogError(errorMessage);
            return false;

        }

        LoginInfo = (LoginSuccessful)result;
        return true;
    }

    public static void Disconnect()
    {
        LocationLookup.Clear();
        ItemQueue = new();
        if (LoginInfo != null)
        {
            Plugin.LogWarn("Disconnected from multiworld");
            Session.Socket.DisconnectAsync();
            LoginInfo = null;
        }
    }

    public static void InitiateNewAPSession(Action postConnectAction)
    {
        Disconnect();

        (string server, int port) = GetServer();
        string user = GetUser();
        string password = GetPassword();
        GetCollectBehavior(); // check user setting for collect behavior
        if (Connect(server, port, user, password))
        {
            // wait until Session is connected & knows about all its items
            Plugin.Instance.StartCoroutine(Util.RunAfterCoroutine(0.5f, () => IsFullyConnected, () =>
            {
                postConnectAction();
                SendLocallySavedLocations(); //send all locations that in the local save but are not in the server's record (in case of disconnection)
                ReceiveUnreceivedItems(); //receive all new items from the AP server
                TriggerItemListeners(); //trigger listener functions for *any* item we have, not just recently received ones
                TriggerLocationListeners(); //trigger listener functions for any location that is collected
                AttachListeners(); //hook up listener functions for future live updates
                ScoutLocations(); //gather information on what items are at locations so that we can show notifications
                UpdateAccessibleLocations();
            }));
        }

        static (string server, int port) GetServer()
        {
            string serverPrefix = "server address:port";
            string serverWithPrefix = Util.FindIntKeyByPrefix(serverPrefix);
            if (serverWithPrefix == "")
            {
                throw new Exception("No server address has been set in the Settings Menu");
            }
            string serverAddressPort = serverWithPrefix.Remove(0, serverPrefix.Length);
            string[] serverComponents = serverAddressPort.Split(':');
            string server = serverComponents[0];
            int port = int.Parse(serverComponents[1]);
            return (server, port);
        }

        static string GetUser()
        {
            return GameData.g.gameSaveData.playerName;
        }

        static string GetPassword()
        {
            string passwordPrefix = "password";
            string passwordWithPrefix = Util.FindIntKeyByPrefix(passwordPrefix);
            if (passwordWithPrefix == "")
            {
                return "";
            }
            else
            {
                return passwordWithPrefix.Remove(0, passwordPrefix.Length);
            }
        }

        static void GetCollectBehavior()
        {
            LocationAutoCollect = Settings.s.ReadBool("!collect counts as checked", true);
        }

        static void SendLocallySavedLocations()
        {
            foreach (long location in Util.FindBoolKeysByPrefix(LocationKeyPrefix).Select(long.Parse))
            {
                if (!Session.Locations.AllLocationsChecked.Contains(location))
                {
                    Plugin.LogDebug("Collecting Saved Location: " + location.ToString());
                    CollectLocationByAPID(location);
                }
            }
        }

        static void ReceiveUnreceivedItems()
        {
            var lastIndex = GameData.g.ReadInt("LastAPItemIndex", 0);
            Plugin.LogDebug($"saved lastindex is {lastIndex}, AP's last index is {Session.Items.Index}");
            if (lastIndex < Session.Items.Index)
            {
                foreach (var item in Session.Items.AllItemsReceived.Skip(lastIndex))
                {
                    ReceiveItem(GetItemEntryByApId(item.ItemId));
                }
            }

            while (Session.Items.Any())
            {
                //Clear the queue of all our initial items
                Session.Items.DequeueItem();
            }

            if (Session.Items.Index >= lastIndex)
            {
                GameData.g.Write("LastAPItemIndex", Session.Items.Index);
            }
            else
            {
                Plugin.LogWarn("Current Item Index from Server is earlier than save file---is connection incorrect?");
            }
        }

        static void TriggerItemListeners()
        {
            foreach (ItemInfo itemInfo in Session.Items.AllItemsReceived)
            {
                Items.Entry item = GetItemEntryByApId(itemInfo.ItemId);
                if (item.clientNameId is not null && SpecialItemFunctions.ContainsKey(item.clientNameId))
                {
                    SpecialItemFunctions[item.clientNameId]();
                }
            }
        }

        static void TriggerLocationListeners()
        {
            IEnumerable<long> locationsCollected;
            if (LocationAutoCollect)
            {
                locationsCollected = Session.Locations.AllLocationsChecked;
            }
            else
            {
                locationsCollected = Util.FindBoolKeysByPrefix("AP ID ").Select(long.Parse);
            }
            foreach (long locationApId in locationsCollected)
            {
                Locations.Entry location = GetLocationEntryByApId(locationApId);
                if (location.clientNameId is not null && SpecialLocationFunctions.ContainsKey(location.clientNameId))
                {
                    SpecialLocationFunctions[location.clientNameId]();
                }
            }
        }

        static void AttachListeners()
        {
            Session.Items.ItemReceived += helper => ItemQueue.Enqueue(GetItemEntryByApId(helper.DequeueItem().ItemId));
        }

        static void ScoutLocations()
        {
            Session.Locations.ScoutLocationsAsync([.. Session.Locations.AllLocations]).ContinueWith(locationInfoPacket =>
            {
                foreach (ItemInfo itemInfo in locationInfoPacket.Result.Values)
                {
                    LocationLookup.Add(itemInfo.LocationId, itemInfo);
                }
            }).Wait(TimeSpan.FromSeconds(5.0f));
            Plugin.LogInfo("Successfully scouted locations for item placements");
        }
    }



    public static ItemInfo ItemAtLocation(int gatorID)
    {
        long apId = GetLocationApId(gatorID);
        return LocationLookup[apId];
        // Fails if invalid gatorID (only use on collected IDs?)
    }

    public static ItemInfo ItemAtLocation(string gatorName)
    {
        long apId = GetLocationApId(gatorName);
        return LocationLookup[apId];
        // Fails if invalid gatorName (only use on collected IDs?)
    }

    private static bool TryGetOptionBool(Option option) {
        try
        {
            object optionReturn = LoginInfo.SlotData[OptionName(option)];
            return optionReturn.ToString() != "0";
        }
        catch (KeyNotFoundException)
        {
            // if game was not generated with an option, assume it is false
            return false;
        }
    }
    public static bool GetOptionBool(Option option) => TryGetOptionBool(option);

    public enum Option
    {
        StartWithFreeplay,
        RequireShieldJump,
        HarderRangedQuests,
        LockPotsBehindItems,
        LockChestsBehindKey,
        LockRacesBehindFlag,
    }

    public static string OptionName(Option option) => option switch
    {
        Option.StartWithFreeplay => "start_with_freeplay",
        Option.RequireShieldJump => "require_shield_jump",
        Option.HarderRangedQuests => "harder_ranged_quests",
        Option.LockPotsBehindItems => "lock_pots_behind_items",
        Option.LockChestsBehindKey => "lock_chests_behind_key",
        Option.LockRacesBehindFlag => "lock_races_behind_flag",
        _ => throw new Exception("Invalid enum value for Option"),
    };

    // TODO: Think about what to do if have non-bool options
    private static Dictionary<string, bool> GetOptions() =>
        Enum.GetValues(typeof(Option)).Cast<Option>().ToDictionary(OptionName, GetOptionBool);

    private static Items.Entry GetItemEntryByApId(long id) => Items.Entries.First(entry => entry.apItemId == id);
    private static Locations.Entry GetLocationEntryByApId(long id) => Locations.Entries.First(entry => entry.apLocationId == id);
    private static void ReceiveItem(Items.Entry entry)
    {
        Plugin.LogDebug($"ReceiveItem for {entry.shortName} (\"{entry.longName}\"). ClientId:{entry.clientNameId}, Type:{entry.clientItemType}, AP:{entry.apItemId}");
        switch (entry.clientItemType)
        {
            case Items.ClientItemType.Item: ItemUtil.GiveItem(entry.clientNameId); break;
            case Items.ClientItemType.Craft: ItemUtil.GiveCraft(entry.clientNameId); break;
            case Items.ClientItemType.Friend: ItemUtil.GiveFriends((int)entry.clientResourceAmount); break;
            case Items.ClientItemType.Craft_Stuff: ItemUtil.GiveCraftStuff((int)entry.clientResourceAmount); break;
            default:
                throw new Exception($"Item {entry.clientNameId} in the item data CSV with an unknown client_item type of {entry.clientItemType}");
        }
        ;

        if (entry.clientNameId is not null && SpecialItemFunctions.ContainsKey(entry.clientNameId))
        {
            SpecialItemFunctions[entry.clientNameId]();
        }
        DialogueModifier.GatorBubble($"Someone sent me {entry.longName}!");
    }

    public static string GetClientIDByAPId(long id) => GetItemEntryByApId(id).clientNameId;

    private static long GetItemApId(string gatorName) =>
        Items.Entries.First(entry => entry.clientNameId == gatorName).apItemId;

    private static long GetLocationApId(int gatorID) =>
        Locations.Entries.First(entry => entry.clientId == gatorID).apLocationId;

    private static long GetLocationApId(string gatorName) =>
        Locations.Entries.First(entry => entry.clientNameId == gatorName).apLocationId;

    private static void CollectLocationByAPID(long id) => Session.Locations.CompleteLocationChecks(id);

    public static long? GetApIdById(int id)
    {
        long apId;
        try
        {
            apId = GetLocationApId(id);
            return apId;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public static bool CollectLocationByID(int id)
    {

        long? apId = GetApIdById(id);
        if (apId is long apIdValue)
        {
            GameData.g.Write(LocationKeyPrefix + apIdValue.ToString(), true);
            CollectLocationByAPID(apIdValue);
            DialogueModifier.AnnounceLocationCollected(id);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool CollectLocationByName(string name)
    {
        long apId;
        try
        {
            apId = GetLocationApId(name);
        }
        catch (InvalidOperationException)
        {
            Plugin.LogWarn($"Tried to collect location with string ID {name}, which does not have an entry in the locations table!");
            return false;
        }

        GameData.g.Write(LocationKeyPrefix + apId.ToString(), true);
        CollectLocationByAPID(apId);
        DialogueModifier.AnnounceLocationCollected(name);
        return true;
    }

    public static bool CollectLocationForNPCs(CharacterProfile[] NPCs)
    {
        foreach (CharacterProfile NPC in NPCs)
        {
            Plugin.LogDebug($"NPC {NPC.id} collected!");
            if (CollectLocationByName(NPC.id))
            {
                Plugin.LogDebug($"{NPC.id} recognized as valid location");
                return true;
            }
        }
        Plugin.LogWarn("No NPCs in the collected location recognized as a valid AP location. Is the spreadsheet missing stuff??");
        return false;
    }

    public static void SendCompletion()
    {
        var statusUpdatePacket = new StatusUpdatePacket
        {
            Status = ArchipelagoClientState.ClientGoal
        };
        Session.Socket.SendPacket(statusUpdatePacket);
    }

    public static void StorePosition(MapManager.PlayerCoords playerCoords)
    {
        if (!IsFullyConnected)
        {
            return;
        }
        Session.DataStorage[$"{Session.ConnectionInfo.Slot}_{Session.ConnectionInfo.Team}_gator_coords"] = JObject.FromObject(playerCoords);
    }
}
