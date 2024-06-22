using UnityEngine;

namespace GatorRando.questMods;

static class SusanneQuestMods
{
    public static void Edits()
    {
        //Edits to Susanne's Quest
        // Need to remove QuestState.JustProgressState from Rock Get Sequence
        GameObject rock_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence");
        DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
        rock_sequencer.beforeSequence.ObliteratePersistentListenerByIndex(0);
        rock_sequencer.beforeSequence.AddListener(CollectedMagicOre);

        if (ArchipelagoManager.LocationIsCollected("BEACH ROCK"))
        {
            CollectedMagicOre();
        }
        if (ArchipelagoManager.ItemIsUnlocked("BEACH ROCK"))
        {
            UnlockedMagicOre();
        }
    }

    public static void CollectedMagicOre()
    {
        GameObject engineer_quest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer");
        GameObject special_rocks = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Special Rocks");
        QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
        engineer_quest_qs.states[1].stateObjects = engineer_quest_qs.states[1].stateObjects.Remove(special_rocks);
        special_rocks.SetActive(false);
    }

    public static void UnlockedMagicOre()
    {
        GameObject engineer_quest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer");
        QuestStates engineer_quest_qs = engineer_quest.GetComponent<QuestStates>();
        GameObject special_rocks = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Special Rocks");
        if (engineer_quest_qs.StateID == 1 && !special_rocks.activeSelf)
        {
            engineer_quest_qs.JustProgressState();
        }
        else
        {
            GameObject rock_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence");
            DialogueSequencer rock_sequencer = rock_seq.GetComponent<DialogueSequencer>();
            rock_sequencer.beforeSequence.AddListener(engineer_quest_qs.JustProgressState);
        }
    }
}