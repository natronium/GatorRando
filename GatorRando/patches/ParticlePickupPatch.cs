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
        Plugin.LogDebug($"ParticlePickup.Start for {__instance.particleSystem.name}");
        if (__instance.particleSystem.name == "Pot Confetti" || __instance.particleSystem.name == "Confetti")
        {
            ItemResource dummyResource = new()
            {
                id = "Dummy_Resource_particle",
                name = "Dummy Resource Particles",
                itemGetID = "Dummy_Part",
                showItemGet = false,
                onAmountChanged = new UnityEvent<int>()
            };
            __instance.resource = dummyResource;
        }
    }
}