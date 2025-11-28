using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;

namespace GatorRando.Archipelago;

public static class ConnectionManager
{
    public const string APVersion = "0.6.2";
    private const string Game = "Lil Gator Game";

    public static bool Authenticated;
    private static bool attemptingConnection;

    public static ArchipelagoData ServerData = new();
    private static ArchipelagoSession session;

    internal static ArchipelagoSession Session => session;

    public static string Seed()
    {
        if (Authenticated)
        {
            return session.RoomState.Seed;
        }
        return "Not Authenticated";
    }

    public static string SlotName()
    {
        if (Authenticated)
        {
            return ServerData.SlotName;
        }
        return "Not Authenticated";
    }

    public static string GetSlotDataOption(string optionName)
    {
        return ServerData.GetSlotDataOption(optionName);
    }

    //TODO: Save and Load existing server data

    public static void InitiateNewAPSession()
    {
        ServerData = new();
        SaveManager.ReadLastConnectionData();
        Connect();
    }


    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    private static void Connect()
    {
        if (Authenticated || attemptingConnection) return;

        try
        {
            session = ArchipelagoSessionFactory.CreateSession(ServerData.Uri);
            SetupSession();
        }
        catch (Exception e)
        {
            Plugin.LogError(e.ToString());
            StateManager.FailedConnection(e.ToString());
        }

        TryConnect();
    }

    /// <summary>
    /// add handlers for Archipelago events
    /// </summary>
    private static void SetupSession()
    {
        session.MessageLog.OnMessageReceived += message => ArchipelagoConsole.LogMessage(message.ToString());
        
        session.Socket.ErrorReceived += OnSessionErrorReceived;
        session.Socket.SocketClosed += OnSessionSocketClosed;
    }

    public static void RegisterItemReceivedListener()
    {
        session.Items.ItemReceived += OnItemReceived;
    }

    public static void UnregisterItemReceivedListener()
    {
        session.Items.ItemReceived -= OnItemReceived;
    }

    /// <summary>
    /// attempt to connect to the server with our connection info
    /// </summary>
    private static void TryConnect()
    {
        attemptingConnection = true;
        try
        {
            // it's safe to thread this function call but unity notoriously hates threading so do not use excessively
            // TODO: UUID, Tags?
            ThreadPool.QueueUserWorkItem(
                _ => HandleConnectResult(
                    session.TryConnectAndLogin(
                        Game,
                        ServerData.SlotName,
                        ItemsHandlingFlags.AllItems,
                        version: new Version(APVersion),
                        password: ServerData.Password,
                        requestSlotData: true
                    )));
        }
        //TODO: Figure out how to cache slotdata
        catch (Exception e)
        {
            Plugin.LogError(e.ToString());
            HandleConnectResult(new LoginFailure(e.ToString()));
            attemptingConnection = false;
        }
    }

    /// <summary>
    /// handle the connection result and do things
    /// </summary>
    /// <param name="result"></param>
    private static void HandleConnectResult(LoginResult result)
    {
        string outText;
        if (result.Successful)
        {
            var success = (LoginSuccessful)result;

            ServerData.SetupSession(success.SlotData, session.RoomState.Seed);
            Authenticated = true;

            outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";
            StateManager.SucceededConnection();
            // ArchipelagoConsole.LogMessage(outText);
        }
        else
        {
            var failure = (LoginFailure)result;
            outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
            outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

            Plugin.LogError(outText);
            StateManager.FailedConnection(outText);

            Authenticated = false;
            Disconnect();
        }

        // ArchipelagoConsole.LogMessage(outText);
        attemptingConnection = false;
    }

    /// <summary>
    /// something we wrong or we need to properly disconnect from the server. cleanup and re null our session
    /// </summary>
    public static void Disconnect()
    {
        Plugin.LogDebug("disconnecting from server...");
        session?.Socket.DisconnectAsync();
        session = null;
        Authenticated = false;
    }

    public static void SendMessage(string message)
    {
        session.Socket.SendPacketAsync(new SayPacket { Text = message });
    }

