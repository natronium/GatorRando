using System.Collections.Generic;
using System.Reflection.Emit;
using Archipelago.MultiClient.Net.Models;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(JunkShop))]
static class JunkShopPatch
{

    static readonly Dictionary<string, Sprite> replacementSprites = [];
    static readonly Dictionary<string, string> replacementDisplayNames = [];
    static readonly Dictionary<string, DialogueChunk> replacementDialogues = [];
    static Sprite GetSprite(string name) => replacementSprites[name];
    static string GetDisplayName(string name) => replacementDisplayNames[name];
    static DialogueChunk GetDialogueChunk(string name) => replacementDialogues[name];


    [HarmonyPrefix]
    [HarmonyPatch("RunShopDialogueSequence")]
    static void PreRunShopDialogueSequence(JunkShop __instance)
    {
        if (replacementSprites.Keys.Count == 0)
        {
            foreach (JunkShop.ShopItem shopItem in __instance.shopItems)
            {
                ItemInfo itemInfo = ArchipelagoManager.ItemAtLocation(shopItem.item.name);
                if (itemInfo.ItemGame == "Lil Gator Game")
                {
                    if (itemInfo.ItemName.Contains("Craft Stuff") || itemInfo.ItemName.Contains("Friend"))
                    {
                        replacementSprites.Add(shopItem.item.name, Util.GetSpriteForItem(itemInfo.ItemName));
                    }
                    else
                    {
                        string clientID = ArchipelagoManager.GetClientIDByAPId(itemInfo.ItemId);
                        replacementSprites.Add(shopItem.item.name, Util.GetSpriteForItem(clientID));
                    }
                }
                else
                {
                    replacementSprites.Add(shopItem.item.name, Util.GetSpriteForItem("Archipelago"));
                }
                replacementDisplayNames.Add(shopItem.item.name, $"{itemInfo.Player.Name}'s {itemInfo.ItemName}");
                replacementDialogues.Add(shopItem.item.name, DialogueModifier.AddNewDialogueChunk(__instance.document, $"bought {itemInfo.Player.Name}'s {itemInfo.ItemName}"));
            }
        }
    }

    [HarmonyTranspiler]
    [HarmonyPatch("RunShopDialogueSequence", MethodType.Enumerator)]
    static IEnumerable<CodeInstruction> TranspileRunShopDialogueSequence(IEnumerable<CodeInstruction> instructions)
    {
        CodeInstruction nop = new(OpCodes.Nop);
        int counter = 1;

        List<CodeInstruction> secondPatch = [
            CodeInstruction.Call(typeof(UnityEngine.Object), "get_name"),
            CodeInstruction.Call(typeof(JunkShopPatch), nameof(GetSprite), [typeof(string)]),
            new CodeInstruction(OpCodes.Ldloc_S, 7),
            CodeInstruction.LoadField(typeof(JunkShop.ShopItem), nameof(JunkShop.ShopItem.item)),
            CodeInstruction.Call(typeof(UnityEngine.Object), "get_name"),
            CodeInstruction.Call(typeof(JunkShopPatch), nameof(GetDisplayName), [typeof(string)]),
            new CodeInstruction(OpCodes.Ldloc_S, 7),
            CodeInstruction.LoadField(typeof(JunkShop.ShopItem), nameof(JunkShop.ShopItem.item)),
            CodeInstruction.Call(typeof(UnityEngine.Object), "get_name"),
            CodeInstruction.Call(typeof(JunkShopPatch), nameof(GetDialogueChunk), [typeof(string)]),
        ];

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
                // 238 and 239 intentionally untouched (UpdateInventory)
                
                // IL_028E - IL_02AC
                // changing the argument of CoroutineUtil.Start  to:
                //  RunSequence(JunkShopPatch.getSprite(shopItem.item.name), JunkShopPatch.getDisplayName(shopItem.item.name), JunkShopPatch.getReplacementDialogueChunk(shopItem.item.name), this.actors)
                245 => nop,
                246 => nop,
                247 => nop,
                248 => nop,
                249 => nop,
                250 => nop,
                251 => nop,
                252 => nop,
                253 => nop,


                //instead of RunSequence(shopItem.item.sprite, shopItem.item.DisplayName, this.document.FetchChunk(shopItem.unlockChunk), this.actors)
                // 


                _ => instruction
            };
            /// TODO: Come in and swap out displayName
            if (counter == 245){
                foreach (CodeInstruction instr in secondPatch)
                {
                    yield return instr;
                }
            }
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

    [HarmonyPrefix]
    [HarmonyPatch("GetChoiceList")]
    static bool GetChoiceList(JunkShop __instance, ref string[] __result)
    {
        __result = new string[__instance.displayedItemCount + 1];
        __result[0] = __instance.document.FetchString(__instance.cancelChoice, Language.Auto);
        for (int i = 0; i < __instance.displayedItemCount; i++)
        {
            ItemInfo itemInfo = ArchipelagoManager.ItemAtLocation(__instance.shopItems[__instance.displayedItems[i]].item.name);
            ///TODO: Hint the items that appear in the list!
            __result[i + 1] = $"{itemInfo.Player.Name}'s {itemInfo.ItemName}";
        }
        return false;
    }
}
