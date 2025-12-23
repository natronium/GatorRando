using GatorRando.Archipelago;
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
        document.chunks = document.chunks.Append(newDialogueChunk);
        return newDialogueChunk;
    }

    public static DialogueChunk AddNewDialogueChunk(string dialogueString)
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
        DialogueManager.d.chunkDic[dialogueString] = newDialogueChunk;
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
}