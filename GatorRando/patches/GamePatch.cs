using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(Game))]
internal static class GamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game.SetWorldState), [typeof(WorldState), typeof(bool), typeof(bool)])]
	private static void PreSetWorldState(WorldState newWorldState)
    {
        if (newWorldState == WorldState.Flashback & RandoSettingsMenu.GetBoolRandoSetting(RandoSettingsMenu.BoolRandoSetting.SkipFinalSequences))
        {
            ConnectionManager.SendGoal(); // TODO: Send player out of flashback and back to tutorial island
        }
    }
}