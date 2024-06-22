using HarmonyLib;
using UnityEngine;

namespace GatorRando.patches;

[HarmonyPatch(typeof(BreakableObject))]
static class BreakableObjectPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Break", [typeof(bool), typeof(Vector3), typeof(bool)])]
    static void PostBreak(BreakableObject __instance, bool fromAttachment, Vector3 velocity, bool isHeavy)
    {
        if (__instance.IsBroken)
        {
            ArchipelagoManager.CollectLocationForBreakableObject(__instance.id, __instance.name);
        }
    }
}