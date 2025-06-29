using System.Data.Common;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(DSItem))]
static class DSItemPatch
{
    
 
    [HarmonyPrefix]
    [HarmonyPatch("RunItemSequence")]
    static bool PreRunItemSequence(DSItem __instance)
    {
        //TODO: Don't intercept LITTER
        string name;
        if (__instance.item == null || __instance.itemName == "POT?" || __instance.itemName == "POT LID?")
        {
            name = __instance.itemName;
        }
        else
        {
            name = __instance.item.name;
        }
        if (name == "")
        {
            // Make sure the first Craft Stuff is not caught by this alteration
            return true;
        }
        if (ArchipelagoManager.CollectLocationByName(name))
        {
            ItemInfo itemInfo = ArchipelagoManager.ItemAtLocation(name);

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
            string dialogueString;
            if (itemInfo.Player.Name == GameData.g.gameSaveData.playerName)
            {
                dialogueString = $"found my {itemInfo.ItemName}. why was that here??";
                __instance.itemName = itemInfo.ItemName;
            }
            else if (itemInfo.ItemGame == "Lil Gator Game")
            {
                dialogueString = $"found a {itemInfo.ItemName}, but it's {itemInfo.Player.Name}'s, not mine, I should send it back";
                __instance.itemName = itemInfo.Player.Name + "'s " + itemInfo.ItemName;
            }
            else
            {
                dialogueString = $"found {itemInfo.Player.Name}'s {itemInfo.ItemName}";
                __instance.itemName = itemInfo.Player.Name + "'s " + itemInfo.ItemName;
                __instance.itemSprite = Util.GetSpriteForItem("Archipelago");
                // Eventually replace itemSprite with AP logo
            }
            
            __instance.dialogue = dialogueString;

            DialogueModifier.AddNewDialogueChunk(__instance.document, dialogueString);
        }
        return true;
    }
}
