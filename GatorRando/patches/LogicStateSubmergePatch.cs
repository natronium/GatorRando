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
        //Only collect water if have the bucket
        if (ArchipelagoManager.ItemIsUnlocked("BUCKET"))
        {
            Traverse traverse = Traverse.Create(__instance).Field("swimmingCounter");
            int swimmingCounter = traverse.GetValue<int>();
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
            traverse.SetValue(swimmingCounter);
        }
        return false;
    }
}