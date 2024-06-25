using UnityEngine;

namespace GatorRando.QuestMods;

static class JadaQuestMods
{
    //Reference LogicState for additional changes to Jada's Quest
    public static void Edits()
    {
        GameObject boar_quest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest");
        QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();
        // Jada: Grass Clippings Section
        // Removing OnProgress() delegate (don't run Got Enough Grass Sequence)
        boar_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        ArchipelagoManager.RegisterItemListener("CLIPPINGS", UnlockedGrassClippings);

        if (ArchipelagoManager.ItemIsUnlocked("CLIPPINGS"))
        {
            UnlockedGrassClippings();
        }
        
        // Jada: Water Bucket Section
        GameObject water_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Sprout/Water Sequence");
        DSDialogue water_dia = water_seq.GetComponents<DSDialogue>()[1];
        //Removing give bucket delegate from water_seq.Dialogue.onStart()
        water_dia.onStart.ObliteratePersistentListenerByIndex(2);
        // Removing OnProgress() delegate (don't run Got Enough Water Sequence)
        boar_quest_qs.states[4].onProgress.ObliteratePersistentListenerByIndex(0);

        ArchipelagoManager.RegisterItemListener("WATER", UnlockedWater);

        if (ArchipelagoManager.ItemIsUnlocked("WATER"))
        {
            UnlockedWater();
        }
    }

    private static void UnlockedGrassClippings()
    {
        GameObject boar_quest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest");
        QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();
        if (boar_quest_qs.StateID == 1 && ArchipelagoManager.LocationIsCollected("CLIPPINGS"))
        {
            boar_quest_qs.JustProgressState();
        }
        else
        {
            GameObject grass_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence");
            DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
            grass_sequencer.afterSequence.AddListener(boar_quest_qs.JustProgressState);
        }
    }

    private static void UnlockedWater()
    {
        GameObject boar_quest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest");
        QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();
        if (boar_quest_qs.StateID == 3 && ArchipelagoManager.LocationIsCollected("WATER"))
        {
            boar_quest_qs.JustProgressState();
        }
        else
        {
            GameObject water_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence");
            DialogueSequencer water_sequencer = water_seq.GetComponent<DialogueSequencer>();
            water_sequencer.afterSequence.AddListener(boar_quest_qs.JustProgressState);
        }
    }
}