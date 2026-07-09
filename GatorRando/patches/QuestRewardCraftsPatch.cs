using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(QuestRewardCrafts))]
internal static class QuestRewardCraftsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(QuestRewardCrafts.GiveReward))]
	private static bool PreGiveReward(QuestRewardCrafts __instance)
    {
        foreach (ItemObject reward in __instance.rewards)
        {
            LocationHandling.CollectLocationByName(reward.name); //TODO: see if this breaks anything in base game
        }
        return false;
        // TODO: UI for what item you picked up
    }
}