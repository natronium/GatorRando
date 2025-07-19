using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(TownNPCManager))]
static class TownNPCManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TownNPCManager.Awake))]
    static void PreAwake(TownNPCManager __instance)
    {
        __instance.populationResource = Util.GenerateItemResource("Dummy_Resource_Population");
    }
}