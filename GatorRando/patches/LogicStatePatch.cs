using GatorRando.Archipelago;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(LogicState))]
internal static class LogicStatePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(LogicState.LogicCompleted))]
	private static bool PreLogicCompleted(LogicState __instance)
    {
        if (__instance.stateName == "Defeat the slimes")
        {
            // Gene's quest, run the sequence to earn the cheese sandwich check without progressing Gene's quest
            if (!LocationHandling.IsLocationCollected("HALF A CHEESE SANDWICH"))
            {
                GameObject loot_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence");
                DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
                loot_sequencer.JustStartSequence();
            }
            __instance.enabled = false;
            return false;
        }
        else if (__instance.stateName == "Go Get Grass")
        {
            if (!LocationHandling.IsLocationCollected("CLIPPINGS"))
            {
                //Jada's quest, run the enough grass sequence at the end of go get grass without progressing Jada's quest
                GameObject grass_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence");
                DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
                grass_sequencer.JustStartSequence();
            }
            __instance.enabled = false;
            return false;
            
        }
        else if (__instance.stateName == "Go get water")
        {
            //Jada's quest, if you have a bucket, run the enough water sequence at the end of go get water without progressing Jada's quest
            // otherwise, reset go get water
            if (ItemHandling.IsItemUnlocked("Hat_Bucket"))
            {
                if (!LocationHandling.IsLocationCollected("WATER"))
                {
                    GameObject water_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence");
                    DialogueSequencer water_sequencer = water_seq.GetComponent<DialogueSequencer>();
                    water_sequencer.JustStartSequence();
                }
                __instance.enabled = false;
                return false;
            }
            else
            {
                ((LogicStateSubmerge)__instance).swimmingCounter = 0;
            }
            return false;
        }
        else
        {
            return true;
        }
    }
}