using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIRootMenu))]
static class UIRootMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("OnCancel")]
    static bool PreOnCancel(UIRootMenu __instance)
    {
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