using UnityEngine;

namespace GatorRando.UIMods;

internal static class TutorialUIMods
{
    public static void Edits()
    {
        GameObject tutorials = Util.GetByPath("Canvas/Tutorials");
        tutorials.SetActive(false);
    }
}