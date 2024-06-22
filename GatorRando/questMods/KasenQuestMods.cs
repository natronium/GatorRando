using UnityEngine;

namespace GatorRando.questMods;

static class KasenQuestMods
{
    public static void Edits()
    {
        GameObject kasen_quest = Util.GetByPath("NorthEast (Canyoney)/SideQuests/FetchVulture");
        QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
        GameObject scooter_pickup = kasen_quest_qs.states[0].stateObjects[0];
        kasen_quest_qs.states[0].stateObjects = kasen_quest_qs.states[0].stateObjects.Remove(scooter_pickup);
        GameObject find = Util.GetByPath("NorthEast (Canyoney)/SideQuests/FetchVulture/find scooter/find");
        DialogueSequencer find_ds = find.GetComponent<DialogueSequencer>();
        find_ds.afterSequence.ObliteratePersistentListenerByIndex(1);
        find_ds.afterSequence.ObliteratePersistentListenerByIndex(0);

        if (ArchipelagoManager.LocationIsCollected("BROKEN WHEELIE THINGY"))
        {
            scooter_pickup.SetActive(false);
        }
        else
        {
            scooter_pickup.SetActive(true);
        }
        if (ArchipelagoManager.ItemIsUnlocked("BROKEN WHEELIE THINGY"))
        {
            UnlockedScooter();
        }
    }

    public static void UnlockedScooter()
    {
        GameObject kasen_quest = Util.GetByPath("NorthEast (Canyoney)/SideQuests/FetchVulture");
        QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
        if (kasen_quest_qs.StateID == 0)
        {
            kasen_quest_qs.JustProgressState();
        }
    }
}