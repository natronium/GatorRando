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
        GameObject thrownPencil1Seq = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Thrown Pencil 1/Sequence");
        GameObject thrownPencil2Seq = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Thrown Pencil 2/Sequence");
        GameObject thrownPencil3Seq = Util.GetByPath("SouthEast (Beach)/Side Quests/Clumsy Quest/Thrown Pencil 3/Sequence");
        GameObject[] seqs = [thrownPencil1Seq, thrownPencil2Seq, thrownPencil3Seq];
        string[] itemNames = ["Thrown_Pencil_1", "Thrown_Pencil_2", "Thrown_Pencil_3"];

        ArchipelagoManager.RegisterItemListener("Thrown_Pencil", UpdateSamState);

        foreach ((GameObject seq, string itemName) in seqs.Zip(itemNames, (s, i) => (s, i)))
        {
            DialogueSequencer sequencer = seq.GetComponent<DialogueSequencer>();
            sequencer.afterSequence.ObliteratePersistentListenerByIndex(0);
            DSItem item = seq.GetComponent<DSItem>();
            item.item = Util.GenerateItemObject(itemName, item.item.sprite);
        }
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