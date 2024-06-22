using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(QuestRewardCrafts))]
static class QuestRewardCraftsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GiveReward")]
    static bool PreGiveReward(QuestRewardCrafts __instance)
    {
        ArchipelagoManager.CollectLocationByName(__instance.rewards[0].name);
        return false;
        // TODO: UI for what item you picked up
    }
}