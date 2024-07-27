using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class SettingsMods
{
    public static void ForceIntoSettingsMenu()
    {
        // Force into settings menu
        UIMenus uIMenus = Util.GetByPath("Canvas").GetComponent<UIMenus>();
        uIMenus.OnPause();
        UISubMenu settingsMenu = Util.GetByPath("Canvas/Pause Menu/Settings").GetComponent<UISubMenu>();
        settingsMenu.Activate();
    }
    private static void ReEnableMenuNavigation()
    {
        Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Back").SetActive(true);
        Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Back To Title").SetActive(false);
    }
    public static void Edits()
    {
        //Header for Display, etc. Settings
        CreateSettingsHeader(3, "General Settings");

        //Header for AP Settings
        CreateSettingsHeader(3, "Archipelago");

        //Connect button
        CreateSettingsButton(
                            4,
                            "Connect To Server",
                            "connect to Archipelago game server using player name, server address, and port set above",
                            () => ArchipelagoManager.InitiateNewAPSession(() =>
                            {
                                ReEnableMenuNavigation();
                                Plugin.ApplyAPDependentMods();
                            })
        );

        //Text fields for server address and port
        CreateStringSetting(4, "Server Address:Port", "type in Archipelago server address formated as address:port", 20, true, true);
        CreateStringSetting(5, "Password", "type in Archipelago server password (if no password, leave blank)", 30, true, true, InputField.ContentType.Password);

        //Borrow character rename for player name
        ReworkPlayerRename();

        // Disallow players starting gameplay before connecting to server
        Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Back").SetActive(false);
        LoadScene backToTitle = Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<LoadScene>();
        CreateSettingsButton(
                            2,
                            "Back To Title",
                            "return to title if you are unable to connect to an Archipelago game server (or chose the wrong save file)",
                            backToTitle.DoLoadScene
                            );

        // Add Disconnect to Back To Title button
        Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<Button>().onClick.AddListener(ArchipelagoManager.Disconnect);

        // Add Toggle so that players can choose whether they want !collect-ed locations to count as checked or not
        CreateSettingsToggle(9, "!collect counts as Checked", "if checked, locations that are !collect-ed by other seeds count as checked for advancing quests." +
            "if unchecked, uses what locations as saved in the save file.");
    }

    private static void ReworkPlayerRename()
    {
        GameObject playerName = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Rename Character");
        playerName.transform.SetSiblingIndex(4);
        GameObject playerLabel = playerName.transform.Find("Label").gameObject;
        Object.Destroy(playerLabel.GetComponent<MLText>());
        Text playerLabelText = playerLabel.GetComponent<Text>();
        playerLabelText.text = "player: " + GameData.g.gameSaveData.playerName; // Get player name and display it here
        UIDescription playerDescript = playerName.AddComponent(typeof(UIDescription)) as UIDescription;
        playerDescript.descriptionText = "set player name to AP slot name";
        GameObject customizeButton = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Customize Splits");
        UIDescription descript = customizeButton.GetComponent<UIDescription>();
        playerDescript.prefab = descript.prefab;
    }

    private static void CreateStringSetting(int siblingIndex, string name, string description, int charLimit, bool shrinkToFit, bool saveToGameData, InputField.ContentType contentType = InputField.ContentType.Standard)
    {
        GameObject settingsMenu = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content");
        GameObject autoname = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/AutoName");
        GameObject field = GameObject.Instantiate(autoname, settingsMenu.transform);
        field.transform.SetSiblingIndex(siblingIndex);
        field.name = name;
        Object.DestroyImmediate(field.GetComponent<SelectOnHighlight>());
        Object.DestroyImmediate(field.GetComponent<SettingOptions>());
        Object.DestroyImmediate(field.GetComponent<SelectOptions>());
        field.transform.Find("Visual/Left").gameObject.SetActive(false);
        field.transform.Find("Visual/Right").gameObject.SetActive(false);
        GameObject visualContainer = field.transform.Find("Visual").gameObject;
        GameObject textGameobject = visualContainer.transform.Find("Selected Option").gameObject;
        Text text = textGameobject.GetComponent<Text>();
        InputField inputfield = field.AddComponent<InputField>();
        inputfield.textComponent = text;
        inputfield.targetGraphic = field.transform.Find("Highlight").GetComponent<Image>();
        inputfield.characterLimit = charLimit;
        inputfield.contentType = contentType;
        if (shrinkToFit)
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
        Text labelText = label.GetComponent<Text>();
        labelText.text = name.ToLower();
    }

    private static void CreateSettingsButton(int siblingIndex, string name, string description, UnityEngine.Events.UnityAction call)
    {
        GameObject settingsMenu = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content");
        GameObject customizeButton = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/Customize Splits");
        GameObject button = GameObject.Instantiate(customizeButton, settingsMenu.transform);
        button.transform.SetSiblingIndex(siblingIndex);
        button.name = name;
        GameObject label = button.transform.Find("Label").gameObject;
        Object.Destroy(label.GetComponent<MLText>());
        Text labelText = label.GetComponent<Text>();
        labelText.text = name.ToLower();
        UIDescription descript = button.GetComponent<UIDescription>();
        descript.document = null;
        descript.descriptionText = description;
        Button buttonButton = button.GetComponent<Button>();
        buttonButton.onClick.ObliteratePersistentListenerByIndex(0);
        buttonButton.onClick.AddListener(call);
    }

    private static void CreateSettingsToggle(int siblingIndex, string name, string description)
    {
        GameObject settingsMenu = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content");
        GameObject aimToggle = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/use movement to aim");
        GameObject toggle = GameObject.Instantiate(aimToggle, settingsMenu.transform);
        toggle.transform.SetSiblingIndex(siblingIndex);
        toggle.name = name;
        GameObject label = toggle.transform.Find("Label").gameObject;
        Object.Destroy(label.GetComponent<MLText>());
        Text labelText = label.GetComponent<Text>();
        labelText.text = name.ToLower();
        UIDescription descript = toggle.GetComponent<UIDescription>();
        descript.document = null;
        descript.descriptionText = description;
        SettingToggle settingToggle = toggle.GetComponent<SettingToggle>();
        settingToggle.key = name.ToLower();
    }

    private static void CreateSettingsHeader(int siblingIndex, string name)
    {
        GameObject settingsMenu = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content");
        GameObject controlsHeader = Util.GetByPath("Canvas/Pause Menu/Settings/Viewport/Content/--Controls Header--");
        GameObject header = GameObject.Instantiate(controlsHeader, settingsMenu.transform);
        header.transform.SetSiblingIndex(siblingIndex);
        header.name = "--" + name + " Header--";
        GameObject text = header.transform.Find("Text").gameObject;
        Object.Destroy(text.GetComponent<MLText>());
        Text textText = text.GetComponent<Text>();
        textText.text = name.ToUpper();
    }
}