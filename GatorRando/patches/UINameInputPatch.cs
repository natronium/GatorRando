using HarmonyLib;
using UnityEngine;
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
        GameObject player_name = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Rename Character");
        GameObject player_label = player_name.transform.Find("Label").gameObject;
        Text player_label_text = player_label.GetComponent<Text>();
        player_label_text.text = "player: " + GameData.g.gameSaveData.playerName; // Get player name and display it here
        // Go back into Settings menu
        Game.State = GameState.Menu;
        GameObject pausemenu = Util.GetByPath("Canvas/Pause Menu");
        pausemenu.SetActive(true);
        GameObject settings = Util.GetByPath("Canvas/Pause Menu/Settings");
        settings.SetActive(true);
    }

    [HarmonyPostfix]
    [HarmonyPatch("Awake")]
    static void PostAwake(UINameInput __instance) {
        __instance.inputField.characterLimit = 16;
    }
}