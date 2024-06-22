using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using UnityEngine;
using Data;
using System;
using System.Linq;

namespace GatorRando
{
    public static class ArchipelagoManager
    {
        public static ArchipelagoSession session;
        public static LoginSuccessful loginInfo;

        public static void Connect()
        {
            if (session is not null && session.Socket.Connected)
            {
                return;
            }
            var server = "localhost";
            var user = "TestGator";

            session = ArchipelagoSessionFactory.CreateSession(server, 53072);
            LoginResult result;
            try
            {
                result = session.TryConnectAndLogin("Lil Gator Game", user, ItemsHandlingFlags.AllItems);
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

            loginInfo = (LoginSuccessful)result;

        }

        private static void CollectLocationByAPID(int id)
        {
            session.Locations.CompleteLocationChecks(id);
        }


        public static bool CollectLocationByID(int id)
        {
            Locations.Entry location;
            try
            {
                location = Locations.Entries.First((entry) => entry.client_id == id);
            }
            catch (InvalidOperationException)
            {
                Plugin.LogWarn($"Tried to collect location with numeric ID {id}, which does not have an entry in the locations table!");
                return false;
            }

            CollectLocationByAPID((int)location.ap_location_id);
            return true;
        }

        public static bool CollectLocationByName(string name)
        {
            Locations.Entry location;
            try
            {
                location = Locations.Entries.First((entry) => entry.client_name_id == name);
            }
            catch (InvalidOperationException)
            {
                Plugin.LogWarn($"Tried to collect location with string ID {name}, which does not have an entry in the locations table!");
                return false;
            }

            CollectLocationByAPID((int)location.ap_location_id);
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
                if (CollectLocationByName(NPC.id)){
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

        public static bool ItemIsUnlocked(string item)
        {
            return items_unlocked.Contains(item);
        }

        public static bool LocationIsCollected(string location)
        {
            return locations_collected.Contains(location);
        }

        private static List<string> items_unlocked = [];
        private static List<string> locations_collected = []; //Need ways to add to these fields

    }




}