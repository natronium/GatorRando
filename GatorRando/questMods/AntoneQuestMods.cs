using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

static class AntoneQuestMods
{
    public static void Edits()
    {
        //Edits to Antone's Quest
        GameObject entomologistQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist");
        QuestStates entomologistQuestQS = entomologistQuest.GetComponent<QuestStates>();
        GameObject sneakSeq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist/Sneak up sequence");
        entomologistQuestQS.states[1].stateObjects = entomologistQuestQS.states[1].stateObjects.Remove(sneakSeq);
        sneakSeq.SetActive(false);

        ItemHandling.RegisterItemListener("Sword_Net", UnlockedBugNet);
    }

    private static void UnlockedBugNet()
    {
        GameObject entomologistQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist");
        QuestStates entomologistQuestQS = entomologistQuest.GetComponent<QuestStates>();
        GameObject sneakSeq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Entomologist/Sneak up sequence");
        entomologistQuestQS.states[1].stateObjects = entomologistQuestQS.states[1].stateObjects.Add(sneakSeq);
        if (entomologistQuestQS.StateID == 1)
        {
            sneakSeq.SetActive(true);
        }
    }
}