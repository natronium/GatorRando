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
    }
}