using UnityEngine;

namespace GatorRando.QuestMods;

static class GeneQuestMods
{
    public static void Edits()
    {
        //Edits to Gene's Quest
        GameObject economist_quest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist");
        QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();
        // Removing Loot Get Sequence from economist_quest_qs.states[2].onProgress()
        economist_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        if (ArchipelagoManager.ItemIsUnlocked("HALF A CHEESE SANDWICH"))
        {
            UnlockedCheeseSandwich();
        }
    }

    public static void UnlockedCheeseSandwich()
    {
        GameObject economist_quest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist");
        QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();

        LSDestroy ls_destroy = economist_quest_qs.GetComponent<LSDestroy>();
        if (economist_quest_qs.StateID == 1 && !ls_destroy.enabled)
        {
            economist_quest_qs.JustProgressState();
        }
        else
        {
            GameObject loot_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence");
            DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
            loot_sequencer.afterSequence.AddListener(economist_quest_qs.JustProgressState);
        }
    }

}