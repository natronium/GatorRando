using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ClamQuest))]
internal static class ClamQuestPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ClamQuest.IsPlayerInWater))]
    private static void PostPickUpClam(ClamQuest __instance)
    {
        if (!ItemHandling.IsItemUnlocked("Clam"))
        {
            __instance.DropClam(); // If do not have the Clam item, drop the clam
        }
    }
}