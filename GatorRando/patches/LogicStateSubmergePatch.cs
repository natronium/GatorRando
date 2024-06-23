using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(LogicStateSubmerge))]
static class LogicStateSubmergePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("FixedUpdate")]
    static bool PreFixedUpdate(LogicStateSubmerge __instance)
    {
        //TODO: Revise to remove duplicated code--> just move functionality over to the LogicStatePatch instead?
        //TODO: Revise to not run every frame
        //Only collect water if have the bucket
        if (ArchipelagoManager.ItemIsUnlocked("Hat_Bucket"))
        {
            int swimmingCounter = __instance.swimmingCounter;
            if (Player.movement.IsSwimming)
            {
                swimmingCounter++;
            }
            else
            {
                swimmingCounter = 0;
            }
            if (swimmingCounter > 10)
            {
                __instance.LogicCompleted();
            }
            __instance.swimmingCounter = swimmingCounter;
        }
        return false;
    }
}