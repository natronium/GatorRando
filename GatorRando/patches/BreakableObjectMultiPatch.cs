using GatorRando.UIMods;
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
                DialogueModifier.GatorBubble("I need a KEY before I can unlock chests...");
                return false;
            }
            else
            {
                if (ArchipelagoManager.GetOptionBool(ArchipelagoManager.Option.LockChestsBehindKey))
                {
                    DialogueModifier.GatorBubble("Keys are reusable, so chests are easy to open!");
                }
                else
                {
                    DialogueModifier.GatorBubble("Who needs keys when I can smash things!");
                }
                
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