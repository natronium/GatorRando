using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using UnityEngine;
using Data;
using System;

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

        static int KnownLocationCount => Items.Entries.Length;
        static int KnownItemTypeCount => Locations.Entries.Length;

        public static bool CollectLocationForItem(string itemName)
        {
            Plugin.LogDebug($"Item {itemName} collected!");
            // Lookup table here
            return true;
        }

        public static void CollectLocationForNPCs(CharacterProfile[] NPCs)
        {
            foreach (var NPC in NPCs)
            {
                Plugin.LogDebug($"NPC {NPC.id} collected!");
            }
        }

        // public static void CollectLocationForConfetti(string name)
        // {
        //     Plugin.LogDebug($"Confetti {name} collected!");
        // }

        public static void CollectLocationForBreakableObject(int breakable_id, string name)
        {
            //lookup table to filter only the ones we care about
            Plugin.LogDebug($"Breakable Object {name} {breakable_id} collected!");
        }

        public static void CollectLocationForRace(int breakable_id)
        {
            Plugin.LogDebug($"Race {breakable_id} collected!");
        }

        public static void CollectLocationForBracelet(string shop_save_id)
        {
            Plugin.LogDebug($"{shop_save_id} collected!");
        }

        public static void CollectLocationForJunkShop(string name)
        {
            Plugin.LogDebug($"Junk Shop item {name} collected!");
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