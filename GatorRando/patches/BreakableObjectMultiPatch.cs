using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BreakableObjectMulti))]
static class BreakableObjectMultiPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Break", [typeof(bool), typeof(Vector3), typeof(bool)])]
    static void PostBreak(BreakableObjectMulti __instance, bool fromAttachment, Vector3 velocity, bool isSturdy)
    {
        if (__instance.IsBroken)
        {
            ArchipelagoManager.CollectLocationByID(__instance.id);
        }
    }
}