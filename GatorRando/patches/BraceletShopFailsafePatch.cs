using HarmonyLib;

namespace GatorRando.patches;

[HarmonyPatch(typeof(BraceletShopFailsafe))]
static class BraceletShopFailsafePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("CheckFailsafe")]
    static bool PreCheckFailsafe()
    {
        Plugin.LogCall("BraceletShopFailsafe", "CheckFailsafe");
        return false;
    }
}