using System;
using System.Collections;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GatorRando
{
    public static class Util
    {
        public static GameObject GetByPath(string path)
        {
            var elements = path.Trim('/').Split('/');
            var activeScene = SceneManager.GetActiveScene();
            var rootObjects = activeScene.GetRootGameObjects();

            var root = rootObjects.First((go) => go.name == elements[0]);
            GameObject current = root;
            foreach (var element in elements.Skip(1))
            {
                current = current.GetComponentsInChildren<Transform>(true)
                    .First((t) => t.name == element)
                    .gameObject;
            }
            return current;
        }
    }
}