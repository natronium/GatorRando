using BepInEx;
using GatorRando.Archipelago;
using HarmonyLib;
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
        ArchipelagoConsole.Awake();
        ArchipelagoConsole.LogMessage($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void Update()
    {
        StateManager.Update();
    }

    private void OnGUI()
    {
        ArchipelagoConsole.OnGUI();
    }

    void OnEnable()
    {
        // Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += StateManager.OnSceneLoaded;
    }

    void Destroy()
    {
        StateManager.Disconnect();
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
