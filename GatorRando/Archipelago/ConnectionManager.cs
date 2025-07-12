using System;
using System.Linq;
using System.Threading;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Models;
using Archipelago.MultiClient.Net.Packets;

namespace GatorRando.Archipelago;

public static class ConnectionManager
{
    public const string APVersion = "0.6.2";
    private const string Game = "Lil Gator Game";

    public static bool Authenticated;
    private static bool attemptingConnection;

    public static ArchipelagoData ServerData = new();
    private static ArchipelagoSession session;

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

    //TODO: Save and Load existing server data



    /// <summary>
    /// call to connect to an Archipelago session. Connection info should already be set up on ServerData
    /// </summary>
    /// <returns></returns>
    public static void Connect()
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
        }

        TryConnect();
    }

    /// <summary>
    /// add handlers for Archipelago events
    /// </summary>
    private static void SetupSession()
    {
        // session.MessageLog.OnMessageReceived += message => ArchipelagoConsole.LogMessage(message.ToString());
        session.Items.ItemReceived += OnItemReceived;
        session.Socket.ErrorReceived += OnSessionErrorReceived;
        session.Socket.SocketClosed += OnSessionSocketClosed;
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
                        requestSlotData: ServerData.NeedSlotData
                    )));
        }
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

            session.Locations.CompleteLocationChecksAsync([.. ServerData.CheckedLocations]);
            outText = $"Successfully connected to {ServerData.Uri} as {ServerData.SlotName}!";
            // UIMods.MainMenuMod.PostConnect(); // Enable entering the game

            // ArchipelagoConsole.LogMessage(outText);
        }
        else
        {
            var failure = (LoginFailure)result;
            outText = $"Failed to connect to {ServerData.Uri} as {ServerData.SlotName}.";
            outText = failure.Errors.Aggregate(outText, (current, error) => current + $"\n    {error}");

            Plugin.LogError(outText);

            Authenticated = false;
            Disconnect();
        }

        // ArchipelagoConsole.LogMessage(outText);
        attemptingConnection = false;
    }

    /// <summary>
    /// something we wrong or we need to properly disconnect from the server. cleanup and re null our session
    /// </summary>
    private static void Disconnect()
    {
        Plugin.LogDebug("disconnecting from server...");
        session?.Socket.DisconnectAsync();
        session = null;
        Authenticated = false;
        // UIMods.MainMenuMod.Disconnect();
    }

    public static void SendMessage(string message)
    {
        session.Socket.SendPacketAsync(new SayPacket { Text = message });
    }

    /// <summary>
    /// we received an item so reward it here
    /// </summary>
    /// <param name="helper">item helper which we can grab our item from</param>
    private static void OnItemReceived(ReceivedItemsHelper helper)
    {
        ItemInfo receivedItem = helper.DequeueItem();

        if (helper.Index < ServerData.Index) return; // TODO: Intercept prior items here to populate ItemManager and ActManager HashSets? Or load the client data and use that to populate

        // ItemManager.ItemQueue.Enqueue(receivedItem.ItemId);
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
        // ArchipelagoConsole.LogMessage(message);
    }

    /// <summary>
    /// something went wrong closing our connection. disconnect and clean up
    /// </summary>
    /// <param name="reason"></param>
    private static void OnSessionSocketClosed(string reason)
    {
        Plugin.LogError($"Connection to Archipelago lost: {reason}");
        Disconnect();
    }

    public static void SendGoal()
    {
        session.SetGoalAchieved();
    }

    public static void HintLocation(long apId)
    {
        session.Locations.ScoutLocationsAsync(HintCreationPolicy.CreateAndAnnounceOnce, [apId]);
    }


    // Connect on title screen
    // Handle and display errors
    // --Wrong slot name
    // --room not open
    // --mismatched index

    // Display in game that have become disconnected from server
    // 

}