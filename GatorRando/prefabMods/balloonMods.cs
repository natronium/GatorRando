using GatorRando.Patches;
using System;
using System.Collections;
using UnityEngine;

namespace GatorRando.PrefabMods;

internal static class BalloonMods
{
    internal static float floatTimer;
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
    
    internal static IEnumerator Floating()
    {
        DSItem floater;
        float choice = UnityEngine.Random.value;
        if (choice >= 0.5)
        {
            floater = Util.GetByPath("NorthEast (Canyoney)/SideQuests/Balloon Owl/End Sequence").GetComponent<DSItem>();
        }
        else
        {
            floater = Util.GetByPath("SouthEast (Beach)/Side Quests/Old Man Quest/End Sequence").GetComponent<DSItem>();
        }
        RagdollControllerPatch.floatTrap = true;

        GameObject originalPrefab = floater.item.prefab.GetComponent<ItemSpawnObject>().spawnedObjectPrefab;
        GameObject spawnedObject = UnityEngine.Object.Instantiate(originalPrefab, Player.RawPosition + Player.transform.rotation * new Vector3(-.25f, 1.5f, 0.25f), Player.transform.rotation);
        spawnedObject.GetComponent<StaminaDrainItem>().drainSpeed = 0;
        Player.movement.isModified = true;
        Player.movement.Ragdoll();
        Player.movement.modItemRule = PlayerMovement.ModRule.Locked;
        Player.movement.modJumpRule = PlayerMovement.ModRule.Locked;
        Player.movement.modPrimaryRule = PlayerMovement.ModRule.Locked;
        Player.movement.modSecondaryRule = PlayerMovement.ModRule.Locked;

        // Lower timer
		while (floatTimer > 0)
		{
			floatTimer -= Time.deltaTime;
			yield return null;
		}

		// At this point, timer has ran out

		floatTimer = 0f;

        UnityEngine.Object.Destroy(spawnedObject);
        Player.movement.ClearMods();
        Player.movement.modItemRule = PlayerMovement.ModRule.Allowed;
        Player.movement.modJumpRule = PlayerMovement.ModRule.Allowed;
        Player.movement.modPrimaryRule = PlayerMovement.ModRule.Allowed;
        Player.movement.modSecondaryRule = PlayerMovement.ModRule.Allowed;
        RagdollControllerPatch.floatTrap = false;
    }

}