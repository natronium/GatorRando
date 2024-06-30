using System.Collections.Generic;
using GatorRando.QuestMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UISwapItemsMenu))]
static class UISwapItemsMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("UpdateInventories")]
    static void PreUpdateInventories()
    {
        List<ItemObject> questItemsReceived = [];
        foreach (ItemObject item in QuestItems.QuestItemObjects)
        {
            if (ArchipelagoManager.ItemIsUnlocked(item.name))
            {
                questItemsReceived.Add(item);
            }
        }

        GameObject QuestItemGrid = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Contents Mask/Tab Contents/Quest Item Grid");
        QuestItemGrid?.GetComponent<ItemGrid>().LoadElements([.. questItemsReceived]);
    }
}