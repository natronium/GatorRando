using BepInEx;
using BepInEx.Configuration;
using GatorRando.Archipelago;
using HarmonyLib;
using UnityEngine.SceneManagement;
using MC = Archipelago.MultiClient.Net;

namespace GatorRando;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static Plugin Instance;

    private static ConfigEntry<float> _loadDelay;
    public static float LoadDelay => _loadDelay.Value;

    public bool quitting = false;

    private void Awake()
    {
        Instance = this;
        Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        harmony.PatchAll(); // automatically patch based on harmony attributes
        ArchipelagoConsole.Awake();
        ArchipelagoConsole.LogMessage($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        _loadDelay = Config.Bind("Settings", "LoadDelay", 1f, "The time the mod will wait after loading in before the mod will finish loading all quest mods. Increase this value if it appears that modications are being applied too soon.");
    }

    private void Update()
    {
        StateManager.Update();
    }

    private void OnGUI()
    {
        ArchipelagoConsole.OnGUI();
    }

    private void OnEnable()
    {
        // Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += StateManager.OnSceneLoaded;
    }

    private void OnApplicationQuit()
    {
        LogDebug("Application quitting");
        quitting = true;
        if (ConnectionManager.Authenticated)
        {
            try
            {
                StateManager.Disconnect();
            }
            catch (MC.Exceptions.ArchipelagoSocketClosedException)
            {
                // Do nothing
            }
        }
    }

    private void Destroy()
    {
        if (ConnectionManager.Authenticated)
        {
            StateManager.Disconnect();
        }
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
