using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class TitleScreenMods
{
    public static void Edits()
    {
        GameObject startButton = Util.GetByPath("Main Menu/Main Menu Canvas/Title Screen/Buttons/Load Game Menu/Text");
        Object.DestroyImmediate(startButton.GetComponent<MLText>());
        Text startButtonText = startButton.GetComponent<Text>();
        startButtonText.text = "Start Rando";

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
        UISubMenu newSettingsMenu = SettingsMods.CreateNewSettingsMenu();
        newButtonButton.onClick.AddListener(newSettingsMenu.Activate);

    }

    public static void NewSettings()
    {

    }

}