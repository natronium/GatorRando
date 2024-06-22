using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(JunkShop))]
static class JunkShopPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("RunShopDialogue")]
    static bool PreRunShopDialogue(JunkShop __instance)
    {
        CoroutineUtil.Start(RunShopDialogueSequenceModified(__instance));
        return false;
    }

    private static IEnumerator RunShopDialogueSequenceModified(JunkShop js)
    {
        //TODO: Revise to remove duplicated code
        Game.DialogueDepth++;
        js.itemResource.ForceShow = true;
        GameObject[] array = js.shopStateObjects;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(true);
        }
        yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(js.document.FetchChunk(js.shopIntro), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
        DialogueManager.optionChosen = -1;
        CoroutineUtil.Start(DialogueManager.d.RunDialogueOptions(js.GetChoiceList()));
        int selectedOption = 0;
        while (DialogueManager.optionChosen == -1)
        {
            int currentlySelectedIndex = DialogueOptions.CurrentlySelectedIndex;
            for (int j = 0; j < js.cameras.Length; j++)
            {
                js.cameras[j].SetActive(j == currentlySelectedIndex - 1);
            }
            if (currentlySelectedIndex != selectedOption)
            {
                if (currentlySelectedIndex == 0)
                {
                    js.uiItemResource.ClearPrice();
                }
                else
                {
                    js.uiItemResource.SetPrice(js.shopItems[js.displayedItems[currentlySelectedIndex - 1]].cost);
                }
            }
            yield return null;
        }
        array = js.cameras;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(false);
        }
        if (DialogueManager.optionChosen == 0)
        {
            js.uiItemResource.ClearPrice();
            js.itemResource.ForceShow = false;
            yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(js.document.FetchChunk(js.cancelDialogue), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
        }
        else
        {
            int num = DialogueManager.optionChosen - 1;
            JunkShop.ShopItem shopItem = js.shopItems[js.displayedItems[num]];
            if (js.itemResource.Amount >= shopItem.cost)
            {
                js.uiItemResource.ClearPrice();
                js.itemResource.ForceShow = false;
                js.itemResource.Amount -= shopItem.cost;
                ArchipelagoManager.CollectLocationByName(shopItem.item.name);
                shopItem.isHidden = true;
                js.shopItems[js.displayedItems[num]] = shopItem;
                js.UpdateInventory();
                yield return CoroutineUtil.Start(js.itemGet.RunSequence(shopItem.item.sprite, shopItem.item.DisplayName, js.document.FetchChunk(shopItem.unlockChunk), js.actors));
            }
            else
            {
                yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(js.document.FetchChunk(js.notEnoughDialogue), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
                js.uiItemResource.ClearPrice();
                js.itemResource.ForceShow = false;
            }
        }
        array = js.shopStateObjects;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].SetActive(false);
        }
        if (js.displayedItemCount == 0)
        {
            yield return DialogueManager.d.LoadChunk(js.document.FetchChunk(js.soldOutDialogue), js.actors, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false);
            yield return js.stateMachine.ProgressState(-1);
        }
        Game.DialogueDepth--;
        yield break;
    }
}
