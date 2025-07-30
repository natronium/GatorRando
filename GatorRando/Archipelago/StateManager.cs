using System.Collections;
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
        PlayingGameRetryingConnection
    }

    public static State GetCurrentState()
    {
        return currentState;
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Plugin.LogDebug("OnSceneLoaded: " + scene.name);

        if (scene.name == "Prologue")
        {
            if (GetCurrentState() != State.TitleScreenPreConnect)
            {
                ConnectionManager.Disconnect();
            }
            StartUp();

        }
        else if (scene.name == "Island")
        {
            UIMenus.u.SetGameplayState(false, true); // Disable player input
            Plugin.Instance.StartCoroutine(SetupLoadedGame());
        }
    }

    public static void StartUp()
    {
        currentState = State.TitleScreenPreConnect;
        TitleScreenMods.Edits();
        TitleScreenMods.DisableStartButton();
        SaveManager.CreateSaveDirectory();
    }

    public static void Update()
    {
        if (currentState == State.PlayingGameConnected && ConnectionManager.Authenticated && SceneManager.GetActiveScene().name == "Island")
        {
            ItemHandling.ProcessItemQueue();
            MapManager.UpdateCoordsIfNeeded();
            BubbleManager.Update();
            ItemUtil.RefreshPlayerItemManagerIfNeeded();
        }
    }

    public static void Disconnect()
    {
        ConnectionManager.UnregisterItemReceivedListener();
        //TODO TEST QUIT TO TITLE SCREEN
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
        ItemHandling.ClearItemQueue();
        ConnectionManager.Disconnect();
        TitleScreenMods.DisableStartButton();
    }

    public static void AttemptConnection()
    {
        if (GetCurrentState() == State.TitleScreenConnectionSucceeded)
        {
            Disconnect();
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
            PostConnectTitleScreen();
            //TODO take connection succeeded actions
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

    private static void PostConnectTitleScreen()
    {
        SaveManager.LoadAPSaveData();
        TitleScreenMods.EnableStartButton();
        RandoSettingsMenu.LeaveRandoSettingsMenu();
    }

    private static void QuitToTitleScreen()
    {
        SaveManager.ForceSave();
        LoadScene backToTitle = Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<LoadScene>();
        backToTitle.DoLoadScene();
    }

    public static void StartNewGame(int index)
    {
        if (RandoSettingsMenu.IsPrologueToBeSkipped())
        {
            //Skip the prologue by loading a built-in post prologue save file
            currentState = State.NewGameSkipPrologue;
            SaveManager.LoadPostPrologueSaveData(index);
        }
        else
        {
            //Don't modify game flow if going into prologue
            currentState = State.NewGamePrologue;
        }
    }

    public static bool LoadGame(int index)
    {
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
        yield return new WaitForSeconds(Plugin.LoadDelay);
        currentState = State.PlayingGameConnected;
        ConnectionManager.RegisterItemReceivedListener();
        SpriteHandler.LoadSprites();
        UIEditMod.ApplyUIEdits();
        BalloonMods.EditBalloonStamina();
        RockMods.EditRockLayer();
        Util.PopulatePotPrefabs();
        ConnectionManager.ServerData.PopulateLocationLookupCache();
        LocationHandling.SendLocallySavedLocations();
        ConnectionManager.ReceiveUnreceivedItems();
        QuestEditMod.ApplyQuestEdits();
        ItemHandling.TriggerItemListeners();
        LocationHandling.TriggerLocationListeners();
        LocationAccessibilty.UpdateAccessibleLocations();
        UIMenus.u.SetGameplayState(true, true);
    }
}