
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(LogicState))]
static class LogicStatePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("LogicCompleted")]
    static bool PreLogicCompleted(LogicState __instance)
    {
        if (__instance.stateName != "Defeat the slimes")
        {
            return true;
        }
        else
        {
            GameObject loot_seq = GameObject.Find("Loot Get Sequence");
            DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
            loot_sequencer.JustStartSequence();
            __instance.enabled = false;
            return false;
        }
    }
}