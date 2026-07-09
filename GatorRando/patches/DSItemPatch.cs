using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;
using System.Collections;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(DSItem))]
internal static class DSItemPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(DSItem.RunItemSequence))]
	private static bool PreRunItemSequence(DSItem __instance, ref IEnumerator __result)
    {
        string name;
        if (__instance.item == null || __instance.itemName == "POT?" || __instance.itemName == "POT LID?")
        {
            if (__instance.itemName == "")
            {
                name = __instance.itemName_ID; //DLC added some items that only have this field...
            }
            else
            {
                name = __instance.itemName;
            }
        }
        else
        {
            name = __instance.item.name;
        }
        if (name == "" || name == "LITTER" || name == "CraftingMaterial_Name")
        {
            // Make sure the first Craft Stuff and Litter are not caught by this alteration
            return true;
        }
        if (name.Contains("Greet"))
        {
            name = "Queen's Secret Letter";
        }
        if (name.Contains("Recieve")) // Typo is in vanilla game
        {
            name = "Other Queen's Secret Letter";
        }
        if (name.Contains("SpecialStamina_Name"))
        {
            __result = DoNothing();
            return false;
        }
        if (LocationHandling.CollectLocationByName(name))
        {
            DialogueModifier.SetModifiedDialogue(true);
            LocationHandling.ItemAtLocation itemAtLocation = LocationHandling.GetItemAtLocation(name);
            __instance.isRealItem = false;
            string dialogueString = DialogueModifier.GetDialogueStringForItemAtLocation(itemAtLocation);
            __instance.itemName = DialogueModifier.GetItemNameForItemAtLocation(itemAtLocation);
            __instance.itemName_ID = __instance.itemName;
            __instance.itemSprite = DialogueModifier.GetSpriteForItemAtLocation(itemAtLocation);
            __instance.dialogue = dialogueString;

            DialogueModifier.AddNewDialogueChunk(dialogueString, __instance.document);
        }
        return true;
    }

    private static IEnumerator DoNothing()
    {
        yield break;
    }

}
