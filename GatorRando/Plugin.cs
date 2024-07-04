using BepInEx;
using GatorRando.prefabMods;
using GatorRando.QuestMods;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;

    private void Awake()
    {
        Instance = this;
        var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(); // automatically patch based on harmony attributes
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        if (ArchipelagoManager.IsFullyConnected)
        {
            ArchipelagoManager.ProcessItemQueue();
        }
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log("OnSceneLoaded: " + scene.name);
            Debug.Log(mode);
            if (scene.name == "Island")
            {
                ApplyQuestEdits();
                ApplyUIEdits();
                BalloonMods.EditBalloonStamina();
                ForceIntoSettingsMenu();
            }

            static void ApplyQuestEdits()
            {
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
                SamQuestMods.Edits();

                //Goal Completion Edits
                CreditsMods.Edits();
            }

            static void ApplyUIEdits()
            {
                //UI Edits
                TutorialUIMods.Edits();
                QuestItems.AddItems();
                InventoryMods.AddQuestItemTab();
                SettingsMods.Edits();
            }

            static void ForceIntoSettingsMenu()
            {
                // Force into settings menu
                Game.State = GameState.Menu;
                GameObject pausemenu = Util.GetByPath("Canvas/Pause Menu");
                pausemenu.SetActive(true);
                GameObject settings = Util.GetByPath("Canvas/Pause Menu/Settings");
                settings.SetActive(true);
            }
        }
    }

    void Destroy()
    {
        ArchipelagoManager.Disconnect();
    }

    public static void ApplyAPDependentMods()
    {
        //Allow Freeplay
        if (ArchipelagoManager.GetOption("start_with_freeplay") == "1")
        {
            TutorialQuestMods.StartWithFreeplay();
        }

        // Junk4Trash Edits
        Junk4TrashQuestMods.HideCollectedShopLocations();
    }

    public static void LogInfo(string infoMessage)
    {
        Instance.Logger.LogInfo(infoMessage);
    }

    public static void LogDebug(string debugMessage)
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
}
