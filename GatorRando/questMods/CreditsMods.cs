using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

internal static class CreditsMods
{
    internal static void Edits()
    {
        GameObject sisFlashbacks = Util.GetByPath("Center (Playground)/Story Sequences/Act3 Sis/Sis Flashbacks");
        DialogueSequencer dialogueSeq = sisFlashbacks.GetComponent<DialogueSequencer>();
        dialogueSeq.beforeSequence.RemoveListener(() => ConnectionManager.SetStoryComplete(false));
        dialogueSeq.beforeSequence.AddListener(() => ConnectionManager.SetStoryComplete(false));
    }
}