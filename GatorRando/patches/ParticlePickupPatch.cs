using HarmonyLib;
using UnityEngine.Events;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ParticlePickup))]
static class ParticlePickupPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    static void PreStart(ParticlePickup __instance)
    {
        // Check if Pot Confetti for Pots, Confetti for Chests and Racetracks
        Plugin.LogInfo($"ParticlePickup.Start for {__instance.particleSystem.name}");
        if (__instance.particleSystem.name == "Pot Confetti" || __instance.particleSystem.name == "Confetti")
        {
            __instance.resource = Util.GenerateItemResource("Dummy_Resource_Particle");
        }
    }
}