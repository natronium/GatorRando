using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(PlayerSpecialStamina))]
internal static class PlayerSpecialStaminaPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(PlayerSpecialStamina.RefreshStaminaPoints))]
	private static bool PreRefreshStaminaPoints()
    {
        return ItemHandling.IsItemUnlocked("CHARM KEYCHAIN");
    }
}