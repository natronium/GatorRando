using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(Settings))]
internal static class SettingsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Settings.LoadSettings))]
	private static void PostLoadSettings()
    {
        SpeedrunData.autoName = AutoNameFunctionality.Off;
        SpeedrunTimerDisplay.ShowOrHideTimer(); // Show or hide speedrun timer based on settings
    }
}