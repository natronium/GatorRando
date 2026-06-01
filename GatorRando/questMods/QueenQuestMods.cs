using GatorRando.UIMods;
using System.Collections;

namespace GatorRando.QuestMods;

internal static class QueenQuestMods
{
    private static readonly string noLetter = "noLetter";
    // Modifies interactions between Jane, Ant Queen, and Lola, Marten Queen
    internal static void SetupQueenDialogue()
    {
        string noLetterDialogue = "i saw you talking to the other queen--do you have anything for me?";
        DialogueModifier.AddNewMultiLineDialogueChunk(noLetter, [noLetterDialogue],[1],[0]);
    }

    internal static void LolaNoLetter()
    {
        DialogueActor dialogueActor = Util.GetByPath("Side Content/Side Quests/Queen Beef/Marten").GetComponent<DialogueActor>();
        DSDialogue dSDialogue = new()
        {
            dialogue = noLetter,
            actors = [dialogueActor],
        };
        dialogueActor.StartCoroutine(RunDialogue(dSDialogue));
    }

    internal static void JaneNoLetter()
    {
        DialogueActor dialogueActor = Util.GetByPath("Side Content/Side Quests/Queen Beef/AntQueen").GetComponent<DialogueActor>();
        DSDialogue dSDialogue = new()
        {
            dialogue = noLetter,
            actors = [dialogueActor],
        };
        dialogueActor.StartCoroutine(RunDialogue(dSDialogue));
    }

    private static IEnumerator RunDialogue(DSDialogue dSDialogue)
    {
        yield return dSDialogue.Run();
    }
}