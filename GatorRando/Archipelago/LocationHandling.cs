using System;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using GatorRando.Data;
using GatorRando.UIMods;

namespace GatorRando.Archipelago;

public static class LocationHandling
{
    public readonly struct ItemAtLocation(string itemName, long itemId, string itemPlayer, string itemGame)
    {
        public readonly string itemName = itemName;
        public readonly long itemId = itemId;
        public readonly string itemPlayer = itemPlayer;
        public readonly string itemGame = itemGame;
    }

    private static readonly Dictionary<long, ItemInfo> LocationLookup = [];
    private static readonly Dictionary<string, Action> SpecialLocationFunctions = [];
    public static void RegisterLocationListener(string locationName, Action listener) => SpecialLocationFunctions[locationName] = listener;
    public static void TriggerLocationListeners()
    {
        IEnumerable<long> locationsCollected;
        if (GetCollectBehavior())
        {
            locationsCollected = ArchipelagoManager.Session.Locations.AllLocationsChecked;
        }
        else
        {
            locationsCollected = Util.FindBoolKeysByPrefix("AP ID ").Select(long.Parse);
        }
        foreach (long locationApId in locationsCollected)
        {
            Locations.Location location = GetLocationEntryByApId(locationApId);
            if (location.clientNameId is not null && SpecialLocationFunctions.ContainsKey(location.clientNameId))
            {
                SpecialLocationFunctions[location.clientNameId]();
            }
        }
    }

    private static readonly string LocationKeyPrefix = "AP ID: ";

    private static bool GetCollectBehavior()
    {
        return Settings.s.ReadBool("!collect counts as checked", true);
    }
    public static bool IsLocationCollected(string location)
    {
        if (GetCollectBehavior())
        {
            try
            {
                return ArchipelagoManager.Session.Locations.AllLocationsChecked.Contains(GetLocationApId(location));
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
    public static bool CheckIfAPLocationInSave(long id) => Util.FindBoolKeysByPrefix(LocationKeyPrefix).Contains(id.ToString());

    public static long GetLocationApId(int gatorID) =>
        Locations.locationData.First(entry => entry.clientId == gatorID).apLocationId;

    public static long GetLocationApId(string gatorName) =>
        Locations.locationData.First(entry => entry.clientNameId == gatorName).apLocationId;

    private static Locations.Location GetLocationEntryByApId(long id) => Locations.locationData.First(entry => entry.apLocationId == id);

    private static void CollectLocationByAPID(long id) => ArchipelagoManager.Session.Locations.CompleteLocationChecks(id);

    public static long? GetLocationApIdById(int id)
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


    public static ItemAtLocation GetItemAtLocation(int gatorID)
    {
        long apId = GetLocationApId(gatorID);
        return ConnectionManager.ServerData.LocationLookup[apId];
        // Fails if invalid gatorID (only use on collected IDs)
    }

    public static ItemAtLocation GetItemAtLocation(string gatorName)
    {
        long apId = GetLocationApId(gatorName);
        return ConnectionManager.ServerData.LocationLookup[apId];
        // Fails if invalid gatorName (only use on collected IDs?)
    }

    private static void AnnounceLocationChecked(int gatorID)
    {
        // Plugin.LogDebug($"Announcing id {gatorID}");
        ItemAtLocation itemAtLocation = GetItemAtLocation(gatorID);
        BubbleManager.QueueBubble(DialogueModifier.GetDialogueStringForItemAtLocation(itemAtLocation), BubbleManager.BubbleType.LocationChecked);
    }
    private static void AnnounceLocationChecked(string gatorName)
    {
        // Plugin.LogDebug($"Announcing gatorName {gatorName}");
        ItemAtLocation itemAtLocation = GetItemAtLocation(gatorName);
        BubbleManager.QueueBubble(DialogueModifier.GetDialogueStringForItemAtLocation(itemAtLocation), BubbleManager.BubbleType.LocationChecked);
    }


    public static bool CollectLocationByID(int id)
    {

        long? apId = GetLocationApIdById(id);
        if (apId is long apIdValue)
        {
            GameData.g.Write(LocationKeyPrefix + apIdValue.ToString(), true);
            CollectLocationByAPID(apIdValue);
            AnnounceLocationChecked(id);
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
        if (!IsLocationCollected(name))
        {
            CollectLocationByAPID(apId);
            AnnounceLocationChecked(name);
        }
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
        Plugin.LogWarn("No NPCs in the collected location recognized as a valid AP location. Did the DLC release? :P");
        return false;
    }

    public static void SendLocallySavedLocations()
    {
        foreach (long location in Util.FindBoolKeysByPrefix(LocationKeyPrefix).Select(long.Parse))
        {
            if (!ArchipelagoManager.Session.Locations.AllLocationsChecked.Contains(location))
            {
                Plugin.LogDebug("Collecting Saved Location: " + location.ToString());
                CollectLocationByAPID(location);
            }
        }
    }

    // TODO: Cache the scout locations in the save file
    public static void ScoutLocations()
    {
        ArchipelagoManager.Session.Locations.ScoutLocationsAsync([.. ArchipelagoManager.Session.Locations.AllLocations]).ContinueWith(locationInfoPacket =>
        {
            foreach (ItemInfo itemInfo in locationInfoPacket.Result.Values)
            {
                LocationLookup.Add(itemInfo.LocationId, itemInfo);
            }
        }).Wait(TimeSpan.FromSeconds(5.0f));
        Plugin.LogInfo("Successfully scouted locations for item placements");
    }

    public static void OnDisconnect()
    {
        LocationLookup.Clear();
    }
}