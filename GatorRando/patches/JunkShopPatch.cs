using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(JunkShop))]
static class JunkShopPatch
{
    [HarmonyTranspiler]
    [HarmonyPatch("RunShopDialogueSequence", MethodType.Enumerator)]
    static IEnumerable<CodeInstruction> TranspileRunShopDialogueSequence(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction nop = new(OpCodes.Nop);
        int counter = 1;
        foreach (var instruction in instructions)
        {

            //NB: counter is individual instruction count which is different from the IL_ labels (which are byte-based)
            CodeInstruction newInstruction = counter switch
            {
                //IL_0262 - IL_0275
                //  ArchipelagoManager.CollectLocationByName(shopItem.item.name);
                //  QuestMods.Junk4TrashQuestMods.HideCollectedShopLocations();
                231 => CodeInstruction.Call(typeof(UnityEngine.Object), "get_name"),
                232 => CodeInstruction.Call(typeof(ArchipelagoManager), nameof(ArchipelagoManager.CollectLocationByName), [typeof(string)]),
                233 => new(OpCodes.Pop),
                234 => CodeInstruction.Call(typeof(QuestMods.Junk4TrashQuestMods), nameof(QuestMods.Junk4TrashQuestMods.HideCollectedShopLocations)),
                235 => nop,
                236 => nop,
                237 => nop,

                _ => instruction
            };

            counter++;
            yield return newInstruction;
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch("UpdateInventory")]
    static IEnumerable<CodeInstruction> TranspileUpdateInventory(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction nop = new(OpCodes.Nop);
        int counter = 1;
        foreach (var instruction in instructions)
        {

            //NB: counter is individual instruction count which is different from the IL_ labels (which are byte-based)
            CodeInstruction newInstruction = counter switch
            {
                //IL_005A - IL_0097
                //  ignore the "!this.shopItems[k].item.IsUnlocked" check
                53 => new(OpCodes.Pop), //IL_0083 Minimal change to bypass one condition in a && chain

                _ => instruction
            };

            counter++;
            yield return newInstruction;
        }
    }
}
