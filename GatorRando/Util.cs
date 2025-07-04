using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GatorRando;

public static class Util
{
    

    public static IEnumerator WaitThenRunCoroutine(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
        yield break;
    }

    public static IEnumerator RunAfterCoroutine(float waitTime, Func<bool> condition, Action action)
    {
        while (true)
        {
            if (condition())
            {
                action();
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(waitTime);
            }
        }
    }

    public static GameObject GetByPath(string path)
    {
        var elements = path.Trim('/').Split('/');
        var activeScene = SceneManager.GetActiveScene();
        var rootObjects = activeScene.GetRootGameObjects();

        var root = rootObjects.First((go) => go.name == elements[0]);
        GameObject current = root;
        foreach (var element in elements.Skip(1))
        {
            current = current.transform.Cast<Transform>()
            .First((t) => t.name == element)
            .gameObject;
        }
        return current;
    }

    public static ItemResource FindItemResourceByName(string name)
    {
        ItemResource[] resources = Resources.FindObjectsOfTypeAll<ItemResource>();
        foreach (ItemResource resource in resources)
        {
            if (resource.name == name)
            {
                return resource;
            }
        }
        return null;
    }
    public static ItemObject FindItemObjectByName(string item)
    {
        ItemObject[] itemObjects = Resources.FindObjectsOfTypeAll<ItemObject>();
        foreach (ItemObject itemObject in itemObjects)
        {
            if (itemObject.name == item)
            {
                return itemObject;
            }
        }
        return null;
    }

    public static ItemObject GenerateItemObject(string name, Sprite sprite)
    {
        ItemObject itemObj = ScriptableObject.CreateInstance<ItemObject>();
        itemObj.id = name;
        itemObj.name = name;
        itemObj.sprite = sprite;
        itemObj.document = ScriptableObject.CreateInstance<MultilingualTextDocument>(); // To avoid a NullReferenceException when speedrun mode is enabled
        return itemObj;
    }
    public static ItemResource GenerateItemResource(string name)
    {
        ItemResource itemRes = ScriptableObject.CreateInstance<ItemResource>();
        itemRes.id = name;
        itemRes.name = name;
        itemRes.itemGetID = name;
        itemRes.showItemGet = false;
        itemRes.onAmountChanged = new UnityEvent<int>();
        return itemRes;
    }
    

    public static string FindIntKeyByPrefix(string prefix)
    {
        List<string> keys = [];
        foreach (string key in GameData.g.gameSaveData.ints.Keys)
        {
            if (key.StartsWith(prefix))
            {
                keys.Add(key);
            }
        }
        if (keys.Count > 1)
        {
            Plugin.LogWarn($"{keys.Count} keys were found. Only returning 1st key.");
        }
        else if (keys.Count == 0)
        {
            return "";
        }
        return keys[0];
    }

    public static List<string> FindBoolKeysByPrefix(string prefix)
    {
        List<string> keys = [];
        foreach (string key in GameData.g.gameSaveData.bools.Keys)
        {
            if (key.StartsWith(prefix))
            {
                keys.Add(key.Substring(prefix.Length));
            }
        }
        return keys;
    }


    public static void RemoveIntKeysByPrefix(string prefix)
    {
        List<string> keys = [];
        foreach (string key in GameData.g.gameSaveData.ints.Keys)
        {
            if (key.StartsWith(prefix))
            {
                keys.Add(key);
            }
        }
        foreach (string key in keys)
        {
            GameData.g.gameSaveData.ints.Remove(key);
        }
    }

    private static GameObject[] potPrefabs;

    public static void PopulatePotPrefabs()
    {
        potPrefabs = [.. Resources.FindObjectsOfTypeAll<ParticlePickup>()
                .Where(e => e.name.Equals("Pot Confetti"))
                .Select(e => e.gameObject.transform.parent.gameObject)];
    }

    public static PersistentObjectType GetPersistentObjectType(PersistentObject persistentObject) => persistentObject switch
    {
        BreakableObjectMulti _ => PersistentObjectType.Chest,
        Racetrack _ => PersistentObjectType.Race,
        PositionChallenge _ => PersistentObjectType.Challenge,
        TimedBreakables _ => PersistentObjectType.Challenge,
        BreakableObject pot when potPrefabs.Contains(pot.breakingPrefab) => PersistentObjectType.Pot,
        BreakableObject cardboard when !potPrefabs.Contains(cardboard.breakingPrefab) => PersistentObjectType.Cardboard,
        _ => PersistentObjectType.Other,
    };

    public enum PersistentObjectType
    {
        Pot,
        Chest,
        Race,
        Cardboard,
        Challenge,
        Other,
    }

    public static T[] Append<T>(this T[] array, T item)
    {
        if (array == null)
        {
            return [item];
        }
        Array.Resize(ref array, array.Length + 1);
        array[array.Length - 1] = item;

        return array;
    }

}
