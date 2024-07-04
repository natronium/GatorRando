using UnityEngine;

namespace GatorRando.QuestMods;

static class EsmeQuestMods
{
    public static void Edits()
    {
        GameObject getIceCream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream");
        DialogueSequencer getIceCreamSeq = getIceCream.GetComponent<DialogueSequencer>();
        getIceCreamSeq.afterSequence.ObliteratePersistentListenerByIndex(0);
        getIceCreamSeq.afterSequence.AddListener(CollectedSorbet);

        ArchipelagoManager.RegisterItemListener("ICE CREAM", UnlockedSorbet);
        ArchipelagoManager.RegisterLocationListener("ICE CREAM", CollectedSorbet);

        GameObject becomeVampire = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Become Vampire");
        DSDialogue vampireHat = becomeVampire.GetComponents<DSDialogue>()[1];
        vampireHat.onStart.ObliteratePersistentListenerByIndex(0);
        vampireHat.onStart.AddListener(() => { ArchipelagoManager.CollectLocationByName("Hat_Vampire"); });
    }

    private static void CollectedSorbet()
    {
        GameObject iceCream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/IceCream");
        iceCream.SetActive(false);
    }

    private static void UnlockedSorbet()
    {
        GameObject vampireQuest = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire");
        QuestStates vampireQuestQS = vampireQuest.GetComponent<QuestStates>();
        if (vampireQuestQS.StateID == 1 && ArchipelagoManager.LocationIsCollected("ICE CREAM"))
        {
            vampireQuestQS.JustProgressState();
        }
        else
        {
            GameObject getIceCream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream");
            DialogueSequencer getIceCreamSeq = getIceCream.GetComponent<DialogueSequencer>();
            getIceCreamSeq.afterSequence.RemoveListener(vampireQuestQS.JustProgressState);
            getIceCreamSeq.afterSequence.AddListener(vampireQuestQS.JustProgressState);
        }
    }
}