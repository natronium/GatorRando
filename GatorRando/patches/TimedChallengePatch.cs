using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(TimedChallenge))]
static class TimedChallengePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("StartRace")]
    static bool PreStartRace(TimedChallenge __instance)
    {
        if (__instance is Racetrack)
        {
            return RaceManager.CheckIfRaceAvailable();
        }
        return true;
    }

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