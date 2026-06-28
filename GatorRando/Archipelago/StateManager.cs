using System.Collections;
using GatorRando.Patches;
using GatorRando.PrefabMods;
using GatorRando.QuestMods;
using GatorRando.UIMods;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GatorRando.Archipelago;

public static class StateManager
{
    // private static State previousState = State.TitleScreenPreConnect;
    private static State currentState;
    public enum State
    {
        TitleScreenPreConnect,
        TitleScreenAttemptingConnection,
        TitleScreenConnectionSucceeded,
        TitleScreenConnectionError,
        NewGamePrologue,
        NewGameSkipPrologue,
        LoadingGame,
        PlayingGameConnected,
        PlayingGameRetryingConnection,
        SwitchingScenes,
    }

    public static State GetCurrentState()
    {
        return currentState;
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        // Plugin.LogDebug("OnSceneLoaded: " + scene.name);
        if (Plugin.Instance.firstLoad == true)
        {
            if (scene.name == "Prologue")
            {
                SpriteHandler.LoadSprites();
            }
        }
        else
        {
            if (scene.name == "Prologue")
            {
                StartUp();
            }
            else if (scene.name == "Island" || scene.name == "Underground")
            {
                if (currentState == State.PlayingGameConnected)
                {
                    currentState = State.SwitchingScenes;
                    Plugin.Instance.StartCoroutine(SetupLoadedScene());
                }
                else
                {
                    Plugin.Instance.StartCoroutine(SetupLoadedGame());
                }
                UIMenus.u.SetGameplayState(false, true); // Disable player input
            }
        }
    }

    public static void StartUp()
    {
        if (GetCurrentState() != State.TitleScreenPreConnect)
        {
            ConnectionManager.Disconnect();
        }
        currentState = State.TitleScreenPreConnect;
        if (!TitleScreenMods.CheckIfEditsApplied())
        {
            TitleScreenMods.Edits();
        }
        TitleScreenMods.DisableStartButton();
        
    }

    public static void Update()
    {
        if (currentState == State.PlayingGameConnected && ConnectionManager.Authenticated && (SceneManager.GetActiveScene().name == "Island" || SceneManager.GetActiveScene().name == "Underground"))
        {
            ItemHandling.ProcessItemQueue();
            MapManager.UpdateCoordsIfNeeded();
            BubbleManager.Update();
            ItemUtil.RefreshPlayerItemManagerIfNeeded();
            NavigationUI.UpdateNavigation();
            TrapManager.trapHandler.MoveNext();
        }
    }

    public static void Disconnect()
    {
        ConnectionManager.UnregisterItemReceivedListener();
        ItemHandling.ClearItemQueue();
        ConnectionManager.Disconnect();
        DialogueModifier.CleanUp();
        switch (GetCurrentState())
        {
            case State.NewGamePrologue:
            case State.NewGameSkipPrologue:
            case State.LoadingGame:
            case State.PlayingGameConnected:
            case State.PlayingGameRetryingConnection:
                QuitToTitleScreen();
                break;
        }
    }

    public static void AttemptConnection()
    {
        if (GetCurrentState() == State.TitleScreenConnectionSucceeded)
        {
            ConnectionManager.UnregisterItemReceivedListener();
            ItemHandling.ClearItemQueue();
            TitleScreenMods.DisableStartButton();
        }
        currentState = State.TitleScreenAttemptingConnection;
        DisplayMessage("Attempting Connection to server");
        ConnectionManager.InitiateNewAPSession();
    }

    public static void FailedConnection(string error)
    {
        currentState = State.TitleScreenConnectionError;
        DisplayError(error);
    }

    public static void DisplayMessage(string message)
    {
        ArchipelagoConsole.LogMessage(message);
    }

    public static void DisplayError(string error)
    {
        ArchipelagoConsole.LogError(error);
    }

    public static void SucceededConnection()
    {
        if (GetCurrentState() == State.TitleScreenAttemptingConnection)
        {
            currentState = State.TitleScreenConnectionSucceeded;
            Plugin.Instance.StartCoroutine(PostConnectTitleScreen());
            //TODO take connection succeeded actions // Unsure what I meant here?
        }
        else if (GetCurrentState() == State.PlayingGameRetryingConnection)
        {
            currentState = State.PlayingGameConnected;
            //TODO take connection retry succeeded actions
        }
        else
        {
            Plugin.LogError($"SucceededConnection called when not in a connection attempt state. State was {currentState}");
        }
    }

