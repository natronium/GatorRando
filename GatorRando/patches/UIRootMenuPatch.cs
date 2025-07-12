using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIRootMenu))]
static class UIRootMenuPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("OnCancel")]
    static bool PreOnCancel(UIRootMenu __instance)
    {
        UISubMenu randoSettingSubMenu = Util.GetByPath(SettingsMods.GetCurrentRandoSettingsPath()).GetComponent<UISubMenu>();
        if (__instance.menuStack.Count > 0 && __instance.menuStack[__instance.menuStack.Count - 1] == randoSettingSubMenu)
        {
            if (!ArchipelagoManager.IsFullyConnected)
            {
                return false;
            }
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                return false;
            }
        }
        return true;
    }
}