using GatorRando.QuestMods;

namespace GatorRando.UIMods;

public static class UIEditMod
{
    public static void ApplyUIEdits()
    {
        //UI Edits
        TutorialUIMods.Edits();
        QuestItems.AddItems();
        InventoryMods.AddQuestItemTab();
        RandoSettingsMenu.CreateNewSettingsMenu();
    }
}