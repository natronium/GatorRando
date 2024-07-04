using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(FallbackCraftIdea))]
static class FallbackCraftIdeaPatch {
    [HarmonyPrefix]
    [HarmonyPatch("CheckState")]
    private static bool PreCheckState(FallbackCraftIdea __instance, int stateID) {
        if (stateID >= __instance.minState)
		{
            if (ArchipelagoManager.IsFullyConnected)
            {
			    ArchipelagoManager.CollectLocationByName(__instance.craft.name);
            }
		}
        return false;
    }
}