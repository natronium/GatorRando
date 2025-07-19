using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(SaveFileScreen))]
static class SaveFileScreenPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SaveFileScreen.PressSaveFileButton))]
    static bool PrePressSaveFileButton(SaveFileScreen __instance, int index)
    {
        if (__instance.currentState == SaveFileScreen.State.Standard)
        {
            if (FileUtil.IsSaveFileStarted(index))
            {
                return StateManager.LoadGame(index);
            }
            else
            {
                StateManager.StartNewGame(index);
                return true;
            }
        }
        return true;
    }
}