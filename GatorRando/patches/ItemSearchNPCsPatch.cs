using System;
using System.Collections.Generic;
using System.Linq;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSearchNPCs))]
internal static class ItemSearchNPCsPatch
{
    private static List<DialogueActor> additionalActors;
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ItemSearchNPCs.GetList))]
    private static void PostGetList(ref DialogueActor[] __result)
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
    private static void PostIsValid(ItemSearchNPCs __instance, DialogueActor item, ref bool __result)
    {
        string gatorName = LocationAccessibilty.ConvertDAToGatorName(item);
        string[] checks = LocationAccessibilty.ChecksForNPC(gatorName);
        __result = gatorName switch
        {
            "Tutorial Avery" => !checks.Select(LocationHandling.IsLocationCollected).All(b => b),
            "Tutorial Stick Jill" => !checks.Select(LocationHandling.IsLocationCollected).All(b => b),
            "Tutorial End Jill" => HandleTutorialEndJill(__instance, checks),
            "Tutorial Martin" => !checks.Select(LocationHandling.IsLocationCollected).All(b => b),
            "Prep Introduction Jill" => HandlePrepIntroJill(),
            "Prep Pre-Finale Jill" => HandlePrepSadJill(),
            "Prep Finale Jill" => HandlePrepFinaleJill(),
            "Cool Kid Introduction Martin" => HandleCoolKidMartin(),
            "Theatre Introduction Avery" => HandleIntroTheatreAvery(),
            "Theatre Finale Avery" => HandleFinaleTheatreAvery(),
            "NPC_Susanne" => HandlePrepSubquests(gatorName),
            "NPC_Gene" => HandlePrepSubquests(gatorName),
            "NPC_Antone" => HandlePrepSubquests(gatorName),
            "NPC_Cool_Goose" => HandleCoolSubquests(gatorName),
            "NPC_Cool_Boar" => HandleCoolSubquests(gatorName),
            "NPC_Cool_Wolf" => HandleCoolSubquests(gatorName),
            "NPC_Theatre_Cowboy" => HandleTheatreSubquests(gatorName),
            "NPC_Theatre_Space" => HandleTheatreSubquests(gatorName),
            "NPC_Theatre_Bat" => HandleTheatreSubquests(gatorName),
            "NPC_Part-Timer" => HandleTheatreSubquests(gatorName),
            _ => __result,
        };
    }

    private static bool HandleTutorialEndJill(ItemSearchNPCs thisItemSearchNPCs, string[] checks)
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

    private static bool HandlePrepIntroJill()
    {
        GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
        QuestStates questStates = prepQuest.GetComponent<QuestStates>();
        return questStates.StateID == 0;
    }

    private static bool HandlePrepSadJill()
    {
        GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
        QuestStates questStates = prepQuest.GetComponent<QuestStates>();
        return questStates.StateID == 2;
    }

    private static bool HandlePrepFinaleJill()
    {
        GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
        QuestStates questStates = prepQuest.GetComponent<QuestStates>();
        return questStates.StateID == 3;
    }

    private static bool HandlePrepSubquests(string gatorName)
    {
        GameObject prepQuest = Util.GetByPath("/West (Forest)/Prep Quest/");
        QuestStates questStates = prepQuest.GetComponent<QuestStates>();
        return questStates.StateID == 1 && gatorName switch
        {
            "NPC_Susanne" => Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/").GetComponent<QuestStates>().StateID < 4,
            "NPC_Gene" => Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/").GetComponent<QuestStates>().StateID < 3,
            "NPC_Antone" => Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist/").GetComponent<QuestStates>().StateID < 4,
            _ => false , // Should not happen
        };
    }

    private static bool HandleCoolKidMartin()
    {
        GameObject coolQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/");
        QuestStates questStates = coolQuest.GetComponent<QuestStates>();
        return questStates.StateID == 0 || questStates.StateID == 2;
    }

    private static bool HandleCoolSubquests(string gatorName)
    {
        GameObject coolQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/");
        QuestStates questStates = coolQuest.GetComponent<QuestStates>();
        return questStates.StateID == 1 && gatorName switch
        {
            "NPC_Cool_Goose" => Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Goose Quest/").GetComponent<QuestStates>().StateID < 1,
            "NPC_Cool_Boar" => Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/").GetComponent<QuestStates>().StateID < 7,
            "NPC_Cool_Wolf" => Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Wolf Quest/").GetComponent<QuestStates>().StateID < 1,
            _ => false , // Should not happen
        };;
    }

    private static bool HandleIntroTheatreAvery()
    {
        GameObject theatreQuest = Util.GetByPath("North (Mountain)/Theatre Quest/");
        QuestStates questStates = theatreQuest.GetComponent<QuestStates>();
        return questStates.StateID == 0;
    }

    private static bool HandleFinaleTheatreAvery()
    {
        GameObject theatreQuest = Util.GetByPath("North (Mountain)/Theatre Quest/");
        QuestStates questStates = theatreQuest.GetComponent<QuestStates>();
        return questStates.StateID == 2;
    }

    private static bool HandleTheatreSubquests(string gatorName)
    {
        GameObject theatreQuest = Util.GetByPath("North (Mountain)/Theatre Quest/");
        QuestStates questStates = theatreQuest.GetComponent<QuestStates>();
        int batState = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/").GetComponent<QuestStates>().StateID;
        return questStates.StateID == 1 && gatorName switch
        {
            "NPC_Theatre_Cowboy" => Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Cowfolk/").GetComponent<QuestStates>().StateID < 3,
            "NPC_Theatre_Space" => Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Space!!!/").GetComponent<QuestStates>().StateID < 7,
            "NPC_Theatre_Bat" => batState != 1 && batState !=4,
            "NPC_Part-Timer" => batState == 1,
            _ => false , // Should not happen
        };;
    }
}