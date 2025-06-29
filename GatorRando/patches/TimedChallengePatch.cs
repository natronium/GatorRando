using GatorRando.UIMods;
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
            if (RaceManager.CheckIfRaceAvailable())
            {
                if (ArchipelagoManager.GetOptionBool(ArchipelagoManager.Option.LockRacesBehindFlag))
                {
                    DialogueModifier.GatorBubble("I can run races since I have my Finish Flag!");
                }
                else
                {
                    DialogueModifier.GatorBubble("I'm down for a race any time!");
                }
                return true;
            }
            else
            {
                DialogueModifier.GatorBubble("I need a FINISH FLAG before I can run races...");
                return false;
            }
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