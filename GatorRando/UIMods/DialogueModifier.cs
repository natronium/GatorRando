using GatorRando.Archipelago;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GatorRando.UIMods;

public static class DialogueModifier
{
    public static bool inModifiedDialogue = false;
    public static void SetModifiedDialogue(bool modified)
    {
        inModifiedDialogue = modified;
    }
    
    public static DialogueChunk AddNewDialogueChunk(string dialogueString, MultilingualTextDocument document = null)
    {
        DialogueLine newDialogueLine = new()
        {
            actorIndex = 0,
            emote = 0,
            english = [dialogueString],
            position = 0,
            holdEmote = false,
            cue = false,
            noInput = false,
            lookTarget = 0,
            state = -1,
        };
        DialogueChunk newDialogueChunk = new()
        {
            name = dialogueString,
            id = Animator.StringToHash(dialogueString),
            lines = [newDialogueLine],
            options = [],
            mlOptions = [],
        };
        if (document == null)
        {
            DialogueManager.d.chunkDic[dialogueString] = newDialogueChunk;
        }
        else
        {
            document.chunks = document.chunks.Append(newDialogueChunk);
        }
        return newDialogueChunk;
    }

    public static DialogueChunk AddNewMultiLineDialogueChunk(List<string> dialogueStrings, List<int> actors)
    {
        int i = 0;
        List<DialogueLine> dialogueLines = [];
        if (dialogueStrings.Count != actors.Count)
        {
            throw new System.Exception("Mismatched actors and lines");
        }
        foreach (string dialogueString in dialogueStrings)
        {
            dialogueLines.Add(new()
            {
                actorIndex = actors[i],
                emote = 0,
                english = [dialogueString],
                position = 0,
                holdEmote = false,
                cue = false,
                noInput = false,
                lookTarget = 0,
                state = -1,
            });
            i++;
        }
        DialogueChunk newDialogueChunk = new()
        {
            name = dialogueStrings[0],
            id = Animator.StringToHash(dialogueStrings[0]),
            lines = [.. dialogueLines],
            options = [],
            mlOptions = [],
        };
        DialogueManager.d.chunkDic[dialogueStrings[0]] = newDialogueChunk;
        return newDialogueChunk;
    }

    public static void GatorBubble(string dialogueString)
    {
        AddNewDialogueChunk(dialogueString);
        DialogueManager.d.Bubble(dialogueString, null, 0f, false, true, true);
    }

    public static string GetDialogueStringForItemAtLocation(LocationHandling.ItemAtLocation itemAtLocation)
    {
        string dialogueString;
        if (itemAtLocation.itemPlayer == ConnectionManager.SlotName())
        {
            dialogueString = $"I found my {itemAtLocation.itemName}. why was that here??";
        }
        else if (itemAtLocation.itemGame == "Lil Gator Game")
        {
            dialogueString = $"I found a {itemAtLocation.itemName}, but it's {itemAtLocation.itemPlayer}'s, not mine, I should send it back";
        }
        else
        {
            dialogueString = $"I found {itemAtLocation.itemPlayer}'s {itemAtLocation.itemName}";
        }
        return dialogueString;
    }

    public static string GetItemNameForItemAtLocation(LocationHandling.ItemAtLocation itemAtLocation)
    {
        string itemName;
        if (itemAtLocation.itemPlayer == ConnectionManager.SlotName())
        {
            itemName = itemAtLocation.itemName;
        }
        else
        {
            itemName = itemAtLocation.itemPlayer + "'s " + itemAtLocation.itemName;
        }
        return itemName;
    }

    public static Sprite GetSpriteForItemAtLocation(LocationHandling.ItemAtLocation itemAtLocation)
    {
        Sprite itemSprite;
        if (itemAtLocation.itemGame == "Lil Gator Game")
        {
            if (itemAtLocation.itemName.Contains("Craft Stuff") || itemAtLocation.itemName.Contains("Friend"))
            {
                itemSprite = SpriteHandler.GetSpriteForItem(itemAtLocation.itemName);
            }
            else
            {
                string clientID = ItemHandling.GetClientIDByAPId(itemAtLocation.itemId);
                itemSprite = SpriteHandler.GetSpriteForItem(clientID);
            }
        }
        else
        {
            itemSprite = SpriteHandler.GetSpriteForItem("Archipelago");
        }
        return itemSprite;
    }

    public static void DialogueTrap()
    {
        DSDialogue trapDialogue;
        BraceletShopDialogue braceletShopDialogue;

		IEnumerator RunDialogueTrap()
        {
            SetModifiedDialogue(true);
            braceletShopDialogue.gameObject.SetActive(true);
            braceletShopDialogue.gameObject.transform.position = Player.RawPosition + Player.transform.rotation * new Vector3(0,0,2);
            braceletShopDialogue.gameObject.transform.rotation = Player.transform.rotation;
            braceletShopDialogue.gameObject.transform.Rotate(new Vector3(0,180,0)); // Face the player
            yield return trapDialogue.Run();
            yield return braceletShopDialogue.Poof();
            SetModifiedDialogue(false);
        }

        List<string> dialogueStrings = ["This is a dialogue trap", "with", "multiple", "lines"];
        // Eventually randomize sets of lines
        AddNewMultiLineDialogueChunk(dialogueStrings, [.. Enumerable.Repeat(1,dialogueStrings.Count)]);
        GameObject monkey = Util.GetByPath("NorthWest (Tutorial Island)/Side/Monkey Quest/Monkey");
        GameObject newMonkey = Object.Instantiate(monkey);
        braceletShopDialogue = newMonkey.GetComponent<BraceletShopDialogue>();
        braceletShopDialogue.onExit = null;
        trapDialogue = new()
        {
            dialogue = dialogueStrings[0],
            actors = [braceletShopDialogue.actors[0]],
        };

        trapDialogue.actors[0].StartCoroutine(RunDialogueTrap());
    }
}