using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(FallbackCraftIdea))]
static class FallbackCraftIdeaPatch {
    [HarmonyPrefix]
    [HarmonyPatch("CheckState")]
    private static bool PreCheckState(FallbackCraftIdea __instance, int stateID) {
        if (stateID >= __instance.minState)
		{
            if (ConnectionManager.Authenticated)
            {
			    LocationHandling.CollectLocationByName(__instance.craft.name);
            }
		}
        return false;
    }
}