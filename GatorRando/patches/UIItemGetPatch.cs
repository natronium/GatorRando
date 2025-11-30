using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIItemGet))]
internal static class UIItemGetPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIItemGet.Deactivate))]
	private static void PostDeactivate()
    {
        DialogueModifier.SetModifiedDialogue(false);
    }
}