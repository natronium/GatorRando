using UnityEngine;

namespace GatorRando.UIMods;

static class TutorialUIMods
{
    public static void Edits()
    {
        GameObject tutorials = Util.GetByPath("Canvas/Tutorials");
        tutorials.SetActive(false);
    }
}