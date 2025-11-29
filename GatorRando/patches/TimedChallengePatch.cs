using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(TimedChallenge))]
internal static class TimedChallengePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(TimedChallenge.StartRace))]
	private static bool PreStartRace(TimedChallenge __instance)
    {
        if (__instance is Racetrack)
        {
            if (RaceManager.CheckIfRaceAvailable())
            {
                if (Options.GetOptionBool(Options.Option.LockRacesBehindFlag))
                {
                    BubbleManager.QueueUnimportantBubble("I can run races since I have my Finish Flag!", BubbleManager.UnimportantMessageType.Race);
                }
                else
                {
                    BubbleManager.QueueUnimportantBubble("I'm down for a race any time!", BubbleManager.UnimportantMessageType.Race);
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
    [HarmonyPatch(nameof(TimedChallenge.FinishRace))]
	private static void PreFinishRace(TimedChallenge __instance)
    {
        if (__instance is Racetrack)
        {
            LocationHandling.CollectLocationByID(__instance.id);
        }
    }
}