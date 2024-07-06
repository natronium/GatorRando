using Archipelago.MultiClient.Net.Models;
using Data;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(DSItem))]
static class DSItemPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("RunItemSequence")]
    static bool PreRunItemSequence(DSItem __instance)
    {
        //TODO: Don't intercept Craft Stuff, Pot Lid?, LITTER
        // TODO: decide how to handle Sword_Pencil
        string name = "";
        if (__instance.item == null || __instance.itemName == "POT?" || __instance.itemName == "POT LID?")
        {
            name = __instance.itemName;
        }
        else
        {
            name = __instance.item.name;
        }
        if (ArchipelagoManager.CollectLocationByName(name))
        {
            ItemInfo itemInfo = ArchipelagoManager.ItemAtLocation(name);
            __instance.document = null;
            __instance.isRealItem = false;
            if (itemInfo.ItemGame == "Lil Gator Game")
            {
                if (itemInfo.ItemName.Contains("Craft Stuff") || itemInfo.ItemName.Contains("Friend"))
                {
                    __instance.itemSprite = Util.GetSpriteForItem(itemInfo.ItemName);
                }
                else
                {
                    string clientID = ArchipelagoManager.GetClientIDByAPId(itemInfo.ItemId);
                    __instance.itemSprite = Util.GetSpriteForItem(clientID);
                }
            }
            if (itemInfo.Player.Name == GameData.g.gameSaveData.playerName)
            {
                __instance.dialogue = $"found my {itemInfo.ItemName}. why was that here??"; // Need to replace this with a valid dialogue?
                __instance.itemName = itemInfo.ItemName;
            }
            else if (itemInfo.ItemGame == "Lil Gator Game")
            {
                __instance.dialogue = $"found a {itemInfo.ItemName}, but it's {itemInfo.Player.Name}'s, not mine, I should send it back"; // Need to replace this with a valid dialogue?
                __instance.itemName = itemInfo.Player.Name + "'s " + itemInfo.ItemName;
            }
            else
            {
                __instance.dialogue = $"Found {itemInfo.Player.Name}'s {itemInfo.ItemName}"; // Need to replace this with a valid dialogue?
                __instance.itemName = itemInfo.Player.Name + "'s " + itemInfo.ItemName;
                __instance.itemSprite = Util.GetSpriteForItem("Archipelago");
                // Eventually replace itemSprite with AP logo
            }
        }
        return true;
    }
}
