using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine;
using Data;
using System;
using System.Linq;
using System.Collections.Concurrent;

namespace GatorRando
{
    public class ArchipelagoManager : MonoBehaviour
    {
        public static ArchipelagoSession Session;
        public static LoginSuccessful LoginInfo;
        public static ArchipelagoManager Instance;
        private static readonly ConcurrentQueue<Items.Entry> ItemQueue = new();

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
            var server = "localhost";
            var user = "TestGator";
            var port = 62761;

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

        }

        private static readonly Dictionary<string, Action> SpecialItemFunctions = [];
        public static void RegisterItemListener(string itemName, Action listener) => SpecialItemFunctions[itemName] = listener;

        public static void OnSceneLoad()
        {
            Plugin.LogDebug("ArchipelagoManager.OnSceneLoad!");
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

            GameData.g.Write("LastAPItemIndex", Session.Items.Index);

            Session.Items.ItemReceived += helper => ItemQueue.Enqueue(GetEntryByApId(helper.DequeueItem().ItemId));
        }

        private static Items.Entry GetEntryByApId(long id) => Items.Entries.First(entry => entry.ap_item_id == id);
        private static void ReceiveItem(Items.Entry entry)
        {
            Plugin.LogDebug($"ReceiveItem for {entry.shortname} (\"{entry.longname}\"). ClientId:{entry.client_name_id}, Type:{entry.client_item_type}, AP:{entry.ap_item_id}");
            switch (entry.client_item_type)
            {
                case "Item": GiveItem(entry.client_name_id); break;
                case "Craft": GiveCraft(entry.client_name_id); break;
                case "Friends": GiveFriends((int)entry.client_resource_amount); break;
                case "Craft Stuff": GiveCraftStuff((int)entry.client_resource_amount); break;
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
                Plugin.LogWarn($"Tried to collect location with numeric ID {id}, which does not have an entry in the locations table!");
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

        public static bool CollectLocationForItem(string itemName)
        {
            Plugin.LogDebug($"Item {itemName} collected!");
            return CollectLocationByName(itemName);
        }

        public static bool CollectLocationForNPCs(CharacterProfile[] NPCs)
        {
            foreach (var NPC in NPCs)
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

        public static bool CollectLocationForBreakableObject(int breakable_id, string name)
        {
            //lookup table to filter only the ones we care about
            Plugin.LogDebug($"Breakable Object {name} {breakable_id} collected!");
            return CollectLocationByID(breakable_id);
        }

        public static bool CollectLocationForRace(int breakable_id)
        {
            Plugin.LogDebug($"Race {breakable_id} collected!");
            return CollectLocationByID(breakable_id);
        }

        public static bool CollectLocationForBracelet(string shop_save_id)
        {
            Plugin.LogDebug($"{shop_save_id} collected!");
            return CollectLocationByName(shop_save_id);
        }

        public static bool CollectLocationForJunkShop(string name)
        {
            Plugin.LogDebug($"Junk Shop item {name} collected!");
            return CollectLocationByName(name);
        }

        public static void GiveFriends(int amount)
        {
            ItemResource popresource = FindItemResourceByName("Population");
            popresource.Amount += amount;
        }

        public static void GiveCraftStuff(int amount)
        {
            ItemResource matresource = FindItemResourceByName("CraftingMaterial");
            matresource.Amount += amount;
        }

        public static void GiveItem(string item)
        {
            ItemObject itemObject = FindItemObjectByName(item);
            if (itemObject != null)
            {
                ItemManager.i.UnlockItem(itemObject.id);
                UIMenus.craftNotification.LoadItems([itemObject]);
                return;
            }
            ItemManager.i.UnlockItem(item);
        }

        public static void GiveCraft(string item)
        {
            // Gives a recipe instead of the item
            ItemObject itemObject = FindItemObjectByName(item);
            UIMenus.craftNotification.LoadItems([itemObject]);
            itemObject.hasShopEntry = true;
            itemObject.IsShopUnlocked = true;
        }

        private static ItemResource FindItemResourceByName(string name)
        {
            ItemResource[] resources = Resources.FindObjectsOfTypeAll<ItemResource>();
            foreach (ItemResource resource in resources)
            {
                if (resource.name == name)
                {
                    return resource;
                }
            }
            return null;
        }
        private static ItemObject FindItemObjectByName(string item)
        {
            ItemObject[] itemObjects = Resources.FindObjectsOfTypeAll<ItemObject>();
            foreach (ItemObject itemObject in itemObjects)
            {
                if (itemObject.name == item)
                {
                    return itemObject;
                }
            }
            return null;
        }

        public static bool ItemIsUnlocked(string item) =>
            Session.Items.AllItemsReceived.Select(info => info.ItemId).Contains(GetItemApId(item));

        public static bool LocationIsCollected(string location) =>
            Session.Locations.AllLocationsChecked.Contains(GetLocationApId(location));
    }
}