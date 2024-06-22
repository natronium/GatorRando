﻿using System;
using System.Collections;
using BepInEx;
using GatorRando.questMods;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;

    //TODO: fix to be options from Archipelago
    readonly bool freeplay_from_start = true;

    private void Awake()
    {
        Instance = this;
        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(); // automatically patch based on harmony attributes
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        gameObject.AddComponent<ArchipelagoManager>();
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    static IEnumerator WaitThenRun(float duration, Action action)
    {
        yield return new WaitForSeconds(duration);
        action();
        yield break;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        if (scene.name == "Prologue")
        {
            StartCoroutine(WaitThenRun(0.5f, ArchipelagoManager.Connect));
        }
        if (scene.name == "Island")
        {
            //Allow Freeplay
            if (freeplay_from_start)
            {
                TutorialQuestMods.HandleFreeplay();
            }

            ArchipelagoManager.OnSceneLoad();

            //Edits to Martin's Tutorial Quest
            MartinQuestMods.Edits();

            //Edits to Jada's Quest
            JadaQuestMods.Edits();

            //Edits to Prep Quest
            GeneQuestMods.Edits();
            SusanneQuestMods.Edits();
            AntoneQuestMods.Edits();

            //Edits to Esme's Quest
            EsmeQuestMods.Edits();

            //Edits to sidequests
            KasenQuestMods.Edits();

            Junk4TrashQuestMods.HideCollectedItems();
        }
    }

    public static void LogDebug(String debugMessage)
    {
        Instance.Logger.LogDebug(debugMessage);
    }

    public static void LogWarn(string warningMessage)
    {
        Instance.Logger.LogWarning(warningMessage);
    }

    public static void LogError(string errorMessage)
    {
        Instance.Logger.LogError(errorMessage);
    }

    public static void LogCall(String typeName, String methodName)
    {
        Instance.Logger.LogDebug($"{typeName}.{methodName}!");
        Instance.Logger.LogDebug(new System.Diagnostics.StackTrace().ToString());
    }

    public static void LogCheck(String typeName, String methodName, String checkName)
    {
        Instance.Logger.LogDebug($"{typeName}.{methodName} gave {checkName}");
    }
}
