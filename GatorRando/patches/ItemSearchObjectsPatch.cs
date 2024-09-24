using System;
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
        __result = SettingsMods.GetCheckfinderBehavior() switch
        {
            SettingsMods.CheckfinderBehavior.Logic => Array.FindAll(__result, ArchipelagoManager.IsLocationAccessible),
            SettingsMods.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, ArchipelagoManager.IsLocationACheck),
            SettingsMods.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };
    }
}