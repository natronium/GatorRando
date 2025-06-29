using UnityEngine;

namespace GatorRando.UIMods;

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
}

            // foreach (DialogueChunk dialogueChunk in __instance.document.chunks)
            // {
            //     Plugin.LogDebug(dialogueChunk.name);
            //     foreach (DialogueLine dialogueLine in dialogueChunk.lines)
            //     {
            //         // Plugin.LogDebug(dialogueLine.actorIndex.ToString());
            //         Plugin.LogDebug(dialogueLine.english[0].ToString());
            //         Plugin.LogDebug(dialogueLine.state.ToString());
            //     }  
            // }