using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using System;
using Archipelago.MultiClient.Net.Packets;
using Newtonsoft.Json.Linq;
using GatorRando.Archipelago;
using Archipelago.MultiClient.Net.Models;

namespace GatorRando;

public static class ArchipelagoManager
{
    public const string APVersion = "0.6.2";
    public static ArchipelagoSession Session;
    public static LoginSuccessful LoginInfo;

    public static bool IsFullyConnected
    {
        get => Session is not null
                && LoginInfo is not null
                && Session.Socket.Connected
                && Session.Items.Index >= GameData.g.ReadInt("LastAPItemIndex", 0);
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
            //TODO: Figure out what past us meant?
            //TODO: report a UUID
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
        LocationHandling.OnDisconnect();
        ItemHandling.OnDisconnect();
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
        if (Connect(server, port, user, password))
        {
            // wait until Session is connected & knows about all its items
            Plugin.Instance.StartCoroutine(Util.RunAfterCoroutine(0.5f, () => IsFullyConnected, () =>
            {
                postConnectAction();
                LocationHandling.SendLocallySavedLocations(); //send all locations that in the local save but are not in the server's record (in case of disconnection)
                ItemHandling.ReceiveUnreceivedItems(); //receive all new items from the AP server
                ItemHandling.TriggerItemListeners(); //trigger listener functions for *any* item we have, not just recently received ones
                LocationHandling.TriggerLocationListeners(); //trigger listener functions for any location that is collected
                AttachListeners(); //hook up listener functions for future live updates
                LocationHandling.ScoutLocations(); //gather information on what items are at locations so that we can show notifications
                LocationAccessibilty.UpdateAccessibleLocations();
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

        static void AttachListeners()
        {
            Session.Items.ItemReceived += helper =>
            {
                ItemInfo item = helper.DequeueItem();
                ItemHandling.EnqueueItem(item.ItemId, item.Player.Alias);
            };
        }
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
