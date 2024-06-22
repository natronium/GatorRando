using HarmonyLib;
using UnityEngine;

namespace GatorRando.patches;

[HarmonyPatch(typeof(LogicStateCollectGrass))]
static class LogicStateCollectGrassPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("OnDetailsCut")]
    static bool PreOnDetailsCut(LogicStateCollectGrass __instance, int cutAmount)
    {
        Plugin.LogCall("LogicStateCollectGrass", "OnDetailsCut");
        Traverse traverse = Traverse.Create(__instance).Field("currentCutAmount");
        int currentCutAmount = traverse.GetValue<int>();

        currentCutAmount += cutAmount;
        if (currentCutAmount > __instance.cutAmountNeeded)
        {
            GameObject grass_seq = GameObject.Find("Got Enough Grass Sequence");
            DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
            grass_sequencer.JustStartSequence();
            __instance.enabled = false;
        }

        traverse.SetValue(currentCutAmount);
        return false;
    }
}
