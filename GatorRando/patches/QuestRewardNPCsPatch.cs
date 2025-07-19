using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(QuestRewardNPCs))]
static class QuestRewardNPCsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GiveReward")]
    static void PreGiveReward(QuestRewardNPCs __instance)
    {
        if (ConnectionManager.Authenticated)
        {
            LocationHandling.CollectLocationForNPCs(__instance.rewards);
        }
    }
}