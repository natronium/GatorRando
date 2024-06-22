using UnityEngine;

namespace GatorRando.questMods;

static class MartinQuestMods
{
    public static void Edits()
    {
        GameObject get_pot_lid = GameObject.Find("Get Pot Lid");
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
        // GameObject pot_pickup = GetGameObjectByPath("Act 1/Quests/Martin Quest/Pickup");
        GameObject act1 = GameObject.Find("Act 1");
        Transform act1_quests = act1.transform.Find("Quests");
        GameObject martin_quest = act1_quests.Find("Martin Quest").gameObject;
        QuestStates martin_quest_qs = martin_quest.GetComponent<QuestStates>();
        GameObject pot_pickup = martin_quest.transform.Find("Pickup").gameObject;

        if (martin_quest_qs.StateID == 1 && !pot_pickup.activeSelf)
        {
            martin_quest_qs.JustProgressState();
        }
        else
        {
            GameObject get_pot_lid = GameObject.Find("Get Pot Lid");
            DialogueSequencer get_pot_sequence = get_pot_lid.GetComponent<DialogueSequencer>();
            get_pot_sequence.beforeSequence.AddListener(martin_quest_qs.JustProgressState);
        }
    }

    public static void CollectedPot()
    {
        GameObject act1 = GameObject.Find("Act 1");
        Transform act1_quests = act1.transform.Find("Quests");
        GameObject martin_quest = act1_quests.Find("Martin Quest").gameObject;
        QuestStates martin_quest_qs = martin_quest.GetComponent<QuestStates>();
        GameObject pot_pickup = martin_quest.transform.Find("Pickup").gameObject;
        martin_quest_qs.states[2].stateObjects = martin_quest_qs.states[2].stateObjects.Remove(pot_pickup);
        pot_pickup.SetActive(false);
    }
}