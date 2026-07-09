using GatorRando.Archipelago;
using GatorRando.UIMods;
using UnityEngine;

namespace GatorRando.QuestMods;

internal static class ITDEndQuestMods
{
    internal static void SetUpDLCGoal()
    {
        Util.GetByPath("Tutorial Region/Act 3").GetComponent<QuestStates>().onStateChange.AddListener(ITDEndQuestListener);
    }

    private static void ITDEndQuestListener(int questStateId)
    {
        if (RandoSettingsMenu.GetBoolRandoSetting(RandoSettingsMenu.BoolRandoSetting.SkipFinalSequences))
        {
            if (questStateId == 3)
            {
                ConnectionManager.SetStoryComplete(true);
            }
        }
        else
        {
            if (questStateId == 6)
            {
                ConnectionManager.SetStoryComplete(true);
            }
        }
    }
}