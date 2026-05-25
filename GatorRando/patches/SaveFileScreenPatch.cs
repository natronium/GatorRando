using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(SaveFileScreen))]
internal static class SaveFileScreenPatch
{
    internal static bool startingLoad = false;

    [HarmonyPrefix]
    [HarmonyPatch(nameof(SaveFileScreen.PressSaveFileButton))]
	private static bool PrePressSaveFileButton(SaveFileScreen __instance, int index)
    {
        if (__instance.currentState == SaveFileScreen.State.Standard && !startingLoad)
        {
            startingLoad = true;
            if (FileUtil.IsSaveFileStarted(index))
            {
                return StateManager.LoadGame(index);
            }
            else
            {
                return StateManager.StartNewGame(index);
            }
        }
        else
        {
            return !startingLoad;
        }
    }
}