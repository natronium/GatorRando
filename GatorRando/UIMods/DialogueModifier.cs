using Archipelago.MultiClient.Net.Models;
using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.UIMods;

// TODO:
// BubbleNotificationManager

public static class DialogueModifier
{
    public static DialogueChunk AddNewDialogueChunk(MultilingualTextDocument document, string dialogueString)
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

    public static void AnnounceLocationCollected(int gatorID)
    {
        ItemInfo itemInfo = LocationHandling.ItemAtLocation(gatorID);
        GatorBubble(GetDialogueStringForItemInfo(itemInfo));
    }

    public static void AnnounceLocationCollected(string gatorName)
    {
        ItemInfo itemInfo = LocationHandling.ItemAtLocation(gatorName);
        GatorBubble(GetDialogueStringForItemInfo(itemInfo));
    }

    public static string GetDialogueStringForItemInfo(ItemInfo itemInfo)
    {
        string dialogueString;
        if (itemInfo.Player.Name == GameData.g.gameSaveData.playerName)
        {
            dialogueString = $"I found my {itemInfo.ItemName}. why was that here??";
        }
        else if (itemInfo.ItemGame == "Lil Gator Game")
        {
            dialogueString = $"I found a {itemInfo.ItemName}, but it's {itemInfo.Player.Name}'s, not mine, I should send it back";
        }
        else
        {
            dialogueString = $"I found {itemInfo.Player.Name}'s {itemInfo.ItemName}";
            // Eventually replace itemSprite with AP logo
        }
        return dialogueString;
    }

    public static string GetItemNameForItemInfo(ItemInfo itemInfo)
    {
        string itemName;
        if (itemInfo.Player.Name == GameData.g.gameSaveData.playerName)
        {
            itemName = itemInfo.ItemName;
        }
        else if (itemInfo.ItemGame == "Lil Gator Game")
        {
            itemName = itemInfo.Player.Name + "'s " + itemInfo.ItemName;
        }
        else
        {
            itemName = itemInfo.Player.Name + "'s " + itemInfo.ItemName;
        }
        return itemName;
    }

    public static Sprite GetSpriteForItemInfo(ItemInfo itemInfo)
    {
        Sprite itemSprite;
        if (itemInfo.ItemGame == "Lil Gator Game")
        {
            if (itemInfo.ItemName.Contains("Craft Stuff") || itemInfo.ItemName.Contains("Friend"))
            {
                itemSprite = Texture2DHandler.GetSpriteForItem(itemInfo.ItemName);
            }
            else
            {
                string clientID = ItemHandling.GetClientIDByAPId(itemInfo.ItemId);
                itemSprite = Texture2DHandler.GetSpriteForItem(clientID);
            }
        }
        else
        {
            itemSprite = Texture2DHandler.GetSpriteForItem("Archipelago");
        }
        return itemSprite;
    }
}