using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(SaveFileScreen))]
static class SaveFileScreenPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SaveFileScreen.PressSaveFileButton))]
    static void PrePressSaveFileButton(SaveFileScreen __instance, int index)
    {
        if (__instance.currentState == SaveFileScreen.State.Standard)
        {
            if (FileUtil.IsSaveFileStarted(index))
            {
                StateManager.LoadGame();
            }
            else
            {
                StateManager.StartNewGame(index);
            }
        }
    }
}