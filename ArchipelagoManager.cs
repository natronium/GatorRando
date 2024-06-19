using System;
using System.Collections.Generic;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando
{
    public static class ArchipelagoManager
    {
        public static ArchipelagoSession session;
        public static RoomInfoPacket roomInfo;
        public static LoginResult loginResult; 
        public static async void thingy() {
            //TODO: load these values from somewhere. config file to start? eventually UI
            session = ArchipelagoSessionFactory.CreateSession("localhost", 38281);
            //session.TryConnectAndLogin("Lil Gator Game", "TestGator", ItemsHandlingFlags.AllItems);
            roomInfo = await session.ConnectAsync();
            loginResult = await session.LoginAsync("Clique", "TestGator", ItemsHandlingFlags.AllItems);            
        }

        public static bool CollectLocationForItem(string itemName)
        {
            Plugin.LogDebug($"Item {itemName} collected!");
            // Lookup table here
            return true;
        }

        public static void CollectLocationForNPCs(CharacterProfile[] NPCs)
        {
            foreach (var NPC in NPCs){
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

        public static void CollectLocationForBracelet(int shop_id)
        {
            Plugin.LogDebug($"Bracelet {shop_id} collected!");
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
                return;
            }
            ItemManager.i.UnlockItem(item);
        }

        public static void GiveCraft(string item)
        {
            // Gives a recipe instead of the item
            ItemObject itemObject = FindItemObjectByName(item);
            ItemObject[] itemObjects = [itemObject];
            UIMenus.craftNotification.LoadItems(itemObjects);
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