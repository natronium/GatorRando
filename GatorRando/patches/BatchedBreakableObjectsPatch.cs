using GatorRando.Archipelago;
using HarmonyLib;
using System.Linq;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BatchedBreakableObjects))]
internal static class BatchedBreakableObjectsPatch
{
    internal static bool breakingBatch = false;
    [HarmonyPrefix]
    [HarmonyPatch(nameof(BatchedBreakableObjects.OnBreakableBroken))]
	private static void PreOnBreakableBroken(BatchedBreakableObjects __instance)
    {
        BreakableObjectPatch.ignoreWallBreakables.AddRange(__instance.batchedBreakables.Select(b => b.id));
        LocationHandling.CollectLocationByID(__instance.id);
        // Filter out BreakableObjects that break as part of a batch
    }
}