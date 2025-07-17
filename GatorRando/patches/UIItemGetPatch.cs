using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIItemGet))]
static class UIItemGetPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIItemGet.Deactivate))]
    static void PostDeactivate()
    {
        DialogueModifier.SetModifiedDialogue(false);
    }
}