using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(TownNPCManager))]
internal static class TownNPCManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TownNPCManager.Awake))]
	private static void PreAwake(TownNPCManager __instance)
    {
        __instance.populationResource = Util.GenerateItemResource("Dummy_Resource_Population");
    }
}