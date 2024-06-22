using UnityEngine;

namespace GatorRando.questMods;

static class SusanneQuestMods
{
    public static void Edits()
    {
        //Edits to Susanne's Quest
        // Need to remove QuestState.JustProgressState from Rock Get Sequence
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
        GameObject rock_seq = engineer_quest.transform.Find("Rock Get Sequence").gameObject;
        DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
        rock_sequencer.beforeSequence.ObliteratePersistentListenerByIndex(0);
        rock_sequencer.beforeSequence.AddListener(CollectedMagicOre);

        if (ArchipelagoManager.LocationIsCollected("Magic Ore Pickup"))
        {
            CollectedMagicOre();
        }
        if (ArchipelagoManager.ItemIsUnlocked("Magic Ore"))
        {
            UnlockedMagicOre();
        }
    }

    public static void CollectedMagicOre()
    {
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
        GameObject rocks = engineer_quest.transform.Find("Special Rocks").gameObject;
        QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
        engineer_quest_qs.states[1].stateObjects = engineer_quest_qs.states[1].stateObjects.Remove(rocks);
        rocks.SetActive(false);
    }

    public static void UnlockedMagicOre()
    {
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        GameObject engineer_quest = prep_subquests.Find("Engineer").gameObject;
        QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
        GameObject special_rocks = engineer_quest.transform.Find("Special Rocks").gameObject;
        if (engineer_quest_qs.StateID == 1 && !special_rocks.activeSelf)
        {
            engineer_quest_qs.JustProgressState();
        }
        else
        {
            GameObject rock_seq = engineer_quest.transform.Find("Rock Get Sequence").gameObject;
            DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
            rock_sequencer.beforeSequence.AddListener(engineer_quest_qs.JustProgressState);
        }
    }
}