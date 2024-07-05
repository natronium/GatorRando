using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(Settings))]
static class SettingsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("LoadSettings")]
    static void PostLoadSettings()
    {
        SpeedrunData.autoName = AutoNameFunctionality.Off;
    }
}