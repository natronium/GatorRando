using System;
using UnityEngine;

namespace GatorRando.PrefabMods;

internal static class BalloonMods
{
    internal static void EditBalloonStamina()
    {
        // Edit the minimum stamina to be negative for the balloon and bubble gum so that they appear and instantly pop with no bracelets
        // Allows them to be used as a cardboard destroyer
        DSItem bubblegum = Util.GetByPath("SouthEast (Beach)/Side Quests/Old Man Quest/End Sequence").GetComponent<DSItem>();
        bubblegum.item.prefab.GetComponent<ItemSpawnObject>().minimumStamina = -1;
        DSItem balloon = Util.GetByPath("NorthEast (Canyoney)/SideQuests/Balloon Owl/End Sequence").GetComponent<DSItem>();
        balloon.item.prefab.GetComponent<ItemSpawnObject>().minimumStamina = -1;
        string basePath = "Players/Player/Heroboy/Heroboy/Hips/";
        string[] anchors = ["HipAnchor/", "HipAnchor (R)/"];
        string[] items = ["Bubble Gum Balloon Item(Clone)", "Balloon Item(Clone)"];
        foreach (string anchor in anchors)
        {
            foreach (string item in items)
            {
                try
                {
                    GameObject itemInUse = Util.GetByPath(basePath + anchor + item);
                    itemInUse.GetComponent<ItemSpawnObject>().minimumStamina = -1;
                }
                catch(InvalidOperationException)
                {
                    // Balloon-type item is not at that location, so we don't need to modify it
                }
            }
        }

    }
}