using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(RagdollController))]
internal static class RagdollControllerPatch
{
    internal static bool floatTrap = false;
    [HarmonyPrefix]
    [HarmonyPatch(nameof(RagdollController.Update))]
    private static bool PreUpdate()
    {
        // return true;
        return !floatTrap;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(RagdollController.Jump))]
    private static bool PreJump()
    {
        // return true;
        return !floatTrap;
    }

}