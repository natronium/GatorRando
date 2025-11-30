using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BraceletShopFailsafe))]
internal static class BraceletShopFailsafePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(BraceletShopFailsafe.CheckFailsafe))]
	private static bool PreCheckFailsafe()
    {
        // Skip the Bracelet Failsafe! Don't run the original code!
        return false;
    }
}