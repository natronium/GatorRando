using UnityEngine;

namespace GatorRando.questMods;

static class EsmeQuestMods
{
    public static void Edits()
    {
        GameObject get_ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream");
        DialogueSequencer get_ice_cream_seq = get_ice_cream.GetComponent<DialogueSequencer>();
        get_ice_cream_seq.afterSequence.ObliteratePersistentListenerByIndex(0);
        get_ice_cream_seq.afterSequence.AddListener(CollectedSorbet);
        if (ArchipelagoManager.LocationIsCollected("ICE CREAM"))
        {
            CollectedSorbet();
        }
        if (ArchipelagoManager.ItemIsUnlocked("ICE CREAM"))
        {
            UnlockedSorbet();
        }

        GameObject become_vampire = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Become Vampire");
        DSDialogue vampire_hat = become_vampire.GetComponents<DSDialogue>()[1];
        vampire_hat.onStart.ObliteratePersistentListenerByIndex(0);
        vampire_hat.onStart.AddListener(() => { ArchipelagoManager.CollectLocationForItem("Hat_Vampire"); });
    }

    public static void CollectedSorbet()
    {
        GameObject ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/IceCream");
        ice_cream.SetActive(false);
    }

    public static void UnlockedSorbet()
    {
        GameObject vampire_quest = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire");
        QuestStates vampire_quest_qs = vampire_quest.GetComponent<QuestStates>();
        GameObject ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/IceCream");

        if (vampire_quest_qs.StateID == 1 && !ice_cream.activeSelf)
        {
            vampire_quest_qs.JustProgressState();
        }
        else
        {
            GameObject get_ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream");
            DialogueSequencer get_ice_cream_seq = get_ice_cream.GetComponent<DialogueSequencer>();
            get_ice_cream_seq.afterSequence.AddListener(vampire_quest_qs.JustProgressState);
        }
    }
}