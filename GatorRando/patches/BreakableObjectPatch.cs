using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BreakableObject))]
static class BreakableObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(BreakableObject.CanBeDestroyed))]
    static bool PreCanBeDestroyed(BreakableObject __instance, ref bool __result)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(__instance);
        if (persistentObjectType == Util.PersistentObjectType.Pot)
        {
            if (!PotManager.CheckIfPotBreakable(__instance.id))
            {
                BubbleManager.QueueBubble(PotManager.GetPotString(__instance.id), BubbleManager.BubbleType.Alert);
                __result = false;
                return false;
            }
            else
            {
                BubbleManager.UnimportantMessageType unimportantMessageType = PotManager.GetPotUnimportantMessageType(__instance.id);
                BubbleManager.QueueUnimportantBubble(PotManager.GetPotString(__instance.id), unimportantMessageType);
            }
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(BreakableObject.Break), [typeof(bool), typeof(Vector3), typeof(bool)])]
    static void PostBreak(BreakableObject __instance, bool fromAttachment, Vector3 velocity, bool isHeavy)
    {
        if (__instance.IsBroken)
        {
            LocationHandling.CollectLocationByID(__instance.id);
        }
    }
}