using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class SettingsMods
{
    public static void Edits()
    {
        GameObject settings_menu = Util.GetByPath("Main Menu/Main Menu Canvas/Settings/Viewport/Content");
        GameObject controls_header = Util.GetByPath("Main Menu/Main Menu Canvas/Settings/Viewport/Content/--Controls Header--");
        GameObject display_header = GameObject.Instantiate(controls_header, settings_menu.transform);
        GameObject display_text = display_header.transform.Find("Text").gameObject;
        Text display_text_text = display_text.GetComponent<Text>();
        display_text_text.text = "Display and Volume";
    }
} 