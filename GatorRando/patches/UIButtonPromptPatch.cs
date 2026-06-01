using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIButtonPrompt))]
internal static class UIButtonPromptPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(UIButtonPrompt.Awake))]
    private static void PostAwake(UIButtonPrompt __instance)
    {
        if (__instance.allowSkip)
        {
            __instance.waitUntilTriggered = new UnityEngine.WaitUntil(() => __instance.triggered || DebugButtons.IsSkipHeld);
            //Restore pre-DLC behavior so that we can turn on and off speedrun dialogue skipping at will
        }
    }

}