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

    public static string GetOption(string option_name)
    {
        return LoginInfo.SlotData[option_name].ToString();
    }

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

    public static string GetClientIDByAPId(long id) => GetItemEntryByApId(id).clientNameId;

    private static int GetItemApId(string gatorName) =>
        (int)Items.Entries.First(entry => entry.clientNameId == gatorName).apItemId;

    private static long GetLocationApId(int gatorID) =>
        (long)Locations.Entries.First(entry => entry.clientId == gatorID).apLocationId;

    private static long GetLocationApId(string gatorName) =>
        (long)Locations.Entries.First(entry => entry.clientNameId == gatorName).apLocationId;

    private static void CollectLocationByAPID(long id) => Session.Locations.CompleteLocationChecks(id);


    public static bool CollectLocationByID(int id)
    {
        long apId;
        try
        {
            apId = GetLocationApId(id);
            Plugin.LogInfo($"Tried to collect location with numeric ID {id}, which does have an entry in the locations table!");
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
