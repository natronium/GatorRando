using HarmonyLib;
using UnityEngine.SceneManagement;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIMenus))]
static class UIMenusPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("OnInventory")]
    static bool PreOnInventory()
    {
        if (SceneManager.GetActiveScene().name == "Island")
        {
            if (!ArchipelagoManager.IsFullyConnected)
            {
                return false;
            }
        }
        return true;
    }
}