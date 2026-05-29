using GatorRando.Patches;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GatorRando.PrefabMods;

internal static class BalloonMods
{
    internal static float floatTimer;
    
    internal static IEnumerator Floating()
    {
        GameObject floaterPrefab;
        float choice = UnityEngine.Random.value;
        if (choice >= 0.5)
        {
            floaterPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Player/Physics Items/Bubble Gum Balloon Item.prefab").WaitForCompletion();
        }
        else
        {
            floaterPrefab = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Player/Physics Items/Balloon Item.prefab").WaitForCompletion();
        }
        RagdollControllerPatch.floatTrap = true;

        GameObject originalPrefab = floaterPrefab.GetComponent<ItemSpawnObject>().spawnedObjectPrefab;
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