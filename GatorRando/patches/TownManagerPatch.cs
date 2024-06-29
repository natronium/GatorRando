using HarmonyLib;
using UnityEngine.Events;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(TownNPCManager))]
static class TownNPCManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    static void PreAwake(TownNPCManager __instance)
    {
        __instance.populationResource = Util.GenerateItemResource("Dummy_Resource_Population");
    }
}