using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net.Models;
using Newtonsoft.Json;

namespace GatorRando.Archipelago;

public class ArchipelagoData
{
    public string Uri;
    public string SlotName;
    public string Password;
    public int Index;

    public List<long> CheckedLocations;
    public Dictionary<long, LocationHandling.ItemAtLocation> LocationLookup = [];

    /// <summary>
    /// seed for this archipelago data. Can be used when loading a file to verify the session the player is trying to
    /// load is valid to the room it's connecting to.
    /// </summary>
    private string seed;

    private Dictionary<string, object> slotData;

    public bool NeedSlotData => slotData == null;

    public ArchipelagoData()
    {
        Uri = "localhost";
        SlotName = "Player1";
        CheckedLocations = [];
    }

    public ArchipelagoData(string uri, string slotName, string password)
    {
        Uri = uri;
        SlotName = slotName;
        Password = password;
        CheckedLocations = [];
        LocationLookup = [];
    }

    public ArchipelagoData(string uri, string slotName, string password, List<long> checkedLocations, Dictionary<long, LocationHandling.ItemAtLocation> locationLookup)
    {
        Uri = uri;
        SlotName = slotName;
        Password = password;
        CheckedLocations = checkedLocations;
        LocationLookup = locationLookup;
    }



    /// <summary>
    /// assigns the slot data and seed to our data handler. any necessary setup using this data can be done here.
    /// </summary>
    /// <param name="roomSlotData">slot data of your slot from the room</param>
    /// <param name="roomSeed">seed name of this session</param>
    public void SetupSession(Dictionary<string, object> roomSlotData, string roomSeed)
    {
        slotData = roomSlotData;
        seed = roomSeed;
        if (LocationLookup.Count == 0)
        {
            LocationLookup = ScoutLocations();
        }
    }

    private Dictionary<long, LocationHandling.ItemAtLocation> ScoutLocations()
    {
        Dictionary<long, LocationHandling.ItemAtLocation> locationLookup = [];
        ArchipelagoManager.Session.Locations.ScoutLocationsAsync([.. ArchipelagoManager.Session.Locations.AllLocations]).ContinueWith(locationInfoPacket =>
        {
            foreach (ItemInfo itemInfo in locationInfoPacket.Result.Values)
            {
                locationLookup.Add(itemInfo.LocationId, new LocationHandling.ItemAtLocation(itemInfo.ItemName, itemInfo.ItemId, itemInfo.Player.Name, itemInfo.Player.Game));
            }
        }).Wait(TimeSpan.FromSeconds(5.0f));
        Plugin.LogInfo("Successfully scouted locations for item placements");

        return locationLookup;
    }

    /// <summary>
    /// returns the object as a json string to be written to a file which you can then load
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

}