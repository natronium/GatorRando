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
        if (RandoSettingsMenu.IsCollectCountedAsChecked())
        {
            locationsCollected = ConnectionManager.LocationsCollected();
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
    public static bool IsLocationCollected(string location)
    {
        if (RandoSettingsMenu.IsCollectCountedAsChecked())
        {
            try
            {
                return ConnectionManager.LocationsCollected().Contains(GetLocationApId(location));
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

    public static bool IsLocationCollected(int gatorID)
    {
        if (RandoSettingsMenu.IsCollectCountedAsChecked())
        {
            try
            {
                return ConnectionManager.LocationsCollected().Contains(GetLocationApId(gatorID));
            }
            catch (InvalidOperationException)
            {
                return true;
            }
        }
        else
        {
            return CheckIfAPLocationInSave(GetLocationApId(gatorID));
        }
    }

    public static bool CheckIfAPLocationInSave(long id) => Util.FindBoolKeysByPrefix(LocationKeyPrefix).Contains(id.ToString());

    public static long GetLocationApId(int gatorID) =>
        Locations.locationData.First(entry => entry.clientId == gatorID).apLocationId;

    public static long GetLocationApId(string gatorName) =>
        Locations.locationData.First(entry => entry.clientNameId == gatorName).apLocationId;

    private static Locations.Location GetLocationEntryByApId(long id) => Locations.locationData.First(entry => entry.apLocationId == id);

    private static void CheckLocationByApId(long id) => ConnectionManager.CheckLocationByApId(id);

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
            CheckLocationByApId(apIdValue);
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
            CheckLocationByApId(apId);
            AnnounceLocationChecked(name);
        }
        return true;
    }

    public static bool CollectLocationForNPCs(CharacterProfile[] NPCs)
    {
        //TODO: Filter out known excluded NPCs like NPC_Theatre_Cow1
        bool excludedOrFiltered = false;
        foreach (CharacterProfile NPC in NPCs)
        {
            // if (!LocationAccessibilty.IsNPCinExcludedOrFiltered(NPC.id))
            // {
            Plugin.LogDebug($"NPC {NPC.id} collected!");
            if (CollectLocationByName(NPC.id))
            {
                Plugin.LogDebug($"{NPC.id} recognized as valid location");
                return true;
            }
            // }
            // else
            // {
            //     excludedOrFiltered = true;
            // }
        }
        if (!excludedOrFiltered)
        {
            Plugin.LogWarn("No NPCs in the collected location recognized as a valid AP location. Did the DLC release? :P");
        }
        return false;
    }

    public static void SendLocallySavedLocations()
    {
        foreach (long location in Util.FindBoolKeysByPrefix(LocationKeyPrefix).Select(long.Parse))
        {
            if (!ConnectionManager.LocationsCollected().Contains(location))
            {
                Plugin.LogDebug("Collecting Saved Location: " + location.ToString());
                CheckLocationByApId(location);
            }
        }
    }

    public static void OnDisconnect()
    {
        LocationLookup.Clear();
    }

    public static int ConvertTannerIds(int objectId)
    {
        // Convert initial Tanner pot ids to their post intro counterparts
        return objectId switch
        {
            1663 => 1712,
            1638 => 1709,
            1606 => 1695,
            _ => objectId,
        };
    }
}