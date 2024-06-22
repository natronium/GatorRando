using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BraceletShopFailsafe))]
static class BraceletShopFailsafePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("CheckFailsafe")]
    static bool PreCheckFailsafe()
    {
        Plugin.LogDebug("Skipping Bracelet Failsafe!");
        return false;
    }
}