using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UGNPCManager))]
internal static class UGNPCManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(UGNPCManager.RewardNPCs))]
	private static void PreRewardNPCs(UGNPCManager __instance)
    {
        ReplaceItemResources(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(UGNPCManager.RewardNPCsSilently))]
	private static void PreRewardNPCsSilently(UGNPCManager __instance)
    {
        ReplaceItemResources(__instance);
    }

    private static void ReplaceItemResources(UGNPCManager instance)
    {
        if (!instance.friends_forest.GetName().Contains("Dummy"))
        {
            instance.friends_forest = Util.GenerateItemResource("Dummy_Resource_Forest");
        }
        if (!instance.friends_mountain.GetName().Contains("Dummy"))
        {
            instance.friends_mountain = Util.GenerateItemResource("Dummy_Resource_Mountain");
        }
        if (!instance.friends_water.GetName().Contains("Dummy"))
        {
            instance.friends_water = Util.GenerateItemResource("Dummy_Resource_Water");
        }
    }
}