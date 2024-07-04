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
        if (SceneManager.GetActiveScene().name == "Island")
        {
            UISubMenu settingSubMenu = Util.GetByPath("Canvas/Pause Menu/Settings").GetComponent<UISubMenu>();
            if (__instance.menuStack.Count > 0 && __instance.menuStack[__instance.menuStack.Count - 1] == settingSubMenu)
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
        }
        return true;
    }
}