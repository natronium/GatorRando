using GatorRando.Archipelago;
using UnityEngine.SceneManagement;

namespace GatorRando.QuestMods;

internal static class QuestEditMod
{
    internal static void ApplyQuestEdits()
    {
        if (SceneManager.GetActiveScene().name == "Island")
        {
            ApplySurfaceQuestEdits();
        }
        else
        {
            ApplyUndergroundQuestEdits();         
        }
    }

    private static void ApplySurfaceQuestEdits()
    {
        //Edits to Martin's Tutorial Quest
        MartinQuestMods.Edits();

        //Edits to Jada's Quest
        JadaQuestMods.Edits();

        //Edits to Prep Quest
        GeneQuestMods.Edits();
        SusanneQuestMods.Edits();
        AntoneQuestMods.Edits();

        //Edits to Esme's Quest
        EsmeQuestMods.Edits();

        //Edits to sidequests
        KasenQuestMods.Edits();
        SamQuestMods.Edits();

        //Goal Completion Edits
        CreditsMods.Edits();

        //Allow Freeplay
        if (Options.GetOptionBool(Options.Option.StartWithFreeplay))
        {
            TutorialQuestMods.QueueStartWithFreeplay();
        }
        else
        {
            // If freeplay is off,
            // queue open underground once tutorial item is complete if DLC is installed
            // TODO: Add check for DLC option in slot data
            TutorialQuestMods.QueueUndergroundOpening();
        }
    }

    private static void ApplyUndergroundQuestEdits()
    {
        Util.GetByPath("Tutorial Region/Act 1 UG/Intro Sequence/Enemies").SetActive(false);
        QueenQuestMods.SetupQueenDialogue();
        Util.GetByPath("Tutorial Region/Act 1 UG/").GetComponent<QuestStates>().ProgressToEnd(); // Drop stone, except the one blocking the entrance
        ITDEndQuestMods.SetUpDLCGoal();
    }
}