using HarmonyLib;

namespace GatorRando.patches;

[HarmonyPatch(typeof(QuestRewardCrafts))]
static class QuestRewardCraftsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GiveReward")]
    static bool PreGiveReward(QuestRewardCrafts __instance)
    {
        Plugin.LogCheck("QuestRewardCrafts", "GiveReward", __instance.rewards[0].name);
        ArchipelagoManager.CollectLocationByName(__instance.rewards[0].name);
        return false;
        // TODO: UI for what item you picked up
    }
}