using UnityEngine;

namespace GatorRando.QuestMods;

static class TutorialQuestMods
{
    public static void QueueStartWithFreeplay()
    {
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
        act1QuestStates.onStateChange.RemoveListener(Act1QuestHandler);
        act1QuestStates.onStateChange.AddListener(Act1QuestHandler);
        if (act1QuestStates.StateID == 1)
        {
            AdvanceToEndOfTutorial();
        }
        if (act1QuestStates.StateID >= 2)
        {
            ReenableTutorialQuests();
        }
    }

    private static void Act1QuestHandler(int stateID)
    {
        switch (stateID)
        {
            case 0:
                break; // Do nothing on initial cutscene
            case 1:
                AdvanceToEndOfTutorial(); break;
            case 2:
                ReenableTutorialQuests(); EnableFriendsInCutscene(); break;
            case 3:
                ReenableTutorialQuests(); break;
        }
    }

    private static void AdvanceToEndOfTutorial()
    {
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
        if (act1QuestStates.StateID <= 2)
        {
            act1QuestStates.SetState(3);
            // EnableFriendsInCutscene();
            ReenableTutorialQuests();
        }
    }

    private static void EnableFriendsInCutscene()
    {
        // Reenable End object to try to fix Cutscene in Start With Freeplay = true
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
        act1QuestStates.states[2].stateObjects[0].SetActive(true);
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