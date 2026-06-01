using GatorRando.Archipelago;
using GatorRando.QuestMods;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(LogicForkInteraction))]
internal static class LogicForkInteractionPatch
{
    private readonly struct Redirect
    {
		internal readonly string stateName;
        internal readonly string itemName;
        internal readonly Action action;

		internal Redirect(string stateName, string itemName, Action action) : this()
		{
			this.stateName = stateName;
			this.itemName = itemName;
			this.action = action;
		}
	}
    private static readonly List<Redirect> redirects = [
        new("Met Ant, Goto Marten", "First Queen Letter", QueenQuestMods.LolaNoLetter),
        new("Met Marten, Goto Ant", "First Queen Letter", QueenQuestMods.JaneNoLetter),
        new("Return to Ant", "Second Queen Letter", QueenQuestMods.JaneNoLetter),
        new("Return to Marten", "Second Queen Letter", QueenQuestMods.LolaNoLetter),
    ];

    [HarmonyPrefix]
    [HarmonyPatch(nameof(LogicForkInteraction.Interact))]
    private static bool PreInteract(LogicForkInteraction __instance)
    {
        // If the current stateAction for the LogicForkInteraction is in our list of redirects
        // and it is the action that would progress the state, check for the relevant item.
        // If the relevant item has not been received, then redirect the action.
        LogicFork.StateAction stateAction = __instance.stateActions[__instance.stateMachine.StateID];
        string stateName = stateAction.stateName;
        Redirect redirect = redirects.FirstOrDefault(redirect => redirect.stateName == stateName);
        Plugin.LogDebug($"state name: {stateName} matching name: {redirect.stateName}");
        if (stateAction.progressState && redirect.stateName != null)
        {
            if (!ItemHandling.IsItemUnlocked(redirect.itemName))
            {
                redirect.action.Invoke();
                return false;
            }
        }
        return true;
    }
}