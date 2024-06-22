using UnityEngine;

namespace GatorRando.questMods;

static class AntoneQuestMods
{
    public static void Edits()
    {
        //Edits to Antone's Quest
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        GameObject entomologist_quest = prep_subquests.Find("Entomologist").gameObject;
        QuestStates entomologist_quest_qs = entomologist_quest.GetComponent<QuestStates>();
        GameObject sneak_seq = entomologist_quest.transform.Find("Sneak up sequence").gameObject;
        entomologist_quest_qs.states[1].stateObjects = entomologist_quest_qs.states[1].stateObjects.Remove(sneak_seq);
        sneak_seq.SetActive(false);
        if (ArchipelagoManager.ItemIsUnlocked("Bug Net (Sword)"))
        {
            UnlockedBugNet();
        }
    }

    public static void UnlockedBugNet()
    {
        GameObject prep_quest = GameObject.Find("Prep Quest");
        Transform prep_subquests = prep_quest.transform.Find("Subquests");
        GameObject entomologist_quest = prep_subquests.Find("Entomologist").gameObject;
        QuestStates entomologist_quest_qs = entomologist_quest.GetComponent<QuestStates>();
        GameObject sneak_seq = entomologist_quest.transform.Find("Sneak up sequence").gameObject;
        entomologist_quest_qs.states[1].stateObjects.Add(sneak_seq);
        if (entomologist_quest_qs.StateID == 1)
        {
            sneak_seq.SetActive(true);
        }
    }
}