using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Data;
using System;
using System.Linq;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.MultiClient.Net.Models;

namespace GatorRando;

public static class ArchipelagoManager
{
    public static ArchipelagoSession Session;
    public static LoginSuccessful LoginInfo;
    private static ConcurrentQueue<Items.Entry> ItemQueue = new();
    private static readonly Dictionary<long, ItemInfo> LocationLookup = [];
    private static readonly Dictionary<string, Action> SpecialItemFunctions = [];
    private static readonly Dictionary<string, Action> SpecialLocationFunctions = [];
    public static bool LocationAutoCollect = true;
    private static readonly string LocationKeyPrefix = "AP ID: ";
    private static List<long> AccessibleLocations;
    private static readonly string[] excludedNPCs = ["NPC_LunchSwapCardinal", "NPC_Bee", "NPC_Ninja_Tiger", "NPC_SwimSheep", "NPC_Ninja_Tiger"];
    public static void RegisterItemListener(string itemName, Action listener) => SpecialItemFunctions[itemName] = listener;
    public static void RegisterLocationListener(string locationName, Action listener) => SpecialLocationFunctions[locationName] = listener;

    public static bool ItemIsUnlocked(string item) =>
        Session.Items.AllItemsReceived.Select(info => info.ItemId).Contains(GetItemApId(item));

    public static bool LocationIsCollected(string location)
    {
        if (LocationAutoCollect)
        {
            return Session.Locations.AllLocationsChecked.Contains(GetLocationApId(location));
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
                                                                    || (options.TryGetValue("start_with_freeplay", out bool val) ? val : false),
        ranged = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Ranged),
        shield = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Shield),
        sword = (obtainedItems, options, functions) => HasAny(obtainedItems, Items.ItemGroup.Sword),
        hard = (obtainedItems, options, functions) => options.TryGetValue("harder_ranged_quests", out bool val) && val,
        shield_jump = (obtainedItems, options, functions) => options.TryGetValue("shield_jump", out bool val) && val && HasAny(obtainedItems, Items.ItemGroup.Shield),
    };

    private static void UpdateAccessibleLocations() => AccessibleLocations = LocationsAccess.GetAccessibleLocations(GetObtainedItems(), GetOptions(), functions);
    public static bool IsLocationAccessible(string gatorName)
    {
        if (excludedNPCs.Contains(gatorName))
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
        if (excludedNPCs.Contains(gatorName))
        {
            return false;
        }
        return true;
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
            result = Session.TryConnectAndLogin("Lil Gator Game", user, ItemsHandlingFlags.AllItems, null, null, null, password);
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
        long ap_id = GetLocationApId(gatorID);
        return LocationLookup[ap_id];
        // Fails if invalid gatorID (only use on collected IDs?)
    }

    public static ItemInfo ItemAtLocation(string gatorName)
    {
        long ap_id = GetLocationApId(gatorName);
        return LocationLookup[ap_id];
        // Fails if invalid gatorName (only use on collected IDs?)
    }

    public static bool GetOption(Option option) => option switch
    {
        // For now, all options are boolean, so we have to convert 0 or 1 to a boolean value
        Option.StartWithFreeplay => LoginInfo.SlotData[OptionName(option)].ToString() != "0",
        Option.RequireShieldJump => LoginInfo.SlotData[OptionName(option)].ToString() != "0",
        Option.HarderRangedQuests => LoginInfo.SlotData[OptionName(option)].ToString() != "0",
        _ => throw new Exception("Invalid enum value for Option"),
    };

    public enum Option
    {
        StartWithFreeplay,
        RequireShieldJump,
        HarderRangedQuests
    }

    public static string OptionName(Option option) => option switch
    {
        Option.StartWithFreeplay => "start_with_freeplay",
        Option.RequireShieldJump => "require_shield_jump",
        Option.HarderRangedQuests => "harder_ranged_quests",
        _ => throw new Exception("Invalid enum value for Option"),
    };

    private static Dictionary<string, bool> GetOptions() =>
        Enum.GetValues(typeof(Option)).Cast<Option>().ToDictionary(OptionName, GetOption);

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
        };

        if (entry.clientNameId is not null && SpecialItemFunctions.ContainsKey(entry.clientNameId))
        {
            SpecialItemFunctions[entry.clientNameId]();
        }
    }

    public static string GetClientIDByAPId(long id)
    {
        Items.Entry itemEntry = ArchipelagoManager.GetItemEntryByApId(id);
        return itemEntry.clientNameId;
    }

    private static long GetItemApId(string gatorName) =>
        Items.Entries.First(entry => entry.clientNameId == gatorName).apItemId;

    private static long GetLocationApId(int gatorID) =>
        Locations.Entries.First(entry => entry.clientId == gatorID).apLocationId;

    private static long GetLocationApId(string gatorName) =>
        Locations.Entries.First(entry => entry.clientNameId == gatorName).apLocationId;

    private static void CollectLocationByAPID(long id) => Session.Locations.CompleteLocationChecks(id);


    public static bool CollectLocationByID(int id)
    {
        long apId;
        try
        {
            apId = GetLocationApId(id);
        }
        catch (InvalidOperationException)
        {
            //NB: This logs at Info level because it gets hit for *allll* the breakables we don't currently track
            //      perhaps in the future if we're tracking *everything* it might make sense for this to be a warning
            //      but not right now.
            Plugin.LogInfo($"Tried to collect location with numeric ID {id}, which does not have an entry in the locations table!");
            return false;
        }

        GameData.g.Write(LocationKeyPrefix + apId.ToString(), true);
        CollectLocationByAPID(apId);
        return true;
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
}
