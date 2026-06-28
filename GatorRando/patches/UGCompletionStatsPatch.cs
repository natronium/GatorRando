using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UGCompletionStats))]
internal static class UGCompletionStatsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(UGCompletionStats.ShouldDisplay), MethodType.Getter)]
	private static bool PreGetShouldDisplay(ref bool __result)
    {
        __result = true;
        return false;
    }
}