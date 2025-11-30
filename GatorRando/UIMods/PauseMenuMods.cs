using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

internal static class PauseMenuMods
{
    internal static void AddRandoSettingsMenuButton()
    {
        GameObject pauseMenu = Util.GetByPath("Canvas/Pause Menu/Pause Content");
        GameObject settingsButton = Util.GetByPath("Canvas/Pause Menu/Pause Content/Settings");
        GameObject randoSettingsButton = GameObject.Instantiate(settingsButton, pauseMenu.transform);
        randoSettingsButton.transform.SetSiblingIndex(3);
        randoSettingsButton.name = "Rando Settings";
        GameObject label = randoSettingsButton.transform.Find("Text").gameObject;
        Object.Destroy(label.GetComponent<MLText>());
        Text labelText = label.GetComponent<Text>();
        labelText.text = "Rando Settings".ToLower();
        Button buttonButton = randoSettingsButton.GetComponent<Button>();
        buttonButton.onClick.ObliteratePersistentListenerByIndex(0);
        buttonButton.onClick.AddListener(RandoSettingsMenu.EnterRandoSettingsMenu);
    }
}