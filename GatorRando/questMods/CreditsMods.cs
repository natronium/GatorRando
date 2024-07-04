using UnityEngine;

namespace GatorRando.QuestMods;

static class CreditsMods
{
    public static void Edits()
    {
        GameObject credits = Util.GetByPath("Center (Playground)/Story Sequences/Post-Credits");
        DialogueSequencer creditsSeq = credits.GetComponent<DialogueSequencer>();
        creditsSeq.beforeSequence.RemoveListener(ArchipelagoManager.SendCompletion);
        creditsSeq.beforeSequence.AddListener(ArchipelagoManager.SendCompletion);
    }
}