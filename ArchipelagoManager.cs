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
        public static void CollectLocationForItem(string itemName)
        {
            Plugin.LogDebug($"Item {itemName} collected!");
        }

        public static void CollectLocationForNPC(string NPCname)
        {
            Plugin.LogDebug($"NPC {NPCname} collected!");
        }

        public static void CollectLocationForConfetti(string name)
        {
            Plugin.LogDebug($"Confetti {name} collected!");
        }

        public static void CollectLocationForParticlePickup(ParticlePickup particlePickup)
        {
            Plugin.LogDebug($"Particle Pickup {particlePickup.name} collected!");
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

        private static List<string> items_unlocked = [];

         
    }




}