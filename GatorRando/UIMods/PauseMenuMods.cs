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
        GameObject randoLabel = randoSettingsButton.transform.Find("Text").gameObject;
        Object.Destroy(randoLabel.GetComponent<MLText>());
        Text randoLabelText = randoLabel.GetComponent<Text>();
        randoLabelText.text = "Rando Settings".ToLower();
        Button randoButtonButton = randoSettingsButton.GetComponent<Button>();
        randoButtonButton.onClick.ObliteratePersistentListenerByIndex(0);
        randoButtonButton.onClick.AddListener(RandoSettingsMenu.EnterRandoSettingsMenu);
        GameObject resetPosition = GameObject.Instantiate(settingsButton, pauseMenu.transform);
        resetPosition.transform.SetSiblingIndex(3);
        resetPosition.name = "Reset Position";
        GameObject resetLabel = resetPosition.transform.Find("Text").gameObject;
        Object.Destroy(resetLabel.GetComponent<MLText>());
        Text resetLabelText = resetLabel.GetComponent<Text>();
        resetLabelText.text = "reset position".ToLower();
        Button resetButtonButton = resetPosition.GetComponent<Button>();
        resetButtonButton.onClick.ObliteratePersistentListenerByIndex(0);
        resetButtonButton.onClick.AddListener(RandoSettingsMenu.GetUISettings().ResetPlayerPosition);
    }
}