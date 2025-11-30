using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(SpeedrunData))]
static class SpeedrunDataPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(SpeedrunData.IsSpeedrunMode), MethodType.Setter)]
    static bool PreSetSpeedrunMode(bool value)
    {
        SpeedrunData.isSpeedrunMode = value;
        return false;
    }
}