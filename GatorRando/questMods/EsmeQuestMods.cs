using UnityEngine;

namespace GatorRando.questMods;

static class EsmeQuestMods
{
    public static void Edits()
    {
        GameObject theatre_quest = GameObject.Find("Theatre Quest");
        Transform theatre_subquests = theatre_quest.transform.Find("Subquests");
        GameObject vampire_quest = theatre_subquests.Find("Vampire").gameObject;
        GameObject get_ice_cream = vampire_quest.transform.Find("Get Ice Cream").gameObject;
        DialogueSequencer get_ice_cream_seq = get_ice_cream.GetComponent<DialogueSequencer>();
        get_ice_cream_seq.afterSequence.ObliteratePersistentListenerByIndex(0);
        get_ice_cream_seq.afterSequence.AddListener(CollectedSorbet);
        if (ArchipelagoManager.LocationIsCollected("Ice Cream"))
        {
            CollectedSorbet();
        }
        if (ArchipelagoManager.ItemIsUnlocked("Sorbet"))
        {
            UnlockedSorbet();
        }

        GameObject become_vampire = vampire_quest.transform.Find("Become Vampire").gameObject;
        DSDialogue vampire_hat = become_vampire.GetComponents<DSDialogue>()[1];
        vampire_hat.onStart.ObliteratePersistentListenerByIndex(0);
        vampire_hat.onStart.AddListener(() => { ArchipelagoManager.CollectLocationForItem("Hat_Vampire"); });
    }

    public static void CollectedSorbet()
    {
        GameObject theatre_quest = GameObject.Find("Theatre Quest");
        Transform theatre_subquests = theatre_quest.transform.Find("Subquests");
        GameObject vampire_quest = theatre_subquests.Find("Vampire").gameObject;
        GameObject ice_cream = vampire_quest.transform.Find("IceCream").gameObject;
        ice_cream.SetActive(false);
    }

    public static void UnlockedSorbet()
    {
        GameObject theatre_quest = GameObject.Find("Theatre Quest");
        Transform theatre_subquests = theatre_quest.transform.Find("Subquests");
        GameObject vampire_quest = theatre_subquests.Find("Vampire").gameObject;
        QuestStates vampire_quest_qs = vampire_quest.GetComponent<QuestStates>();
        GameObject ice_cream = vampire_quest.transform.Find("IceCream").gameObject;

        if (vampire_quest_qs.StateID == 1 && !ice_cream.activeSelf)
        {
            vampire_quest_qs.JustProgressState();
        }
        else
        {
            GameObject get_ice_cream = GameObject.Find("Get Ice Cream");
            DialogueSequencer get_ice_cream_seq = get_ice_cream.GetComponent<DialogueSequencer>();
            get_ice_cream_seq.afterSequence.AddListener(vampire_quest_qs.JustProgressState);
        }
    }
}