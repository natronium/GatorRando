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
            __instance.document = null;
            __instance.dialogue = "Collected an AP Item!"; // Need to replace this with a valid dialogue?
            __instance.isRealItem = false;
            __instance.itemName = "AP Item Here!";
            // Eventually replace itemSprite too
        }
        return true;
    }
}
