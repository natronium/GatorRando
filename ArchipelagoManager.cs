using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando
{
    public class ArchipelagoManager
    {
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
    }




}