using System;
using BepInEx;
using GatorRando.questMods;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando
{
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
            Logger.LogDebug("THIS IS A DEBUG LOG! LOOK AT ME! @@@@@@@@@@@");
        }

        void OnEnable()
        {
            Debug.Log("OnEnable called");
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);
            if (scene.name == "Island")
            {
                //Allow Freeplay
                if (freeplay_from_start)
                {
                    TutorialQuestMods.HandleFreeplay();
                }

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

        public static void LogDebug(String s)
        {
            Instance.Logger.LogDebug(s);
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
}