    private static IEnumerator PostConnectTitleScreen()
    {
        yield return SaveManager.LoadAPSaveData();
        TitleScreenMods.EnableStartButton();
        RandoSettingsMenu.LeaveRandoSettingsMenu();
    }

    private static void QuitToTitleScreen()
    {
        SaveManager.ForceSave();
        if (RandoSettingsMenu.GetBoolRandoSetting(RandoSettingsMenu.BoolRandoSetting.DeathLink) && !Plugin.Instance.quitting)
        {
            DeathLinkManager.DisableDeathLink();
        }
        if (RandoSettingsMenu.GetBoolRandoSetting(RandoSettingsMenu.BoolRandoSetting.TrapLink) && !Plugin.Instance.quitting)
        {
            TrapManager.DisableTrapLink();
        }
        LoadScene backToTitle = Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<LoadScene>();
        backToTitle.DoLoadScene();
        currentState = State.TitleScreenPreConnect;
    }

    public static bool StartNewGame(int index)
    {
        if (RandoSettingsMenu.GetBoolRandoSetting(RandoSettingsMenu.BoolRandoSetting.Prologue))
        {
            //Skip the prologue by loading a built-in post prologue save file
            currentState = State.NewGameSkipPrologue;
            Plugin.Instance.StartCoroutine(SaveManager.LoadPostPrologueSaveData(index));
            return false;
        }
        else
        {
            //Don't modify game flow if going into prologue
            currentState = State.NewGamePrologue;
            SaveFileScreenPatch.startingLoad = false;
            return true;
        }
    }

    public static bool LoadGame(int index)
    {
        SaveFileScreenPatch.startingLoad = false;
        if (!SaveManager.CheckIfSaveAheadOfServer(index))
        {
            currentState = State.LoadingGame;
            return true;
        }
        DisplayError("Error: The save file you tried to load is ahead of the server it is connected to. When starting a new room for the same seed, you need to start a new save file.");
        return false;
    }

    public static IEnumerator SetupLoadedGame()
    {
        // Tasks that should be done once per loading a save file
        ConnectionManager.RegisterItemReceivedListener();
        ConnectionManager.ServerData.PopulateLocationLookupCache();
        LocationHandling.SendLocallySavedLocations();
        ConnectionManager.ReceiveUnreceivedItems();
        LocationAccessibilty.UpdateAccessibleLocations();
        if (RandoSettingsMenu.IsRagdollDeathLinkOn())
        {
            DeathLinkManager.EnableDeathLink();
        }
        TrapManager.Setup();
        yield return SetupLoadedScene();
    }

    public static IEnumerator SetupLoadedScene()
    {
        // Tasks that should happen everytime Island or Underground are loaded (transient changes to scene hierarchy)
        yield return new WaitForSeconds(Plugin.LoadDelay);
        SkippingRockMods.EditRockLayer();
        Util.PopulatePotPrefabs();
        UIEditMod.ApplyUIEdits();
        QuestEditMod.ApplyQuestEdits();
        ItemHandling.TriggerItemListeners();
        LocationHandling.TriggerLocationListeners();
        NavigationUI.Setup();
        DialogueModifier.CleanUp();
        UIMenus.u.SetGameplayState(true, true);
        currentState = State.PlayingGameConnected;
    }

    internal static void OpenUnderground()
    {
        static void IsTutorialComplete(int stateID)
        {
            if (stateID >= 3)
            {
                OpenUnderground();
            }
        }
        GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
        QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
        if (Options.GetOptionBool(Options.Option.StartWithFreeplay) == true || act1QuestStates.StateID >= 3)
        {
            QuestStates ITD = Util.GetByPath("In The Dark DLC Content/In the dark quest").GetComponent<QuestStates>();
            ITD.ProgressToState(3);
        }
        else
        {
            act1QuestStates.onStateChange.AddListener(IsTutorialComplete);
        }
    }
}