    public static void ReceiveUnreceivedItems()
    {
        Plugin.LogDebug($"saved lastindex is {ServerData.GetIndex()}, AP's last index is {ItemsReceived().Count}");
        if (ServerData.GetIndex() < ItemsReceived().Count)
        {
            foreach (ItemInfo item in ItemsReceived().Skip(ServerData.GetIndex()))
            {
                ItemHandling.EnqueueItem(item.ItemId, item.Player.Name);
            }
        }

        ClearItemQueue();
        ServerData.Index = ItemsReceived().Count;
    }

    /// <summary>
    /// we received an item so reward it here
    /// </summary>
    /// <param name="helper">item helper which we can grab our item from</param>
    private static void OnItemReceived(ReceivedItemsHelper helper)
    {
        ItemInfo receivedItem = helper.DequeueItem();

        Plugin.LogDebug(ServerData.GetIndex().ToString());

        if (helper.Index < ServerData.GetIndex()) return;

        ServerData.Index = helper.Index;

        ItemHandling.EnqueueItem(receivedItem.ItemId, receivedItem.Player.Name);
    }

    public static void ClearItemQueue()
    {
        while (session.Items.Any())
        {
            //Clear the queue of all our initial items
            session.Items.DequeueItem();
        }
    }


    // public static void CheckLocationByApId(long apId) => session.Locations.CompleteLocationChecks(apId);
    public static void CheckLocationByApId(long apId)
    {
        session.Locations.CompleteLocationChecks(apId);
        Plugin.LogDebug($"Checking {apId}");
    }
    public static bool IsLocationCollected(long apId) => session.Locations.AllLocationsChecked.Any(i => i == apId);

    /// <summary>
    /// something went wrong with our socket connection
    /// </summary>
    /// <param name="e">thrown exception from our socket</param>
    /// <param name="message">message received from the server</param>
    private static void OnSessionErrorReceived(Exception e, string message)
    {
        Plugin.LogError(e.ToString());
        StateManager.DisplayError(message);
        StateManager.Disconnect();
    }

    /// <summary>
    /// something went wrong closing our connection. disconnect and clean up
    /// </summary>
    /// <param name="reason"></param>
    private static void OnSessionSocketClosed(string reason)
    {
        if (reason != "")
        {
            Plugin.LogError($"Connection to Archipelago lost: {reason}");
            StateManager.DisplayError($"Connection to Archipelago lost: {reason}");
            Disconnect();
        }
    }

    public static void SendGoal()
    {
        session.SetGoalAchieved();
    }

    public static void HintLocation(long apId)
    {
        session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, [apId]);
    }

    public static Dictionary<long, LocationHandling.ItemAtLocation> ScoutLocations()
    {
        Dictionary<long, LocationHandling.ItemAtLocation> locationLookup = [];
        session.Locations.ScoutLocationsAsync([.. session.Locations.AllLocations]).ContinueWith(locationInfoPacket =>
        {
            foreach (ItemInfo itemInfo in locationInfoPacket.Result.Values)
            {
                locationLookup.Add(itemInfo.LocationId, new LocationHandling.ItemAtLocation(itemInfo.ItemName, itemInfo.ItemId, itemInfo.Player.Name, itemInfo.Player.Game));
            }
        }).Wait(TimeSpan.FromSeconds(5.0f));
        Plugin.LogInfo("Successfully scouted locations for item placements");

        return locationLookup;
    }

    public static ReadOnlyCollection<ItemInfo> ItemsReceived()
    {
        return session.Items.AllItemsReceived;
    }

    public static ReadOnlyCollection<long> LocationsCollected()
    {
        return session.Locations.AllLocationsChecked;
    }

    public static void StorePosition(MapManager.PlayerCoords playerCoords)
    {
        if (!Authenticated)
        {
            return;
        }
        session.DataStorage[$"{session.ConnectionInfo.Slot}_{session.ConnectionInfo.Team}_gator_coords"] = JObject.FromObject(playerCoords);
    }

    public static void SendLocallyCheckedLocations() =>
        session.Locations.CompleteLocationChecksAsync([.. ServerData.CheckedLocations]);

    // Connect on title screen
    // Handle and display errors
    // --Wrong slot name
    // --room not open
    // --mismatched index

    // Display in game that have become disconnected from server
    // 

}