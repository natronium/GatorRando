using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

internal static class JadaQuestMods
{
    //Reference LogicState for additional changes to Jada's Quest
    internal static void Edits()
    {
        GameObject boarQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest");
        QuestStates boarQuestQS = boarQuest.GetComponent<QuestStates>();
        // Jada: Grass Clippings Section
        // Removing OnProgress() delegate (don't run Got Enough Grass Sequence)
        boarQuestQS.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        ItemHandling.RegisterItemListener("CLIPPINGS", UnlockedGrassClippings);
        
        // Jada: Water Bucket Section
        GameObject waterSeq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Sprout/Water Sequence");
        DSDialogue waterDia = waterSeq.GetComponents<DSDialogue>()[1];
        //Removing give bucket delegate from water_seq.Dialogue.onStart()
        waterDia.onStart.ObliteratePersistentListenerByIndex(2);
        // Removing OnProgress() delegate (don't run Got Enough Water Sequence)
        boarQuestQS.states[4].onProgress.ObliteratePersistentListenerByIndex(0);

        ItemHandling.RegisterItemListener("WATER", UnlockedWater);
    }

    private static void UnlockedGrassClippings()
    {
        GameObject boarQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest");
        QuestStates boarQuestQS = boarQuest.GetComponent<QuestStates>();
        if (boarQuestQS.StateID == 1 && LocationHandling.IsLocationCollected("CLIPPINGS"))
        {
            boarQuestQS.JustProgressState();
        }
        else
        {
            GameObject grassSeq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence");
            DialogueSequencer grassSequencer = grassSeq.GetComponent<DialogueSequencer>();
            grassSequencer.afterSequence.RemoveListener(boarQuestQS.JustProgressState);
            grassSequencer.afterSequence.AddListener(boarQuestQS.JustProgressState);
        }
    }

    private static void UnlockedWater()
    {
        GameObject boarQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest");
        QuestStates boarQuestQS = boarQuest.GetComponent<QuestStates>();
        if (boarQuestQS.StateID == 3 && LocationHandling.IsLocationCollected("WATER"))
        {
            boarQuestQS.JustProgressState();
        }
        else
        {
            GameObject waterSeq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence");
            DialogueSequencer waterSequencer = waterSeq.GetComponent<DialogueSequencer>();
            waterSequencer.afterSequence.RemoveListener(boarQuestQS.JustProgressState);
            waterSequencer.afterSequence.AddListener(boarQuestQS.JustProgressState);
        }
    }
}