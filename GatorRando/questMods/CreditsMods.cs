using UnityEngine;

namespace GatorRando.QuestMods;

static class CreditsMods
{
    public static void Edits()
    {
        GameObject credits = Util.GetByPath("Center (Playground)/Story Sequences/Credits");
        DialogueSequencer credits_seq = credits.GetComponent<DialogueSequencer>();
        credits_seq.beforeSequence.AddListener(ArchipelagoManager.SendCompletion);
    }
}