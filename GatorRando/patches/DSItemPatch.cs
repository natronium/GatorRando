using Archipelago.MultiClient.Net.Models;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

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
        if (LocationHandling.CollectLocationByName(name))
        {
            ItemInfo itemInfo = LocationHandling.ItemAtLocation(name);
            __instance.isRealItem = false;
            string dialogueString = DialogueModifier.GetDialogueStringForItemInfo(itemInfo);
            __instance.itemName = DialogueModifier.GetItemNameForItemInfo(itemInfo);
            __instance.itemSprite = DialogueModifier.GetSpriteForItemInfo(itemInfo);
            __instance.dialogue = dialogueString;

            DialogueModifier.AddNewDialogueChunk(__instance.document, dialogueString);
        }
        return true;
    }
}
