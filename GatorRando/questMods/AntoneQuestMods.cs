using UnityEngine;

namespace GatorRando.QuestMods;

static class AntoneQuestMods
{
    public static void Edits()
    {
        //Edits to Antone's Quest
        GameObject entomologist_quest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist");
        QuestStates entomologist_quest_qs = entomologist_quest.GetComponent<QuestStates>();
        GameObject sneak_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist/Sneak up sequence");
        entomologist_quest_qs.states[1].stateObjects = entomologist_quest_qs.states[1].stateObjects.Remove(sneak_seq);
        sneak_seq.SetActive(false);

        ArchipelagoManager.RegisterItemListener("Sword_Net", UnlockedBugNet);

        if (ArchipelagoManager.ItemIsUnlocked("Sword_Net"))
        {
            UnlockedBugNet();
        }
    }

    private static void UnlockedBugNet()
    {
        GameObject entomologist_quest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist");
        QuestStates entomologist_quest_qs = entomologist_quest.GetComponent<QuestStates>();
        GameObject sneak_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist/Sneak up sequence");
        entomologist_quest_qs.states[1].stateObjects = entomologist_quest_qs.states[1].stateObjects.Add(sneak_seq);
        if (entomologist_quest_qs.StateID == 1)
        {
            sneak_seq.SetActive(true);
        }
    }
}