using GatorRando.Archipelago;

namespace GatorRando.QuestMods;

public static class QuestEditMod
{
    public static void ApplyQuestEdits()
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
            TutorialQuestMods.StartWithFreeplay();
        } 
    }
}