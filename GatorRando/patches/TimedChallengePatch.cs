using GatorRando.Archipelago;
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
                if (Options.GetOptionBool(Options.Option.LockRacesBehindFlag))
                {
                    BubbleManager.QueueBubble("I can run races since I have my Finish Flag!", BubbleManager.BubbleType.Unimportant);
                }
                else
                {
                    BubbleManager.QueueBubble("I'm down for a race any time!", BubbleManager.BubbleType.Unimportant);
                }
                return true;
            }
            else
            {
                BubbleManager.QueueBubble("I need a FINISH FLAG before I can run races...", BubbleManager.BubbleType.Alert);
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
            LocationHandling.CollectLocationByID(__instance.id);
        }
    }
}