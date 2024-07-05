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
    }
}