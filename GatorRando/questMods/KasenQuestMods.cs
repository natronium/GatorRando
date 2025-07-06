using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

static class KasenQuestMods
{
    public static void Edits()
    {
        GameObject kasenQuest = Util.GetByPath("NorthEast (Canyoney)/SideQuests/FetchVulture");
        QuestStates kasenQuestQS = kasenQuest.GetComponent<QuestStates>();
        GameObject scooterPickup = kasenQuestQS.states[0].stateObjects[0];
        kasenQuestQS.states[0].stateObjects = kasenQuestQS.states[0].stateObjects.Remove(scooterPickup);
        GameObject find = Util.GetByPath("NorthEast (Canyoney)/SideQuests/FetchVulture/find scooter/find");
        DialogueSequencer findDS = find.GetComponent<DialogueSequencer>();
        findDS.afterSequence.ObliteratePersistentListenerByIndex(1);
        findDS.afterSequence.ObliteratePersistentListenerByIndex(0);
        scooterPickup.SetActive(true);

        ItemHandling.RegisterItemListener("Shield_ScooterBoardGreen", UnlockedScooter);
        LocationHandling.RegisterLocationListener("BROKEN WHEELIE THINGY", () => scooterPickup.SetActive(false));
    }

    private static void UnlockedScooter()
    {
        GameObject kasenQuest = Util.GetByPath("NorthEast (Canyoney)/SideQuests/FetchVulture");
        QuestStates kasenQuestQS = kasenQuest.GetComponent<QuestStates>();
        if (kasenQuestQS.StateID == 0)
        {
            kasenQuestQS.JustProgressState();
        }
    }
}