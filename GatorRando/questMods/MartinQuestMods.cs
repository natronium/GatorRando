using UnityEngine;

namespace GatorRando.QuestMods;

static class MartinQuestMods
{
    public static void Edits()
    {
        GameObject get_pot_lid = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid");
        DialogueSequencer get_pot_sequence = get_pot_lid.GetComponent<DialogueSequencer>();
        get_pot_sequence.beforeSequence.ObliteratePersistentListenerByIndex(0);
        get_pot_sequence.beforeSequence.AddListener(CollectedPot);

        if (ArchipelagoManager.LocationIsCollected("POT?"))
        {
            CollectedPot();
        }
        if (ArchipelagoManager.ItemIsUnlocked("POT?"))
        {
            UnlockedPot();
        }
    }

    public static void UnlockedPot()
    {
        GameObject pot_pickup = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Pickup");
        GameObject martin_quest = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest");
        QuestStates martin_quest_qs = martin_quest.GetComponent<QuestStates>();

        if (martin_quest_qs.StateID == 1 && !pot_pickup.activeSelf)
        {
            martin_quest_qs.JustProgressState();
        }
        else
        {
            GameObject get_pot_lid = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid");
            DialogueSequencer get_pot_sequence = get_pot_lid.GetComponent<DialogueSequencer>();
            get_pot_sequence.beforeSequence.AddListener(martin_quest_qs.JustProgressState);
        }
    }

    public static void CollectedPot()
    {
        GameObject martin_quest = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest");
        QuestStates martin_quest_qs = martin_quest.GetComponent<QuestStates>();
        GameObject pot_pickup = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Pickup");
        martin_quest_qs.states[2].stateObjects = martin_quest_qs.states[2].stateObjects.Remove(pot_pickup);
        pot_pickup.SetActive(false);
    }
}