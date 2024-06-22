using HarmonyLib;

namespace GatorRando.patches;

[HarmonyPatch(typeof(InteractItemUnlock))]
static class InteractItemUnlockPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Interact")]
    static bool PreInteract(InteractItemUnlock __instance)
    {
        Plugin.LogCheck("InteractItemUnlock", "Interact", __instance.itemName);
        __instance.gameObject.SetActive(false);
        __instance.SaveTrue();
        ArchipelagoManager.CollectLocationByName(__instance.itemName);
        return false;
    }
}