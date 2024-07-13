using System;
using GatorRando.UIMods;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(UINameInput))]
static class UINameInputPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Confirm")]
    static void PostConfirm()
    {
        // Update the text in the settings menu to reflect the new player name
        try
        {
            GameObject player_name = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Rename Character");
            GameObject player_label = player_name.transform.Find("Label").gameObject;
            Text player_label_text = player_label.GetComponent<Text>();
            player_label_text.text = "player: " + GameData.g.gameSaveData.playerName; // Get player name and display it here
        }
        catch (InvalidOperationException)
        {
            // When setting name in Prologue, the Island version of Settings Menu doesn't exist yet
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    static void PostAwake(UINameInput __instance)
    {
        __instance.inputField.characterLimit = 16;
    }

    [HarmonyPostfix]
    [HarmonyPatch("OnDestroy")]
    static void PostOnDestroy()
    {
        // Go back into Settings menu
        if (SceneManager.GetActiveScene().name == "Island")
        {
            SettingsMods.ForceIntoSettingsMenu();
            APConnectedUI.ShowAPQuest();
        }
    }
}