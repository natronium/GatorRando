using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(BraceletShopDialogue))]
static class BraceletShopDialoguePatch
{

    [HarmonyTranspiler]
    [HarmonyPatch("RunShop", MethodType.Enumerator)]
    static IEnumerable<CodeInstruction> TranspileRunShop(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction nop = new(OpCodes.Nop);
        int counter = 1;
        foreach (var instruction in instructions)
        {

            //NB: counter is individual instruction count which is different from the IL_ labels (which are byte-based)
            CodeInstruction newInstruction = counter switch
            {
                //IL_0308-IL_030E optionally noppable
                //IL_030F-IL_0312 noppable
                //  call instance string BraceletShopDialogue::get_SaveID()
                //  call ArchipelagoManager.CollectLocationByName(string)
                234 => nop, // nop the  ItemManager.i dup so our stack is clean
                237 => new(OpCodes.Ldloc_1), // push "this"
                238 => CodeInstruction.Call(typeof(BraceletShopDialogue), "get_SaveID"), // this.get_SaveID
                239 => CodeInstruction.Call(typeof(ArchipelagoManager), nameof(ArchipelagoManager.CollectLocationByName), [typeof(string)]),
                240 => new(OpCodes.Pop), // clean up the bool from CollectLocationByName

                //IL_033A
                //  Replace call to BraceletShopDialogue::DoBraceletGet() with push null
                255 => nop, // Do not push "this" onto the stack
                256 => new(OpCodes.Ldnull), //yield return null

                //IL_0398-IL03A3
                //  call this.CheckIfAllBraceletShops()
                //  brfalse to IL_047B
                289 => new(OpCodes.Ldloc_1), // "this"
                290 => CodeInstruction.Call(typeof(BraceletShopDialogue), nameof(BraceletShopDialogue.CheckIfAllBraceletShops)), // this.CheckIfAllBraceletShops()
                291 => nop,
                292 => new(OpCodes.Brfalse, instruction.operand), //this is the branch

                _ => instruction
            };

            counter++;
            yield return newInstruction;
        }
    }
}