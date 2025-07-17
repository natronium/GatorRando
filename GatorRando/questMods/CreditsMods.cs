using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

static class CreditsMods
{
    public static void Edits()
    {
        GameObject credits = Util.GetByPath("Center (Playground)/Story Sequences/Post-Credits");
        DialogueSequencer creditsSeq = credits.GetComponent<DialogueSequencer>();
        creditsSeq.beforeSequence.RemoveListener(ConnectionManager.SendGoal);
        creditsSeq.beforeSequence.AddListener(ConnectionManager.SendGoal);
    }
}