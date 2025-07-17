using GatorRando.PrefabMods;
using GatorRando.QuestMods;
using GatorRando.UIMods;
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
            SetupLoadedGame();
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
        ConnectionManager.InitiateNewAPSession();
    }

    public static void FailedConnection(string error)
    {
        currentState = State.TitleScreenConnectionError;
        DisplayError(error);
    }

    public static void DisplayError(string error)
    {
        //TODO Error display
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
        LoadScene backToTitle = Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<LoadScene>();
        backToTitle.DoLoadScene();
    }

    public static void StartNewGame()
    {
        if (RandoSettingsMenu.IsPrologueToBeSkipped())
        {
            currentState = State.NewGameSkipPrologue;
        }
        else
        {
            currentState = State.NewGamePrologue;
        }
    }

    public static void LoadGame()
    {
        currentState = State.LoadingGame;
    }

    public static void SetupLoadedGame()
    {
        currentState = State.PlayingGameConnected;
        ConnectionManager.RegisterItemReceivedListener();
        SpriteHandler.LoadSprites();
        QuestEditMod.ApplyQuestEdits();
        UIEditMod.ApplyUIEdits();
        BalloonMods.EditBalloonStamina();
        RockMods.EditRockLayer();
        Util.PopulatePotPrefabs();
        ConnectionManager.ServerData.PopulateLocationLookupCache();
        LocationHandling.SendLocallySavedLocations();
        ConnectionManager.ReceiveUnreceivedItems();
        ItemHandling.TriggerItemListeners();
        LocationHandling.TriggerLocationListeners();
        LocationAccessibilty.UpdateAccessibleLocations();
    }
}