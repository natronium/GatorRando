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
            if (item.name != "Thrown_Pencil_2" && item.name != "Thrown_Pencil_3" && ArchipelagoManager.IsItemUnlocked(item.name))
            {
                questItemsReceived.Add(item);
                item.IsUnlocked = true;
            }
            else if (item.name == "Thrown_Pencil_2")
            {
                if(ArchipelagoManager.GetItemUnlockCount("Thrown_Pencil") >= 2)
                {
                    questItemsReceived.Add(item);
                    item.IsUnlocked = true;
                }
            }
            else if (item.name == "Thrown_Pencil_3")
            {
                if(ArchipelagoManager.GetItemUnlockCount("Thrown_Pencil") >= 3)
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