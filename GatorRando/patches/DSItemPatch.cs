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
    static void PreRunItemSequence(DSItem __instance)
    {
        string name;
        if (__instance.item == null || __instance.itemName == "POT?" || __instance.itemName == "POT LID?")
        {
            name = __instance.itemName;
        }
        else
        {
            name = __instance.item.name;
        }
        if (name == "" || name == "LITTER")
        {
            // Make sure the first Craft Stuff and Litter are not caught by this alteration
            return;
        }
        if (LocationHandling.CollectLocationByName(name))
        {
            LocationHandling.ItemAtLocation itemAtLocation = LocationHandling.GetItemAtLocation(name);
            __instance.isRealItem = false;
            string dialogueString = DialogueModifier.GetDialogueStringForItemAtLocation(itemAtLocation);
            __instance.itemName = DialogueModifier.GetItemNameForItemAtLocation(itemAtLocation);
            __instance.itemName_ID = __instance.itemName;
            __instance.itemSprite = DialogueModifier.GetSpriteForItemAtLocation(itemAtLocation);
            __instance.dialogue = dialogueString;

            DialogueModifier.AddNewDialogueChunk(__instance.document, dialogueString);
        }
    }
}
