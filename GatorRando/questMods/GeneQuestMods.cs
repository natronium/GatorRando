using UnityEngine;

namespace GatorRando.QuestMods;

static class GeneQuestMods
{
    public static void Edits()
    {
        //Edits to Gene's Quest
        GameObject economistQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist");
        QuestStates economistQuestQS = economistQuest.GetComponent<QuestStates>();
        // Removing Loot Get Sequence from economist_quest_qs.states[2].onProgress()
        economistQuestQS.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        ArchipelagoManager.RegisterItemListener("HALF A CHEESE SANDWICH", UnlockedCheeseSandwich);
    }

    private static void UnlockedCheeseSandwich()
    {
        GameObject economistQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist");
        QuestStates economistQuestQS = economistQuest.GetComponent<QuestStates>();
        if (economistQuestQS.StateID == 1 && ArchipelagoManager.IsLocationCollected("HALF A CHEESE SANDWICH"))
        {
            economistQuestQS.JustProgressState();
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