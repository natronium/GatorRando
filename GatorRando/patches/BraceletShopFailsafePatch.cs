using HarmonyLib;

namespace GatorRando.Patches;

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