
using UnityEngine;

namespace GatorRando.questMods;

static class TutorialQuestMods
{

    public static void HandleFreeplay()
    {
        ReenableTutorialQuests();
        GameObject act1 = GameObject.Find("Act 1");
        QuestStates act1_qs = act1.GetComponent<QuestStates>();
        act1_qs.states[0].onDeactivate.AddListener(AdvanceToEndOfTutorial);
        act1_qs.states[3].onActivate.AddListener(ReenableTutorialQuests);
        GameObject manager = GameObject.Find("Managers");
        Game game = manager.GetComponent<Game>();
        game.SetToStory();
    }

    private static void AdvanceToEndOfTutorial()
    {
        GameObject act1 = GameObject.Find("Act 1");
        QuestStates act1qs = act1.GetComponent<QuestStates>();
        if (act1qs.StateID < 2)
        {
            act1qs.ProgressState(2);
            ReenableTutorialQuests();
        }
    }

    private static void ReenableTutorialQuests()
    {
        GameObject act1 = GameObject.Find("Act 1");
        LSQuests act1lsq = act1.GetComponent<LSQuests>();
        act1lsq.enabled = true;
        Transform act1questsTransform = act1.transform.Find("Quests");
        GameObject act1quests = act1questsTransform.gameObject;
        act1quests.SetActive(true);
    }

}