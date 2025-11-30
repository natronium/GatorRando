using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

internal static class GeneQuestMods
{
    internal static void Edits()
    {
        //Edits to Gene's Quest
        GameObject economistQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist");
        QuestStates economistQuestQS = economistQuest.GetComponent<QuestStates>();
        // Removing Loot Get Sequence from economist_quest_qs.states[2].onProgress()
        economistQuestQS.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        ItemHandling.RegisterItemListener("HALF A CHEESE SANDWICH", UnlockedCheeseSandwich);
    }

    private static void UnlockedCheeseSandwich()
    {
        GameObject economistQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist");
        QuestStates economistQuestQS = economistQuest.GetComponent<QuestStates>();
        if (economistQuestQS.StateID == 1 && LocationHandling.IsLocationCollected("HALF A CHEESE SANDWICH"))
        {
            economistQuestQS.JustProgressState();
        }
        else
        {
            if (LocationHandling.IsLocationCollected("HALF A CHEESE SANDWICH"))
            {
                economistQuestQS.states[1].onProgress.AddListener(economistQuestQS.JustProgressState); // If Sandwich collected, skip the sandwich sequence
            }
            else
            {
                GameObject lootSeq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence");
                DialogueSequencer lootSequencer = lootSeq.GetComponent<DialogueSequencer>();
                lootSequencer.afterSequence.RemoveListener(economistQuestQS.JustProgressState);
                lootSequencer.afterSequence.AddListener(economistQuestQS.JustProgressState);
            }
        }
    }

}