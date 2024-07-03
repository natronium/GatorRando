using System.Net.Mime;
using System.Reflection.Emit;
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
        display_header.name = "--General Settings Header--";
        GameObject display_text = display_header.transform.Find("Text").gameObject;
        Object.Destroy(display_text.GetComponent<MLText>());
        Text display_text_text = display_text.GetComponent<Text>();
        display_text_text.text = "GENERAL SETTINGS";

        //Header for AP Settings
        GameObject AP_header = GameObject.Instantiate(controls_header, settings_menu.transform);
        AP_header.transform.SetSiblingIndex(3);
        AP_header.name = "--Archipelago Header--";
        GameObject AP_text = AP_header.transform.Find("Text").gameObject;
        Object.Destroy(AP_text.GetComponent<MLText>());
        Text AP_text_text = AP_text.GetComponent<Text>();
        AP_text_text.text = "ARCHIPELAGO";

        //Connect button
        GameObject customize_button = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Customize Splits");
        GameObject connect_button = GameObject.Instantiate(customize_button, settings_menu.transform);
        connect_button.transform.SetSiblingIndex(4);
        connect_button.name = "Connect Button";
        GameObject connect_label = connect_button.transform.Find("Label").gameObject;
        Object.Destroy(connect_label.GetComponent<MLText>());
        Text connect_label_text = connect_label.GetComponent<Text>();
        connect_label_text.text = "connect to server";
        UIDescription connect_descript = connect_button.GetComponent<UIDescription>();
        connect_descript.document = null;
        connect_descript.descriptionText = "connect to Archipelago game server using player name, server address, and port set above";
        Button connect_button_button = connect_button.GetComponent<Button>();
        connect_button_button.onClick.ObliteratePersistentListenerByIndex(0);
        connect_button_button.onClick.AddListener(ArchipelagoManager.Connect); //TODO: UI element indicating successful/unsuccessful connection

        //Text fields for server address and port
        GameObject host_field = CreateStringSetting(4,"Server Address","type in Archipelago server address", 20, true, true);
        GameObject port_field = CreateStringSetting(5,"Server Port","type in Archipelago server port", 5, false, true, InputField.ContentType.IntegerNumber);

        //Borrow character rename for player name
        GameObject player_name = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Rename Character");
        player_name.transform.SetSiblingIndex(4);
        GameObject player_label = player_name.transform.Find("Label").gameObject;
        Object.Destroy(player_label.GetComponent<MLText>());
        Text player_label_text = player_label.GetComponent<Text>();
        player_label_text.text = "player: " + GameData.g.gameSaveData.playerName; // Get player name and display it here
        UIDescription player_descript = player_name.AddComponent(typeof(UIDescription)) as UIDescription;
        player_descript.descriptionText = "set player name to AP slot name";
        player_descript.prefab = connect_descript.prefab;

        //TODO: disable backspace means go back on Settings screen
        //TODO: force player into the Setting Menu to make sure they connect on loading a save
        GameObject back_button = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Back");
        back_button.SetActive(false);
    }

    private static GameObject CreateStringSetting(int siblingIndex, string name, string description, int charLimit, bool shrink_to_fit, bool saveToGameData, InputField.ContentType contentType = InputField.ContentType.Standard)
    {
        GameObject settings_menu = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content");
        GameObject autoname = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/AutoName");
        GameObject field = GameObject.Instantiate(autoname, settings_menu.transform);
        field.transform.SetSiblingIndex(siblingIndex);
        field.name = name;
        Object.DestroyImmediate(field.GetComponent<SelectOnHighlight>());
        Object.DestroyImmediate(field.GetComponent<SettingOptions>());
        Object.DestroyImmediate(field.GetComponent<SelectOptions>());
        field.transform.Find("Visual/Left").gameObject.SetActive(false);
        field.transform.Find("Visual/Right").gameObject.SetActive(false);
        GameObject visual_container = field.transform.Find("Visual").gameObject;
        GameObject text_gameobject = visual_container.transform.Find("Selected Option").gameObject;
        Text text = text_gameobject.GetComponent<Text>();
        InputField inputfield = field.AddComponent<InputField>();
        inputfield.textComponent = text;
        inputfield.targetGraphic = field.transform.Find("Highlight").GetComponent<Image>();
        inputfield.characterLimit = charLimit;
        inputfield.contentType = contentType;
        if (shrink_to_fit)
        {
            inputfield.lineType = InputField.LineType.MultiLineSubmit;
        }
        SettingInput input = field.AddComponent<SettingInput>();
        inputfield.onValueChanged.AddListener(input.OnValueChanged);
        field.AddComponent<SelectOnHighlight>();
        input.key = name.ToLower();
        input.saveToGameData = saveToGameData;
        UIDescription descript = field.GetComponent<UIDescription>();
        descript.document = null;
        descript.descriptionText = description;
        GameObject label = field.transform.Find("Label").gameObject;
        Object.Destroy(label.GetComponent<MLText>());
        Text label_text = label.GetComponent<Text>();
        label_text.text = name.ToLower();
        
        return field;
    }
} 