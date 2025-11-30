using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(Game))]
static class GamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Game.SetWorldState), [typeof(WorldState), typeof(bool), typeof(bool)])]
    static void PreSetWorldState(WorldState newWorldState)
    {
        if (newWorldState == WorldState.Flashback & RandoSettingsMenu.IsGoalBeforeEpilogue())
        {
            ConnectionManager.SendGoal();
        }
    }
}