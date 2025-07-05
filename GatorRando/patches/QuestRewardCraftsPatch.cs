using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(QuestRewardCrafts))]
static class QuestRewardCraftsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("GiveReward")]
    static bool PreGiveReward(QuestRewardCrafts __instance)
    {
        LocationHandling.CollectLocationByName(__instance.rewards[0].name);
        return false;
        // TODO: UI for what item you picked up
    }
}