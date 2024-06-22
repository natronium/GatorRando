using UnityEngine;

namespace GatorRando.questMods;

static class JadaQuestMods
{
    public static void Edits()
    {
        GameObject cool_kids_quest = GameObject.Find("Cool Kids Quest");
        Transform cool_kids_subquests = cool_kids_quest.transform.Find("Subquests");
        GameObject boar_quest = cool_kids_subquests.Find("Boar Quest").gameObject;
        QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();
        // Jada: Grass Clippings Section
        // Need to remove OnProgress() delegate
        boar_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        if (ArchipelagoManager.ItemIsUnlocked("CLIPPINGS"))
        {
            UnlockedGrassClippings();
        }
        // Jada: Water Bucket Section
        Transform sprout = boar_quest.transform.Find("Sprout");
        GameObject water_seq = sprout.Find("Water Sequence").gameObject;
        DSDialogue water_dia = water_seq.GetComponents<DSDialogue>()[1];
        //Need to remove give bucket delegate from water_seq.Dialogue.onStart()
        water_dia.onStart.ObliteratePersistentListenerByIndex(2);
    }

    public static void UnlockedGrassClippings()
    {
        GameObject cool_kids_quest = GameObject.Find("Cool Kids Quest");
        Transform cool_kids_subquests = cool_kids_quest.transform.Find("Subquests");
        GameObject boar_quest = cool_kids_subquests.Find("Boar Quest").gameObject;
        QuestStates boar_quest_qs = boar_quest.GetComponent<QuestStates>();

        LogicStateCollectGrass ls_grass = boar_quest_qs.GetComponent<LogicStateCollectGrass>();
        if (boar_quest_qs.StateID == 1 && !ls_grass.enabled)
        {
            boar_quest_qs.JustProgressState();
        }
        else
        {
            GameObject grass_seq = GameObject.Find("Got Enough Grass Sequence");
            DialogueSequencer grass_sequencer = grass_seq.GetComponent<DialogueSequencer>();
            grass_sequencer.afterSequence.AddListener(boar_quest_qs.JustProgressState);
        }
    }
}