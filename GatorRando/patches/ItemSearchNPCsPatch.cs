using System;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSearchNPCs))]
static class ItemSearchNPCsPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("GetList")]
    static void PostGetList(ref DialogueActor[] __result)
    {
        __result = SettingsMods.GetCheckfinderBehavior() switch
        {
            SettingsMods.CheckfinderBehavior.Logic => Array.FindAll(__result, dialogueActor => ArchipelagoManager.IsLocationAccessible(dialogueActor.profile.id)),
            SettingsMods.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, dialogueActor => ArchipelagoManager.IsLocationACheck(dialogueActor.profile.id)),
            SettingsMods.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };
    }
}