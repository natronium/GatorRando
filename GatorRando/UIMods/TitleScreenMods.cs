using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class TitleScreenMods
{
    private static GameObject startButton;

    public static void Edits()
    {
        GameObject newGameButton = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/New Game");
        newGameButton.SetActive(false);
        startButton = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/Load Game Menu");
        GameObject startButtonText = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/Load Game Menu/Text");
        Object.DestroyImmediate(startButtonText.GetComponent<MLText>());
        Text startButtonTextText = startButtonText.GetComponent<Text>();
        startButtonTextText.text = "Start Rando";

        GameObject settingsButton = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/Settings");
        GameObject titleMenuButtons = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/");
        GameObject newButton = GameObject.Instantiate(settingsButton, titleMenuButtons.transform);
        newButton.transform.SetSiblingIndex(1);
        GameObject newButtonLabel = newButton.transform.Find("Text").gameObject;
        Object.Destroy(newButtonLabel.GetComponent<MLText>());
        Text newButtonText = newButton.GetComponentsInChildren<Text>()[0];
        newButtonText.text = "Rando Settings";
        newButton.name = "Rando Settings";
        Button newButtonButton = newButton.GetComponent<Button>();
        newButtonButton.onClick.ObliteratePersistentListenerByIndex(0);
        UISubMenu newSettingsMenu = RandoSettingsMenu.CreateNewSettingsMenu();
        newButtonButton.onClick.AddListener(newSettingsMenu.Activate);

        GameObject titleScreenMenu = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen");
        UIPreventDeselection uIPreventDeselection = titleScreenMenu.GetComponent<UIPreventDeselection>();
        uIPreventDeselection.defaultSelection = startButton;
        uIPreventDeselection.secondarySelection = newButton;
    }

    public static void EnableStartButton()
    {
        startButton = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/Load Game Menu");
        startButton.SetActive(true);
    }

    public static void DisableStartButton()
    {
        startButton = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/Load Game Menu");
        startButton.SetActive(false);
    }



}