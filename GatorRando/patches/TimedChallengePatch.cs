using HarmonyLib;

namespace GatorRando.patches;

[HarmonyPatch(typeof(TimedChallenge))]
static class TimedChallengePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("FinishRace")]
    static void PreFinishRace(TimedChallenge __instance)
    {
        if (__instance is Racetrack)
        {
            ArchipelagoManager.CollectLocationByID(__instance.id);
        }
    }
}