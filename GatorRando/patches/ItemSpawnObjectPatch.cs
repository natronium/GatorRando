using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSpawnObject))]
internal static class ItemSpawnObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ItemSpawnObject.Input))]
	private static void PreInput(ItemSpawnObject __instance)
    {
        __instance.minimumStamina = -1; // Make Balloon and Bubble Gum not require stamina initially, will still immediately pop, but can use them as Cardboard Destroyers
    }
}