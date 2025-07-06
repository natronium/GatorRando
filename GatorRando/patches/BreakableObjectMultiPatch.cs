using GatorRando.Archipelago;
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
                BubbleManager.QueueBubble("I need a KEY before I can unlock chests...", BubbleManager.BubbleType.Alert);
                return false;
            }
            else
            {
                if (Options.GetOptionBool(Options.Option.LockChestsBehindKey))
                {
                    BubbleManager.QueueBubble("Keys are reusable, so chests are easy to open!", BubbleManager.BubbleType.Unimportant);
                }
                else
                {
                    BubbleManager.QueueBubble("Who needs keys when I can smash things!", BubbleManager.BubbleType.Unimportant);
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
            LocationHandling.CollectLocationByID(__instance.id);
        }
    }
}