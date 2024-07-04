using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine;
using Data;
using System;
using System.Linq;
using System.Collections.Concurrent;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.MultiClient.Net.Models;

namespace GatorRando;

public class ArchipelagoManager : MonoBehaviour
{
    public static ArchipelagoSession Session;
    public static LoginSuccessful LoginInfo;
    public static ArchipelagoManager Instance;
    private static ConcurrentQueue<Items.Entry> ItemQueue = new();
    private static readonly Dictionary<long, ItemInfo> LocationLookup = [];
    private static readonly Dictionary<string, Action> SpecialItemFunctions = [];
    public static void RegisterItemListener(string itemName, Action listener) => SpecialItemFunctions[itemName] = listener;

    public static bool ItemIsUnlocked(string item) =>
        Session.Items.AllItemsReceived.Select(info => info.ItemId).Contains(GetItemApId(item));

    public static bool LocationIsCollected(string location) =>
        Session.Locations.AllLocationsChecked.Contains(GetLocationApId(location));

    public static int GetItemUnlockCount(string itemName) =>
        Session.Items.AllItemsReceived.Where(itemInfo => itemInfo.ItemId == GetItemApId(itemName)).Count();

    public void Awake()
    {
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            Plugin.LogWarn("ArchipelagoManager Instance already exists! Self destructing duplicate!");
            Destroy(this);
        }
    }

    public void Update()
    {
        while (ItemQueue.TryDequeue(out Items.Entry entry))
        {
            ReceiveItem(entry);
            var lastIndex = GameData.g.ReadInt("LastAPItemIndex", 0);
            GameData.g.Write("LastAPItemIndex", lastIndex + 1);
        }
    }

    public static void Connect()
    {
        if (Session is not null && Session.Socket.Connected)
        {
            return;
        }
        string serverPrefix = "server address";
        string serverWithPrefix = Util.FindKeyByPrefix(serverPrefix);
        if (serverWithPrefix == "")
        {
            throw new Exception("No server address has been set in the Settings Menu");
        }
        string server = serverWithPrefix.Remove(0, serverPrefix.Length);
        string user = GameData.g.gameSaveData.playerName;
        int port = GameData.g.ReadInt("server port");

        Session = ArchipelagoSessionFactory.CreateSession(server, port);
        LoginResult result;
        try
        {
            result = Session.TryConnectAndLogin("Lil Gator Game", user, ItemsHandlingFlags.AllItems);
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
            return;

        }

        LoginInfo = (LoginSuccessful)result;
        //TODO: Likely move this code somewhere more appropriate
        if (result.Successful)
        {
            Plugin.Setup();
            GameObject backButton = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Back");
            backButton.SetActive(true);
            GameObject backToTitle = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Back To Title");
            backToTitle.SetActive(false);
            ScoutLocations();
        }

    }

    public static bool IsConnected()
    {
        return LoginInfo != null && LoginInfo.Successful;
    }

    public static void Disconnect()
    {
        if (IsConnected())
        {
            Plugin.LogWarn("Disconnected from multiworld");
            LocationLookup.Clear();
            ItemQueue = new();
            Session.Socket.DisconnectAsync();
            LoginInfo = null;
        }
    }

    public void Destroy()
    {
        if (Instance == this)
        {
            Disconnect();
        }
    }

    private static void ScoutLocations()
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

    public static ItemInfo ItemAtLocation(int gatorID)
    {
        int ap_id = GetLocationApId(gatorID);
        return LocationLookup[ap_id];
        // Fails if invalid gatorID (only use on collected IDs?)
    }

    public static ItemInfo ItemAtLocation(string gatorName)
    {
        int ap_id = GetLocationApId(gatorName);
        return LocationLookup[ap_id];
        // Fails if invalid gatorName (only use on collected IDs?)
    }

    public static string Options(string option_name)
    {
        return LoginInfo.SlotData[option_name].ToString();
    }

    public static void OnSceneLoad()
    {
        var lastIndex = GameData.g.ReadInt("LastAPItemIndex", 0);
        Plugin.LogDebug($"saved lastindex is {lastIndex}, AP's last index is {Session.Items.Index}");
        if (lastIndex < Session.Items.Index)
        {
            foreach (var item in Session.Items.AllItemsReceived.Skip(lastIndex))
            {
                ReceiveItem(GetEntryByApId(item.ItemId));
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
        Session.Items.ItemReceived += helper => ItemQueue.Enqueue(GetEntryByApId(helper.DequeueItem().ItemId));
    }

    private static Items.Entry GetEntryByApId(long id) => Items.Entries.First(entry => entry.ap_item_id == id);
    private static void ReceiveItem(Items.Entry entry)
    {
        Plugin.LogDebug($"ReceiveItem for {entry.shortname} (\"{entry.longname}\"). ClientId:{entry.client_name_id}, Type:{entry.client_item_type}, AP:{entry.ap_item_id}");
        switch (entry.client_item_type)
        {
            case "Item": ItemUtil.GiveItem(entry.client_name_id); break;
            case "Craft": ItemUtil.GiveCraft(entry.client_name_id); break;
            case "Friends": ItemUtil.GiveFriends((int)entry.client_resource_amount); break;
            case "Craft Stuff": ItemUtil.GiveCraftStuff((int)entry.client_resource_amount); break;
            default:
                throw new Exception($"Item {entry.client_name_id} in the item data CSV with an unknown client_item type of {entry.client_item_type}");
        };

        if (entry.client_name_id is not null && SpecialItemFunctions.ContainsKey(entry.client_name_id))
        {
            SpecialItemFunctions[entry.client_name_id]();
        }
    }

    private static int GetItemApId(string gatorName) =>
        (int)Items.Entries.First(entry => entry.client_name_id == gatorName).ap_item_id;

    private static int GetLocationApId(int gatorID) =>
        (int)Locations.Entries.First(entry => entry.client_id == gatorID).ap_location_id;

    private static int GetLocationApId(string gatorName) =>
        (int)Locations.Entries.First(entry => entry.client_name_id == gatorName).ap_location_id;

    private static void CollectLocationByAPID(int id) => Session.Locations.CompleteLocationChecks(id);


    public static bool CollectLocationByID(int id)
    {
        int ap_id;
        try
        {
            ap_id = GetLocationApId(id);
        }
        catch (InvalidOperationException)
        {
            //NB: This logs at Info level because it gets hit for *allll* the breakables we don't currently track
            //      perhaps in the future if we're tracking *everything* it might make sense for this to be a warning
            //      but not right now.
            Plugin.LogInfo($"Tried to collect location with numeric ID {id}, which does not have an entry in the locations table!");
            return false;
        }

        CollectLocationByAPID(ap_id);
        return true;
    }

    public static bool CollectLocationByName(string name)
    {
        int ap_id;
        try
        {
            ap_id = GetLocationApId(name);
        }
        catch (InvalidOperationException)
        {
            Plugin.LogWarn($"Tried to collect location with string ID {name}, which does not have an entry in the locations table!");
            return false;
        }

        CollectLocationByAPID(ap_id);
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
        var statusUpdatePacket = new StatusUpdatePacket();
        statusUpdatePacket.Status = ArchipelagoClientState.ClientGoal;
        Session.Socket.SendPacket(statusUpdatePacket);
    }
}
