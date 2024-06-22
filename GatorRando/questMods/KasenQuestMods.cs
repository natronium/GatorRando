using UnityEngine;

namespace GatorRando.questMods;

static class KasenQuestMods
{
    public static void Edits()
    {
        GameObject kasen_quest = GameObject.Find("FetchVulture");
        QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
        GameObject scooter_pickup = kasen_quest_qs.states[0].stateObjects[0];
        kasen_quest_qs.states[0].stateObjects = kasen_quest_qs.states[0].stateObjects.Remove(scooter_pickup);
        GameObject find = kasen_quest.transform.Find("find scooter").Find("find").gameObject;
        DialogueSequencer find_ds = find.GetComponent<DialogueSequencer>();
        find_ds.afterSequence.ObliteratePersistentListenerByIndex(1);
        find_ds.afterSequence.ObliteratePersistentListenerByIndex(0);

        if (ArchipelagoManager.LocationIsCollected("Scooter Pickup"))
        {
            scooter_pickup.SetActive(false);
        }
        else
        {
            scooter_pickup.SetActive(true);
        }
        if (ArchipelagoManager.ItemIsUnlocked("Broken Scooter"))
        {
            UnlockedScooter();
        }
    }

    public static void UnlockedScooter()
    {
        GameObject northeast = GameObject.Find("NorthEast (Canyoney)");
        GameObject kasen_quest = northeast.transform.Find("SideQuests").Find("FetchVulture").gameObject;
        QuestStates kasen_quest_qs = kasen_quest.GetComponent<QuestStates>();
        if (kasen_quest_qs.StateID == 0)
        {
            kasen_quest_qs.JustProgressState();
        }
    }
}