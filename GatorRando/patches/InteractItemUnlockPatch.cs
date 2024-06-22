using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(InteractItemUnlock))]
static class InteractItemUnlockPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Interact")]
    static bool PreInteract(InteractItemUnlock __instance)
    {
        __instance.gameObject.SetActive(false);
        __instance.SaveTrue();
        ArchipelagoManager.CollectLocationByName(__instance.itemName);
        return false;
    }
}