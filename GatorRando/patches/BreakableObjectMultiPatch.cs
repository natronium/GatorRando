using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BreakableObjectMulti))]
static class BreakableObjectMultiPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Break", [typeof(bool), typeof(Vector3), typeof(bool)])]
    static bool PreBreak(BreakableObject __instance, bool fromAttachment, Vector3 velocity, bool isSturdy)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(__instance);
        if (persistentObjectType == Util.PersistentObjectType.Chest)
        {
            if (!ChestManager.CheckIfChestBreakable())
            {
                return false;
            }
        }
        return true;
    }

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