using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UINameInput))]
internal static class UINameInputPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UINameInput.Awake))]
	private static void PostAwake(UINameInput __instance)
    {
        __instance.inputField.characterLimit = 16;
    }
}