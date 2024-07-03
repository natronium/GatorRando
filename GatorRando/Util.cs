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
    public static IEnumerator WaitThenRun(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
        yield break;
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

    public static string FindKeyByPrefix(string prefix)
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

    public static void RemoveKeysByPrefix(string prefix)
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
}