using System;
using System.Collections.Generic;
using System.Linq;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(ItemSearchObjects))]
internal static class ItemSearchObjectsPatch
{
    private static readonly List<PersistentObject> tannerPots = [];
    [HarmonyPostfix]
    [HarmonyPatch(nameof(ItemSearchObjects.GetList))]
	private static void PostGetList(ref PersistentObject[] __result)
    {
        __result = [.. __result, .. TannerPots()];
        __result = RandoSettingsMenu.GetCheckfinderBehavior() switch
        {
            RandoSettingsMenu.CheckfinderBehavior.Logic => Array.FindAll(__result, LocationAccessibilty.IsLocationAccessible),
            RandoSettingsMenu.CheckfinderBehavior.ChecksOnly => Array.FindAll(__result, LocationAccessibilty.IsLocationACheck),
            RandoSettingsMenu.CheckfinderBehavior.Original => __result,
            _ => throw new Exception("Invalid enum value for CheckfinderBehavior"),
        };
    }

	private static List<PersistentObject> TannerPots()
    {
        if (tannerPots.Count == 0) // cache Tanner Pots list
        {
            tannerPots.Add(Util.GetByPath("North (Mountain)/Side Quests/Confused Elk/Intro Objects/Pot (LA) (1)").GetComponent<BreakableObject>());
            tannerPots.Add(Util.GetByPath("North (Mountain)/Side Quests/Confused Elk/Intro Objects/Pot (LA) (2)").GetComponent<BreakableObject>());
            tannerPots.Add(Util.GetByPath("North (Mountain)/Side Quests/Confused Elk/Intro Objects/Pot (LA) (3)").GetComponent<BreakableObject>());
            tannerPots.Add(Util.GetByPath("North (Mountain)/Side Quests/Confused Elk/BreakableObjects/Pot (LA) (1)").GetComponent<BreakableObject>());
            tannerPots.Add(Util.GetByPath("North (Mountain)/Side Quests/Confused Elk/BreakableObjects/Pot (LA) (2)").GetComponent<BreakableObject>());
            tannerPots.Add(Util.GetByPath("North (Mountain)/Side Quests/Confused Elk/BreakableObjects/Pot (LA) (3)").GetComponent<BreakableObject>());
        }
        return tannerPots;
    }
}