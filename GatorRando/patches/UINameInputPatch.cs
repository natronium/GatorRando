using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UINameInput))]
static class UINameInputPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UINameInput.Awake))]
    static void PostAwake(UINameInput __instance)
    {
        __instance.inputField.characterLimit = 16;
    }
}