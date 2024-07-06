using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class APConnectedUI
{
    public static void ImplementAPConnectionStatusAsQuest()
    {
        // Make it so that quest tracker is visible in settings menu
        GameObject settingsMenu = Util.GetByPath("Canvas/Pause Menu/Settings/");
        settingsMenu.AddComponent<ShowActiveQuestProfile>();

        //
        GameObject coolQuestZoneObject = Util.GetByPath("Terrain/QuestZones/Cool");
        GameObject questZones = Util.GetByPath("Terrain/QuestZones");
        GameObject APQuestZoneObject = GameObject.Instantiate(coolQuestZoneObject, questZones.transform);
        APQuestZoneObject.name = "AP Quest Zone";

        GameObject wolfQuest = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Wolf Quest");
        QuestProfile wolfQuestProfile = wolfQuest.GetComponent<SyncQuestStates>().questProfile;
        QuestProfile APQuestProfile = Object.Instantiate(wolfQuestProfile);
        APQuestProfile.questTitle = "archipelago connection quest";
        APQuestProfile.id = "AP_Connected";
        APQuestProfile.name = "AP Connected";
        APQuestProfile.document = null;
        APQuestProfile.displayOnComplete = true;
        APQuestProfile.priority = 0;
        APQuestProfile.isActiveQuestZone = true;
        APQuestProfile.lastZoneTrigger = Time.realtimeSinceStartup;
        QuestProfile.QuestTask APQuestTask = new()
        {
            name = "Connect to AP server",
            document = null,
            statedTask = "connect to AP server",
            taskState = QuestProfile.QuestTaskState.Visible
        };
        APQuestProfile.tasks = [APQuestTask];

        QuestZone APQuestZone = APQuestZoneObject.GetComponent<QuestZone>();
        APQuestZone.questProfile = APQuestProfile;
    }

    public static void ShowAPQuest()
    {
        QuestProfile APQuestProfile = Util.GetByPath("Terrain/QuestZones/AP Quest Zone").GetComponent<QuestZone>().questProfile;
        QuestTrackerPopup questTrackerPopup = Util.GetByPath("Canvas/Notifications/Quest Tracker").GetComponent<QuestTrackerPopup>();
        questTrackerPopup.DisplayQuest(APQuestProfile);
        questTrackerPopup.hideBehavior.autoHideTime = -1f;
    }

    public static void HideAPQuest()
    {
        // Disable the quest zone for the Connection Status quest
        Util.GetByPath("Terrain/QuestZones/AP Quest Zone").GetComponent<QuestZone>().questProfile.MarkTaskComplete(0);
        Util.GetByPath("Terrain/QuestZones/AP Quest Zone").SetActive(false);
        QuestTrackerPopup questTrackerPopup = Util.GetByPath("Canvas/Notifications/Quest Tracker").GetComponent<QuestTrackerPopup>();
        // questTrackerPopup.DisplayQuest(null);
    }
}