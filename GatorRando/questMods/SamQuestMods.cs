using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GatorRando.QuestMods;

static class SamQuestMods
{
    //Edit Sam's Quest to enable interception of each of the Thrown Pencils as an AP item
    public static void Edits()
    {
        GameObject thrown_pencil_1_seq = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Thrown Pencil 1/Sequence");
        GameObject thrown_pencil_2_seq = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Thrown Pencil 2/Sequence");
        GameObject thrown_pencil_3_seq = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Thrown Pencil 3/Sequence");
        GameObject[] seqs = [thrown_pencil_1_seq, thrown_pencil_2_seq, thrown_pencil_3_seq];
        string[] item_names = ["Thrown_Pencil_1", "Thrown_Pencil_2", "Thrown_Pencil_3"];

        ArchipelagoManager.RegisterItemListener("Thrown_Pencil", UpdateSamState);

        foreach ((GameObject seq, string item_name) in seqs.Zip(item_names, (s, i) => (s, i)))
        {
            DialogueSequencer sequencer = seq.GetComponent<DialogueSequencer>();
            sequencer.afterSequence.ObliteratePersistentListenerByIndex(0);
            DSItem item = seq.GetComponent<DSItem>();
            item.item = Util.GenerateItemObject(item_name, item.item.sprite);
        }

        UpdateSamState();
    }

    private static void UpdateSamState()
    {
        GameObject jackal = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Jackal");
        ThrowObjectQuest quest = jackal.GetComponent<ThrowObjectQuest>();

        if (ArchipelagoManager.LocationIsCollected("Thrown_Pencil_1") && ArchipelagoManager.LocationIsCollected("Thrown_Pencil_2") &&
            ArchipelagoManager.LocationIsCollected("Thrown_Pencil_3") && ArchipelagoManager.GetItemUnlockCount("Thrown_Pencil") == 3)
        {
            quest.State = 3;
            quest.chunks[0].IsItemFetched = true;
            quest.chunks[1].IsItemFetched = true;
            quest.chunks[2].IsItemFetched = true;
        }
        else if (ArchipelagoManager.LocationIsCollected("Thrown_Pencil_1") && ArchipelagoManager.LocationIsCollected("Thrown_Pencil_2") &&
             ArchipelagoManager.GetItemUnlockCount("Thrown_Pencil") >=2)
        {
            quest.State = 2;
            quest.chunks[0].IsItemFetched = true;
            quest.chunks[1].IsItemFetched = true;
        }
        else if (ArchipelagoManager.LocationIsCollected("Thrown_Pencil_1") && ArchipelagoManager.ItemIsUnlocked("Thrown_Pencil"))
        {
            quest.State = 1;
            quest.chunks[0].IsItemFetched = true;
        }
    }

}