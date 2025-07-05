using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

static class SusanneQuestMods
{
    public static void Edits()
    {
        //Edits to Susanne's Quest
        // Need to remove QuestState.JustProgressState from Rock Get Sequence
        GameObject rockSeq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence");
        DialogueSequencer rockSequencer = rockSeq.GetComponent<DialogueSequencer>();
        rockSequencer.beforeSequence.ObliteratePersistentListenerByIndex(0);
        rockSequencer.beforeSequence.AddListener(CollectedMagicOre);

        ItemHandling.RegisterItemListener("BEACH ROCK", UnlockedMagicOre);
        LocationHandling.RegisterLocationListener("BEACH ROCK", CollectedMagicOre);
    }

    private static void CollectedMagicOre()
    {
        GameObject engineerQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer");
        GameObject specialRocks = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Special Rocks");
        QuestStates engineerQuestQS = engineerQuest.GetComponent<QuestStates>();
        engineerQuestQS.states[1].stateObjects = engineerQuestQS.states[1].stateObjects.Remove(specialRocks);
        specialRocks.SetActive(false);
    }

    private static void UnlockedMagicOre()
    {
        GameObject engineerQuest = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer");
        QuestStates engineerQuestQS = engineerQuest.GetComponent<QuestStates>();
        if (engineerQuestQS.StateID == 1 && LocationHandling.IsLocationCollected("BEACH ROCK"))
        {
            engineerQuestQS.JustProgressState();
        }
        else
        {
            GameObject rockSeq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence");
            DialogueSequencer rockSequencer = rockSeq.GetComponent<DialogueSequencer>();
            rockSequencer.beforeSequence.RemoveListener(engineerQuestQS.JustProgressState);
            rockSequencer.beforeSequence.AddListener(engineerQuestQS.JustProgressState);
        }
    }
}