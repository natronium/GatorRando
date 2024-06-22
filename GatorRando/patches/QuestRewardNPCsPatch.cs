

using HarmonyLib;

namespace GatorRando.patches;

[HarmonyPatch(typeof(QuestRewardNPCs))]
static class QuestRewardNPCsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GiveReward")]
    static void PreGiveReward(QuestRewardNPCs __instance)
    {
        ArchipelagoManager.CollectLocationForNPCs(__instance.rewards); //BUG: This line fails with ???                
    }
}