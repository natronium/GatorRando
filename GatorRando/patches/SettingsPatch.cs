using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(Settings))]
static class SettingsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Settings.LoadSettings))]
    static void PostLoadSettings()
    {
        SpeedrunData.autoName = AutoNameFunctionality.Off;
        SpeedrunTimerDisplay.ShowOrHideTimer(); // Show or hide speedrun timer based on settings
    }
}