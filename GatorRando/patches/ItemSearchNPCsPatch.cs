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
            additionalActors = new List<DialogueActor>(act1Quests.GetComponentsInChildren<DialogueActor>(true));
            GameObject coolKidsQuest = Util.GetByPath("/East (Creeklands)/Cool Kids Quest/");
            additionalActors.AddRange(coolKidsQuest.GetComponentsInChildren<DialogueActor>(true));
            GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
            additionalActors.AddRange(prepQuest.GetComponentsInChildren<DialogueActor>(true));
            GameObject theatreQuest = Util.GetByPath("/North (Mountain)/Theatre Quest/");
            additionalActors.AddRange(theatreQuest.GetComponentsInChildren<DialogueActor>(true));
            additionalActors = additionalActors.FindAll(dialogueActor => dialogueActor.profile);
            //TODO: remove extraneous additional actors like signs, and possibly main quest actors with finished (sub)quests? 
        }

        List<DialogueActor> filteredMainActors = SettingsMods.GetCheckfinderBehavior() switch
        {
            SettingsMods.CheckfinderBehavior.Logic => additionalActors.FindAll(ArchipelagoManager.IsMainQuestAccessible),
            SettingsMods.CheckfinderBehavior.ChecksOnly => additionalActors.FindAll(ArchipelagoManager.IsMainQuestACheck),
            SettingsMods.CheckfinderBehavior.Original => additionalActors,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };

        DialogueActor[] sideQuestActors = SettingsMods.GetCheckfinderBehavior() switch
        {
            SettingsMods.CheckfinderBehavior.Logic => Array.FindAll(__result, ArchipelagoManager.IsNPCAccessible),
            SettingsMods.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, ArchipelagoManager.IsNPCACheck),
            SettingsMods.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };

        __result =  [.. sideQuestActors, .. filteredMainActors];

        foreach(DialogueActor actor in __result){
            Plugin.LogInfo("ID:" + actor.profile.id + ", Parent:" + actor.transform.parent.name);
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("IsValid")]
    static void PostIsValid(DialogueActor item, ref bool __result)
    {
        __result = __result && item.gameObject.activeInHierarchy;
        string gatorName = ArchipelagoManager.ConvertDAToGatorName(item);
        string[] checks = ArchipelagoManager.ChecksForNPC(gatorName);
        __result = gatorName switch
        {
            "Tutorial Avery" => !checks.Select(ArchipelagoManager.IsLocationCollected).All(b => b),
            "Tutorial Stick Jill" => !checks.Select(ArchipelagoManager.IsLocationCollected).All(b => b),
            "Tutorial End Jill" => !checks.Select(ArchipelagoManager.IsLocationCollected).All(b => b),
            "Tutorial Martin" => !checks.Select(ArchipelagoManager.IsLocationCollected).All(b => b),
            _ => __result,
        };
    }
}