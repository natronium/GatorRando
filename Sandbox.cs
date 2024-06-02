using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando
{
    public class Sandbox
    {
        private static void testreward()
        {

            // QuestRewardNPCs testreward = new QuestRewardNPCs();
            // testreward.name = "test reward";
            // testreward.overrideCount = true;
            // testreward.rewardCount = 5;
            // CharacterProfile characterreward = new CharacterProfile();
            // characterreward.Name = "Tom";
            // testreward.rewards = [characterreward, characterreward, characterreward, characterreward, characterreward];
            // testreward.GiveReward();

            // QuestRewardNPCs testreward = new QuestRewardNPCs();
            // testreward.overrideCount = false;
            // testreward.rewardCount = 1;
            // CharacterProfile characterreward = new CharacterProfile();
            // characterreward.name = "Twig";
            // CharacterProfile[] manyToms = new CharacterProfile[1];
            // for (int i = 0; i < 1; i++)
            // {
            //     manyToms[i] = characterreward;
            // }
            // testreward.rewards = manyToms;
            // Inspect(testreward);
            // testreward.GiveReward();
            // Inspect(testreward);

            // GameObject testrewardobj = new GameObject();
            // QuestRewardNPCs testreward = testrewardobj.AddComponent<QuestRewardNPCs>();
            // testreward.overrideCount = false;
            // testreward.rewardCount = 1;
            // CharacterProfile characterreward = new CharacterProfile();
            // characterreward.name = "Twig";
            // characterreward.IsUnlocked = true;
            // CharacterProfile[] manyToms = new CharacterProfile[1];
            // for (int i = 0; i < 1; i++)
            // {
            //     manyToms[i] = characterreward;
            // }
            // testreward.rewards = manyToms;
            // testreward.GiveReward();


            QuestRewardResources testreward = new QuestRewardResources();
            ItemResource[] resources = (ItemResource[])Resources.FindObjectsOfTypeAll(typeof(ItemResource));
            ItemResource popresource = null;
            foreach (ItemResource resource in resources)
            {
                if (resource.name == "Population")
                {
                    popresource = resource;
                }
            }
            testreward.resource = popresource;
            testreward.amount = 5;
            testreward.GiveReward();

        }
        [HarmonyPatch(typeof(ItemManager))]
        private static class ItemManagerPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("UnlockItem")]
            static void PreUnlockItem(string itemID)
            {
                // LogCall("ItemManager", "UnlockItem");
            }

            // [HarmonyPrefix]
            // [HarmonyPatch("EquipItem")]
            // static void PreEquipItem()
            // {
            //     // LogCall("ItemManager", "EquipItem");
            // }

            [HarmonyPrefix]
            [HarmonyPatch("GiveItem")]
            static void PreGiveItem(ItemObject item, bool equip)
            {
                // LogCall("ItemManager", "GiveItem");
            }

            [HarmonyPrefix]
            [HarmonyPatch("SetUnlocked")]
            static void PreSetUnlocked(string itemName)
            {
                // LogCall("ItemManager", "SetUnlocked");
            }
            //EquipItem (there are several??)
            //GiveItem
            //UnlockItem
            //SetUnlocked?

        }

        [HarmonyPatch(typeof(ItemObject))]
        private static class ItemObejctPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("IsUnlocked", MethodType.Setter)]
            static void PreSetIsUnlocked(bool value)
            {
                // LogCall("ItemObject", "set_IsUnlocked");
            }
        }
        String item = "Sword_Net";
        ItemObject[] itemObjects = Resources.FindObjectsOfTypeAll<ItemObject>();
        ItemObject io;
        foreach (ItemObject itemObject in itemObjects)
                    {
                        if (itemObject.name == item)
                        {
                            io = itemObject;
                        }
        }

        if (io != null)
        {
            ItemManager.i.UnlockItem(io.id);
        }

    }
}