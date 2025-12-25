using GatorRando.Archipelago;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GatorRando.UIMods;

public static class DialogueModifier
{
    public static bool inModifiedDialogue = false;
    public static bool inTrapDialogue = false;
    public static void SetModifiedDialogue(bool modified)
    {
        inModifiedDialogue = modified;
    }

    public static void SetTrapDialogue(bool trap)
    {
        inTrapDialogue = trap;
    }
    private static DialogueTraps dialogueTraps = null;
    internal static void CleanUp()
    {
        dialogueTraps = null;
        SetTrapDialogue(false);
        SetModifiedDialogue(false);
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

    public static DialogueChunk AddNewMultiLineDialogueChunk(string dialogueName, List<string> dialogueStrings, List<int> actors, List<int> emotes)
    {
        int i = 0;
        List<DialogueLine> dialogueLines = [];
        if (dialogueStrings.Count != actors.Count || emotes.Count != actors.Count)
        {
            throw new System.Exception("Mismatched actors, emotes and/or lines");
        }
        foreach (string dialogueString in dialogueStrings)
        {
            dialogueLines.Add(new()
            {
                actorIndex = actors[i],
                emote = emotes[i],
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
            name = dialogueName,
            id = Animator.StringToHash(dialogueName),
            lines = [.. dialogueLines],
            options = [],
            mlOptions = [],
        };
        DialogueManager.d.chunkDic[dialogueName] = newDialogueChunk;
        return newDialogueChunk;
    }

    public static void AddChoiceQuest(GameObject gameObject, List<DialogueActor> actors, List<ChoiceQuest.Prompt.Choice> choices, string promptText, string rewardText)
    {
        List<string> choiceText = [];
        List<MultilingualString> multilingualStrings = [];
        foreach (ChoiceQuest.Prompt.Choice choice in choices)
        {
            choiceText.Add(choice.response);
            MultilingualString multilingualString = new()
            {
                english = [choice.response],
            };
            multilingualStrings.Add(multilingualString);
        }

        ChoiceQuest.Prompt prompt = new()
        {
            text = promptText,
            choices = [.. choices],
        };
        ChoiceQuest choiceQuest = gameObject.AddComponent<ChoiceQuest>();
        choiceQuest.actors = [.. actors];
        choiceQuest.prompts = [prompt];
        choiceQuest.id = "trap";
        choiceQuest.rewardText = rewardText;
        choiceQuest.onReward = new();

        DialogueLine promptLine = new()
        {
            actorIndex = 1,
            emote = 0,
            english = [promptText],
            position = 0,
            holdEmote = false,
            cue = false,
            noInput = false,
            lookTarget = 0,
            state = -1,
        };

        DialogueChunk promptChunk = new()
        {
            name = promptText,
            id = Animator.StringToHash(promptText),
            lines = [promptLine],
            options = [.. choiceText],
            mlOptions = [.. multilingualStrings],
        };
        DialogueManager.d.chunkDic[promptText] = promptChunk;

        DialogueLine rewardLine = new()
        {
            actorIndex = 1,
            emote = 0,
            english = [rewardText],
            position = 0,
            holdEmote = false,
            cue = false,
            noInput = false,
            lookTarget = 0,
            state = -1,
        };
        DialogueChunk rewardChunk = new()
        {
            name = rewardText,
            id = Animator.StringToHash(rewardText),
            lines = [rewardLine],
            options = [],
            mlOptions = [],
        };
        DialogueManager.d.chunkDic[rewardText] = rewardChunk;
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
        dialogueTraps ??= new();

		static IEnumerator RunDialogueTrap()
        {
            int choice = Random.Range(0,3);
            yield return choice switch
            {
                0 => dialogueTraps.RunMonkeyTrap(),
                1 => dialogueTraps.RunCourtroomTrap(),
                2 => dialogueTraps.RunTrishTrap(),
                _ => null, // Shouldn't happen
            };
        }

        Player.actor.StartCoroutine(RunDialogueTrap());
    }
}

// Looking around emote: 485016557 ?