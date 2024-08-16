using System;
using System.Collections.Generic;
using System.Linq;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSearchNPCs))]
static class ItemSearchNPCsPatch
{
    private static List<DialogueActor> additionalActors;
    [HarmonyPostfix]
    [HarmonyPatch("GetList")]
    static void PostGetList(ref DialogueActor[] __result)
    {
        if (additionalActors == null)
        {
            GameObject act1Quests = Util.GetByPath("/NorthWest (Tutorial Island)/Act 1/Quests/");
            additionalActors.AddRange(act1Quests.GetComponentsInChildren<DialogueActor>());
            GameObject coolKidsQuest = Util.GetByPath("/East (Creeklands)/Cool Kids Quest/");
            additionalActors.AddRange(coolKidsQuest.GetComponentsInChildren<DialogueActor>());
            GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
            additionalActors.AddRange(prepQuest.GetComponentsInChildren<DialogueActor>());
            GameObject theatreQuest = Util.GetByPath("/North (Mountain)/Theatre Quest/");
            additionalActors.AddRange(theatreQuest.GetComponentsInChildren<DialogueActor>());
        }
        __result =  [.. __result, .. additionalActors];
        __result = SettingsMods.GetCheckfinderBehavior() switch
        {
            SettingsMods.CheckfinderBehavior.Logic => Array.FindAll(__result, dialogueActor => ArchipelagoManager.IsLocationAccessible(dialogueActor.profile.id)),
            SettingsMods.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, dialogueActor => ArchipelagoManager.IsLocationACheck(dialogueActor.profile.id)),
            SettingsMods.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };
    }
}