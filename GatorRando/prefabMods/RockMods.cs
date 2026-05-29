using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando.PrefabMods;

internal static class SkippingRockMods
{
    internal static void EditRockLayer()
    {
        // Move the thrown rock projectile to the "StaticOnly" physics layer so that it can pass through the tutorial barrier
        // This does not seem to impact any other rock behaviour (e.g. skipping still works),
        //  and other projectiles are on the same "StaticOnly" layer
        if (SceneManager.GetActiveScene().name == "Island") // Only needed during tutorial
        {
            DSItem rock = Util.GetByPath("West (Forest)/Side Quests/Rock Fox/Rock Fox/Intro").GetComponent<DSItem>();
            rock.item.prefab.GetComponent<ItemRock>().thrownPrefab.layer = LayerMask.NameToLayer("StaticOnly");
        }
    }
}