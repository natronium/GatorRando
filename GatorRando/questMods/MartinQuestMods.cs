using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

static class MartinQuestMods
{
    public static void Edits()
    {
        GameObject getPotLid = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid");
        DialogueSequencer getPotSequence = getPotLid.GetComponent<DialogueSequencer>();
        getPotSequence.beforeSequence.ObliteratePersistentListenerByIndex(0);
        getPotSequence.beforeSequence.AddListener(CollectedPot);

        ItemHandling.RegisterItemListener("POT?", UnlockedPot);
        LocationHandling.RegisterLocationListener("POT?", CollectedPot);
    }

    private static void UnlockedPot()
    {
        GameObject martinQuest = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest");
        QuestStates martinQuestQS = martinQuest.GetComponent<QuestStates>();
        if (martinQuestQS.StateID == 1 && LocationHandling.IsLocationCollected("POT?"))
        {
            martinQuestQS.JustProgressState();
        }
        else
        {
            GameObject getPotLid = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid");
            DialogueSequencer getPotSequence = getPotLid.GetComponent<DialogueSequencer>();
            getPotSequence.beforeSequence.RemoveListener(martinQuestQS.JustProgressState);
            getPotSequence.beforeSequence.AddListener(martinQuestQS.JustProgressState);
        }
    }

    private static void CollectedPot()
    {
        GameObject martinQuest = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest");
        QuestStates martinQuestQS = martinQuest.GetComponent<QuestStates>();
        GameObject potPickup = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Pickup");
        martinQuestQS.states[2].stateObjects = martinQuestQS.states[2].stateObjects.Remove(potPickup);
        potPickup.SetActive(false);
    }
}