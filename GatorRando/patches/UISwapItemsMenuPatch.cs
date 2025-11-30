using System.Collections.Generic;
using GatorRando.Archipelago;
using GatorRando.QuestMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UISwapItemsMenu))]
internal static class UISwapItemsMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(UISwapItemsMenu.UpdateInventories))]
	private static void PreUpdateInventories()
    {
        List<ItemObject> questItemsReceived = [];

        foreach (ItemObject item in QuestItems.QuestItemObjects)
        {
            if (item.name == "Broken Scooter Board")
            {
                if (ItemHandling.IsItemUnlocked(item.name, true))
                {
                    questItemsReceived.Add(item);
                }
            }
            else if (item.name == "Glider")
            {
                if (ItemHandling.IsItemUnlocked(item.name, true))
                {
                    questItemsReceived.Add(item);
                    item.IsUnlocked = true;
                }
            }
            else if (item.name == "Archipelago")
            {
                //TODO figure out replacement for this item vis-a-vie scrolling
                questItemsReceived.Add(item);
                item.IsUnlocked = true;
            }
            else if (item.name != "Thrown_Pencil_2" && item.name != "Thrown_Pencil_3" && ItemHandling.IsItemUnlocked(item.name))
            {
                questItemsReceived.Add(item);
                item.IsUnlocked = true;
            }
            else if (item.name == "Thrown_Pencil_2")
            {
                if (ItemHandling.GetItemUnlockCount("Thrown_Pencil") >= 2)
                {
                    questItemsReceived.Add(item);
                    item.IsUnlocked = true;
                }
            }
            else if (item.name == "Thrown_Pencil_3")
            {
                if (ItemHandling.GetItemUnlockCount("Thrown_Pencil") >= 3)
                {
                    questItemsReceived.Add(item);
                    item.IsUnlocked = true;
                }
            }
        }

        GameObject QuestItemGrid = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Contents Mask/Tab Contents/Quest Item Grid");
        QuestItemGrid?.GetComponent<ItemGrid>().LoadElements([.. questItemsReceived]);
    }
}