using UnityEngine;

namespace GatorRando.questMods;

static class GeneQuestMods
{
    public static void Edits()
    {
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        //Edits to Gene's Quest
        GameObject economist_quest = prep_subquests.Find("Economist").gameObject;
        QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();
        // Need to remove Loot Get Sequence from economist_quest_qs.states[2].onProgress()
        economist_quest_qs.states[2].onProgress.ObliteratePersistentListenerByIndex(0);

        if (ArchipelagoManager.ItemIsUnlocked("Cheese Sandwich"))
        {
            UnlockedCheeseSandwich();
        }
    }

    public static void UnlockedCheeseSandwich()
    {
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        GameObject economist_quest = prep_subquests.Find("Economist").gameObject;
        QuestStates economist_quest_qs = economist_quest.GetComponent<QuestStates>();

        LSDestroy ls_destroy = economist_quest_qs.GetComponent<LSDestroy>();
        if (economist_quest_qs.StateID == 1 && !ls_destroy.enabled)
        {
            economist_quest_qs.JustProgressState();
        }
        else
        {
            GameObject loot_seq = economist_quest.transform.Find("Loot Get Sequence").gameObject;
            DialogueSequencer loot_sequencer = loot_seq.GetComponent<DialogueSequencer>();
            loot_sequencer.afterSequence.AddListener(economist_quest_qs.JustProgressState);
        }
    }

}