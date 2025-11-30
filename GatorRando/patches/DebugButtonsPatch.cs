using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(DebugButtons))]
internal static class DebugButtonsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(DebugButtons.IsSkipHeld), MethodType.Getter)]
	private static bool PreIsSkipHeld(ref bool __result)
    {
        if (RandoSettingsMenu.PauseForItemGet())
        {
            if (DialogueModifier.inModifiedDialogue)
            {
                __result = false;
                return false;
            }
        }
        return true;
    }
}