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
        ItemResource dummyResource = new()
        {
            id = "Dummy_Resource_Population",
            name = "Dummy Resource Population",
            itemGetID = "Dummy_Pop",
            showItemGet = false,
            onAmountChanged = new UnityEvent<int>()
        };
        __instance.populationResource = dummyResource;
    }
}