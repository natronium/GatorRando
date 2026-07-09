using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(CharmPointGet))]
internal static class CharmPointGetPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CharmPointGet.PlayAnimation))]
	private static bool PrePlayAnimation()
    {
        return ItemHandling.IsItemUnlocked("CHARM KEYCHAIN");
    }
}