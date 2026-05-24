using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UIButtonPrompt))]
internal static class UIButtonPromptPatch
{
    // [HarmonyPrefix]
    // [HarmonyPatch(nameof(UIButtonPrompt.Awake))]
    // private static void PreAwake(UIButtonPrompt __instance)
    // {
    //     if (__instance.allowSkip)
    //     {
    //         if ((RandoSettingsMenu.PauseForItemGet() && DialogueModifier.inModifiedDialogue) || DialogueModifier.inTrapDialogue)
    //         {
    //             __instance.allowSkip = false;
    //         }
    //     }
    // }

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