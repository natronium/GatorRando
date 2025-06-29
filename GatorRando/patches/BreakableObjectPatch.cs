using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BreakableObject))]
static class BreakableObjectPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("CanBeDestroyed")]
    static bool PreCanBeDestroyed(BreakableObject __instance, ref bool __result)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(__instance);
        if (persistentObjectType == Util.PersistentObjectType.Pot)
        {
            if (!PotManager.CheckIfPotBreakable(__instance.id))
            {
                DialogueModifier.GatorBubble(PotManager.GetPotString(__instance.id));
                __result = false;
                return false;
            }
            
        }
        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("Break", [typeof(bool), typeof(Vector3), typeof(bool)])]
    static void PostBreak(BreakableObject __instance, bool fromAttachment, Vector3 velocity, bool isHeavy)
    {
        if (__instance.IsBroken)
        {
            ArchipelagoManager.CollectLocationByID(__instance.id);
        }
    }
}