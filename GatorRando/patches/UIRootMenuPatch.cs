using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIRootMenu))]
internal static class UIRootMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(UIRootMenu.OnCancel))]
	private static bool PreOnCancel(UIRootMenu __instance)
    {
        // Prevent backspace in the rando settings menu from exiting the menu
        UISubMenu randoSettingSubMenu = Util.GetByPath(RandoSettingsMenu.GetCurrentRandoSettingsPath()).GetComponent<UISubMenu>();
        if (__instance.menuStack.Count > 0 && __instance.menuStack[__instance.menuStack.Count - 1] == randoSettingSubMenu)
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                return false;
            }
        }
        return true;
    }
}