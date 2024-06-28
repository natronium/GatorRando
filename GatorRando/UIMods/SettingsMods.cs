using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class SettingsMods
{
    public static void Edits()
    {
        //Header for Display, etc. Settings
        GameObject settings_menu = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content");
        GameObject controls_header = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/--Controls Header--");
        GameObject display_header = GameObject.Instantiate(controls_header, settings_menu.transform);
        display_header.transform.SetSiblingIndex(3);
        GameObject display_text = display_header.transform.Find("Text").gameObject;
        Object.Destroy(display_text.GetComponent<MLText>());
        Text display_text_text = display_text.GetComponent<Text>();
        display_text_text.text = "GENERAL SETTINGS";

        //Header for AP Settings
        GameObject AP_header = GameObject.Instantiate(controls_header, settings_menu.transform);
        AP_header.transform.SetSiblingIndex(3);
        GameObject AP_text = AP_header.transform.Find("Text").gameObject;
        Object.Destroy(AP_text.GetComponent<MLText>());
        Text AP_text_text = AP_text.GetComponent<Text>();
        AP_text_text.text = "ARCHIPELAGO";

        //Connect button
        GameObject customize_button = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Customize Splits");
        GameObject connect_button = GameObject.Instantiate(customize_button, settings_menu.transform);
        connect_button.transform.SetSiblingIndex(4);
        GameObject connect_label = connect_button.transform.Find("Label").gameObject;
        Object.Destroy(connect_label.GetComponent<MLText>());
        Text connect_label_text = connect_label.GetComponent<Text>();
        connect_label_text.text = "connect to server";
        UIDescription connect_descript = connect_button.GetComponent<UIDescription>();
        connect_descript.document = null;
        connect_descript.descriptionText = "connect to Archipelago game server using host, port, and player name set above";
        // connect_button.button.onClick.

        //Text fields
        GameObject display_type = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Invert horizontal");
        GameObject host_field = GameObject.Instantiate(display_type, settings_menu.transform);
        host_field.transform.SetSiblingIndex(4);
        Object.DestroyImmediate(host_field.GetComponent<SelectOnHighlight>());
        Object.DestroyImmediate(host_field.GetComponent<SettingToggle>());
        Object.DestroyImmediate(host_field.GetComponent<Toggle>());
        Object.DestroyImmediate(host_field.transform.Find("Visual/Checkbox").gameObject);
        GameObject visual_container = host_field.transform.Find("Visual").gameObject;
        GameObject host_text = new("Text", [typeof(RectTransform), typeof(Text)]);
        Text host_textcomponent = host_text.GetComponent<Text>();
        host_textcomponent.text = "OHAI";
        host_text.transform.SetParent(visual_container.transform);
        InputField host_inputfield = host_field.AddComponent<InputField>();
        host_inputfield.textComponent = host_textcomponent;
        host_field.AddComponent<SelectOnHighlight>();

        //Borrow character rename for player name
        GameObject player_name = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Rename Character");
        player_name.transform.SetSiblingIndex(4);
        GameObject player_label = player_name.transform.Find("Label").gameObject;
        Object.Destroy(player_label.GetComponent<MLText>());
        Text player_label_text = player_label.GetComponent<Text>();
        player_label_text.text = "player:"; // Get player name and display it here
        UIDescription player_descript = player_name.AddComponent(typeof(UIDescription)) as UIDescription;
        player_descript.descriptionText = "set player name to AP slot name";
        player_descript.prefab = connect_descript.prefab;

        
    }
} 