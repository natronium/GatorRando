using HarmonyLib;

namespace GatorRando.patches;

[HarmonyPatch(typeof(QuestRewardCrafts))]
static class QuestRewardCraftsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GiveReward")]
    static bool PreGiveReward(QuestRewardCrafts __instance)
    {
        Plugin.LogCheck("QuestRewardCrafts", "GiveReward", __instance.rewards[0].Name);
        ArchipelagoManager.CollectLocationForItem(__instance.rewards[0].Name);
        return false;
        // TODO: UI for what item you picked up
    }
}