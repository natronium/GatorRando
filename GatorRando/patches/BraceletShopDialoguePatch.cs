using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace GatorRando.patches;

[HarmonyPatch(typeof(BraceletShopDialogue))]
static class BraceletShopDialoguePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Interact")]
    static bool PreInteract(BraceletShopDialogue __instance)
    {
        CoroutineUtil.Start(RunShopModified(__instance));
        return false;
    }

    private static IEnumerator RunShopModified(BraceletShopDialogue bsd)
    {
        //TODO: Revise to remove duplicated code
        Game.DialogueDepth++;
        bsd.state = GameData.g.ReadInt(bsd.SaveID, 0);
        bsd.itemResource.ForceShow = true;
        int price = bsd.price;
        MultilingualTextDocument.SetPlaceholder(0, price.ToString("0"));
        yield return bsd.LoadDialogue(bsd.promptDialogue);
        bsd.itemResource.ForceShow = false;
        if (DialogueManager.optionChosen == 1)
        {
            if (bsd.itemResource.Amount >= price)
            {
                bsd.itemResource.Amount -= price;
                yield return bsd.LoadDialogue(bsd.purchaseDialogue);
                bsd.itemResource.ForceShow = false;
                Player.movement.Stamina = 0f;
                ArchipelagoManager.CollectLocationByName(bsd.SaveID);
                yield return null;
                // Player.itemManager.Refresh();
                // yield return bsd.DoBraceletGet(); //TODO: OVERWRITE with appropriate UI for archipelago item
                bsd.state++;
                GameData.g.Write(bsd.SaveID, bsd.state);
                if (bsd.state >= bsd.braceletsInStock)
                {
                    bsd.actors[0].showNpcMarker = false;
                    if (bsd.CheckIfAllBraceletShops())
                    {
                        Game.DialogueDepth++;
                        yield return bsd.LoadDialogue(bsd.allPurchased);
                        yield return bsd.StartCoroutine(bsd.Poof());
                        yield return new WaitForSeconds(1.5f);
                        yield return CoroutineUtil.Start(DialogueManager.d.LoadChunk(bsd.document.FetchChunk(bsd.afterAllPurchased), null, DialogueManager.DialogueBoxBackground.Standard, true, true, false, false));
                        bsd.rewardNPC.GiveReward();
                        Game.DialogueDepth--;
                    }
                    else
                    {
                        yield return bsd.LoadDialogue(bsd.leaveDialogue);
                        yield return bsd.StartCoroutine(bsd.Poof());
                    }
                }
            }
            else
            {
                yield return bsd.LoadDialogue(bsd.notEnoughDialogue);
            }
        }
        else
        {
            yield return bsd.LoadDialogue(bsd.noPurchaseDialogue);
        }
        bsd.itemResource.ForceShow = false;
        GameData.g.Write(bsd.SaveID, bsd.state);
        Game.DialogueDepth--;
        yield break;
    }
}