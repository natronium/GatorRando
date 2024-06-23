using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(LogicStateCollectGrass))]
static class LogicStateCollectGrassPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("OnDetailsCut")]
    static bool PreOnDetailsCut(LogicStateCollectGrass __instance, int cutAmount)
    {
        int currentCutAmount = __instance.currentCutAmount;

        currentCutAmount += cutAmount;
        if (currentCutAmount > __instance.cutAmountNeeded)
        {
            GameObject grass_seq = GameObject.Find("Got Enough Grass Sequence");
            DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
            grass_sequencer.JustStartSequence();
            __instance.enabled = false;
        }

        __instance.currentCutAmount = currentCutAmount;
        return false;
    }
}
