using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BraceletShopFailsafe))]
static class BraceletShopFailsafePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(BraceletShopFailsafe.CheckFailsafe))]
    static bool PreCheckFailsafe()
    {
        // Skip the Bracelet Failsafe! Don't run the original code!
        return false;
    }
}