using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemHatBare))]
internal static class ItemHatBarePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ItemHatBare.OnRemove))]
	private static bool PreOnRemove(ItemHatBare __instance)
    {
        // patching to avoid observed error when equipping hats
        // Sometimes the game's refresh fails otherwise
        if (__instance.itemManager == null || __instance.itemManager.bareHead == null)
        {
            if (__instance.itemManager == null)
            {
                // Plugin.LogWarn("ItemHatBare's itemManager is null");
            }
            else
            {
                // Plugin.LogWarn("ItemHatBare's itemManager.bareHead is null");
            }
            return false;
        }
        return true;
    }
}