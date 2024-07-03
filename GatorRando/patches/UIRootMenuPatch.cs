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
        UISubMenu settingSubMenu = Util.GetByPath("Canvas/Pause Menu/Settings").GetComponent<UISubMenu>();
        if (__instance.menuStack[__instance.menuStack.Count - 1] == settingSubMenu)
        {
            if (!ArchipelagoManager.IsConnected())
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