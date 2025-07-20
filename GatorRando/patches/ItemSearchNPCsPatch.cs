using System;
using System.Collections.Generic;
using System.Linq;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSearchNPCs))]
static class ItemSearchNPCsPatch
{
    private static List<DialogueActor> additionalActors;
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ItemSearchNPCs.GetList))]
    static void PostGetList(ref DialogueActor[] __result)
    {
        if (additionalActors == null)
        {
            GameObject act1Quests = Util.GetByPath("/NorthWest (Tutorial Island)/Act 1/Quests/");
            additionalActors = [.. act1Quests.GetComponentsInChildren<DialogueActor>(true)];
            GameObject coolKidsQuest = Util.GetByPath("/East (Creeklands)/Cool Kids Quest/");
            additionalActors.AddRange(coolKidsQuest.GetComponentsInChildren<DialogueActor>(true));
            GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
            additionalActors.AddRange(prepQuest.GetComponentsInChildren<DialogueActor>(true));
            GameObject theatreQuest = Util.GetByPath("/North (Mountain)/Theatre Quest/");
            additionalActors.AddRange(theatreQuest.GetComponentsInChildren<DialogueActor>(true));
            additionalActors = additionalActors.FindAll(dialogueActor => dialogueActor.profile);
            //TODO: remove extraneous additional actors like signs, and possibly main quest actors with finished (sub)quests? 
        }

        List<DialogueActor> filteredMainActors = RandoSettingsMenu.GetCheckfinderBehavior() switch
        {
            RandoSettingsMenu.CheckfinderBehavior.Logic => additionalActors.FindAll(LocationAccessibilty.IsMainQuestAccessible),
            RandoSettingsMenu.CheckfinderBehavior.ChecksOnly => additionalActors.FindAll(LocationAccessibilty.IsMainQuestACheck),
            RandoSettingsMenu.CheckfinderBehavior.Original => additionalActors,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };

        DialogueActor[] sideQuestActors = RandoSettingsMenu.GetCheckfinderBehavior() switch
        {
            RandoSettingsMenu.CheckfinderBehavior.Logic => Array.FindAll(__result, LocationAccessibilty.IsNPCAccessible),
            RandoSettingsMenu.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, LocationAccessibilty.IsNPCACheck),
            RandoSettingsMenu.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };

        __result = [.. sideQuestActors, .. filteredMainActors];

        // foreach(DialogueActor actor in __result){
        //     Plugin.LogInfo("ID:" + actor.profile.id + ", Parent:" + actor.transform.parent.name);
        // }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(ItemSearchNPCs.IsValid))]
    static void PostIsValid(ItemSearchNPCs __instance, DialogueActor item, ref bool __result)
    {
        __result = __result && item.gameObject.activeInHierarchy;
        string gatorName = LocationAccessibilty.ConvertDAToGatorName(item);
        string[] checks = LocationAccessibilty.ChecksForNPC(gatorName);
        __result = gatorName switch
        {
            "Tutorial Avery" => !checks.Select(LocationHandling.IsLocationCollected).All(b => b),
            "Tutorial Stick Jill" => !checks.Select(LocationHandling.IsLocationCollected).All(b => b),
            "Tutorial End Jill" => HandleTutorialEndJill(__instance, checks),
            "Tutorial Martin" => !checks.Select(LocationHandling.IsLocationCollected).All(b => b),
            _ => __result,
        };
    }

    static bool HandleTutorialEndJill(ItemSearchNPCs thisItemSearchNPCs, string[] checks)
    {
        if (thisItemSearchNPCs.GetList().First(da => LocationAccessibilty.ConvertDAToGatorName(da) == "NPC_TutIsland_Duck").gameObject.activeInHierarchy)
        {
            return false;
        }
        else
        {
            return !checks.Select(LocationHandling.IsLocationCollected).All(b => b);
        }
    }
}