using System;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSearchObjects))]
static class ItemSearchObjectsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("GetList")]
    static void PostGetList(ref PersistentObject[] __result)
    {
        __result = RandoSettingsMenu.GetCheckfinderBehavior() switch
        {
            RandoSettingsMenu.CheckfinderBehavior.Logic => Array.FindAll(__result, LocationAccessibilty.IsLocationAccessible),
            RandoSettingsMenu.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, LocationAccessibilty.IsLocationACheck),
            RandoSettingsMenu.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };
    }
}