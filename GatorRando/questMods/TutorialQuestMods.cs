using UnityEngine;

namespace GatorRando.QuestMods;

static class TutorialQuestMods
{

    public static void StartWithFreeplay()
    {
        ReenableTutorialQuests();
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
        act1QuestStates.states[0].onDeactivate.RemoveListener(AdvanceToEndOfTutorial);
        act1QuestStates.states[0].onDeactivate.AddListener(AdvanceToEndOfTutorial);
        if (act1QuestStates.StateID < 2)
        {
            AdvanceToEndOfTutorial();
        }
        act1QuestStates.states[3].onActivate.RemoveListener(ReenableTutorialQuests);
        act1QuestStates.states[3].onActivate.AddListener(ReenableTutorialQuests);
        GameObject manager = Util.GetByPath("Managers");
        Game game = manager.GetComponent<Game>();
        game.SetToStory();
    }

    private static void AdvanceToEndOfTutorial()
    {
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
        if (act1QuestStates.StateID < 2)
        {
            act1QuestStates.ProgressState(2);
            ReenableTutorialQuests();
        }
        act1QuestStates.states[2].stateObjects[0].SetActive(true); // Reenable End object to try to fix Cutscene in Start With Freeplay = true
    }

    private static void ReenableTutorialQuests()
    {
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        LSQuests act1LSQuests = act1.GetComponent<LSQuests>();
        act1LSQuests.enabled = true;
        GameObject act1Quests = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests");
        act1Quests.SetActive(true);
    }

}