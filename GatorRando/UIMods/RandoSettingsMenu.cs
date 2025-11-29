using GatorRando.Archipelago;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatorRando.UIMods;

internal static class RandoSettingsMenu
{
	private static UISubMenu newSettingsMenu;

    internal enum CheckfinderBehavior
    {
        Logic,
        ChecksOnly,
        Original
    }

    internal static string GetCurrentSettingsPath()
    {
        string currentSettingsParent;
        if (SceneManager.GetActiveScene().name == "Prologue")
        {
            currentSettingsParent = "Main Menu/Main Menu Canvas/";
        }
        else
        {
            currentSettingsParent = "Canvas/Pause Menu/";
        }
        return currentSettingsParent + "Settings/";
    }

    internal static string GetCurrentRandoSettingsPath()
    {
        return GetCurrentSettingsParent() + "Rando Settings/";
    }

    private static string GetCurrentSettingsParent()
    {
        if (SceneManager.GetActiveScene().name == "Prologue")
        {
            return "Main Menu/Main Menu Canvas/";
        }
        else
        {
            return "Canvas/Pause Menu/";
        }
    }

    internal static UISubMenu CreateNewSettingsMenu()
    {
        GameObject settingsMenuObject = Util.GetByPath(GetCurrentSettingsPath());
        GameObject parentCanvas = Util.GetByPath(GetCurrentSettingsParent());
        GameObject newMenu = GameObject.Instantiate(settingsMenuObject, parentCanvas.transform);
        newMenu.name = "Rando Settings";
        GameObject viewportContent = Util.GetByPath(GetCurrentSettingsParent() + "Rando Settings" + "/Viewport/Content");
        foreach (Transform child in viewportContent.transform)
        {
            switch (child.gameObject.name)
            {
                case "Background": break;
                case "--Header--":
                    GameObject headerText = child.Find("Text").gameObject;
                    Object.Destroy(headerText.GetComponent<MLText>());
                    headerText.GetComponent<Text>().text = "RANDO SETTINGS";
                    break;
                case "Back": break;
                default:
                    Object.Destroy(child.gameObject);
                    break;
            }
        }

        if (SceneManager.GetActiveScene().name == "Prologue")
        {
            //Connect button
            CreateSettingsButton(viewportContent,
                            4,
                            "Connect To Server",
                            "connect to Archipelago game server using player name, server address, and port set above",
                            StateManager.AttemptConnection
        );

            //Text fields for server address and port
            CreateStringSetting(viewportContent, 4, SaveManager.slotNameString, "type in slot name", 16, true, true);
            CreateStringSetting(viewportContent, 5, SaveManager.serverString, "type in Archipelago server address formated as address:port", 999, true, true);
            CreateStringSetting(viewportContent, 6, SaveManager.passwordString, "type in Archipelago server password (if no password, leave blank)", 30, true, true, InputField.ContentType.Password);

            // Add Toggle so that players can choose whether they want !collect-ed locations to count as checked or not
            CreateSettingsToggle(viewportContent, 8, "!collect counts as Checked", "set before connecting to server. if checked, locations that are !collect-ed by other seeds count as checked for advancing quests." +
            "if unchecked, uses what locations as saved in the save file.");

            CreateSettingsToggle(viewportContent, 9, "Skip Prologue", "set before starting a new game. If true, will skip the prologue and set the player name to the slot name.");

            //Connect button
            CreateSettingsButton(viewportContent,
                            13,
                            "Delete all AP Saves",
                            "delete all AP saves for Lil Gator Game. useful for cleaning up old runs",
                            SaveManager.EraseAllAPSaveData
        );
        }

        //Borrow character rename for player name
        // ReworkPlayerRename(viewportContent);


        CreateSettingsToggle(viewportContent, 10, "Pause Speedrun for Item Get Dialogues", "If speedrun mode is on, skips through dialogue normally except dialogues that show what item you found");
        CreateSettingsToggle(viewportContent, 11, "Hide Speedrun Timer", "Hides the speedrun timer (if you want to skip dialogue, but not see the timer)");
        CreateSettingsOptions(viewportContent, 12, "Megaphone and Texting Logic?", "The megaphone helps you find friends' quests. Texting with Jill helps you find pots, chests, races, and cardboard." +
            "This setting changes how these tools work. \"logic\": use randomizer logic to show only valid checks, \"checks only\": show all possible checks, \"original\": original behavior", ["logic", "checks only", "original"]);

        newSettingsMenu = newMenu.GetComponent<UISubMenu>();
        return newSettingsMenu;
    }

    internal static void LeaveRandoSettingsMenu()
    {
        newSettingsMenu.Deactivate();
    }

    internal static void EnterRandoSettingsMenu()
    {
        newSettingsMenu.Activate();
    }

    private static void CreateStringSetting(GameObject newParent, int siblingIndex, string name, string description, int charLimit, bool shrinkToFit, bool saveAsLastConnection, InputField.ContentType contentType = InputField.ContentType.Standard)
    {
        GameObject autoname = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/AutoName");
        GameObject field = GameObject.Instantiate(autoname, newParent.transform);
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
        input.saveAsLastConnection = saveAsLastConnection;
        UIDescription descript = field.GetComponent<UIDescription>();
        descript.document = null;
        descript.descriptionText = description;
        GameObject label = field.transform.Find("Label").gameObject;
        Object.Destroy(label.GetComponent<MLText>());
        Text labelText = label.GetComponent<Text>();
        labelText.text = name.ToLower();
    }

    private static void CreateSettingsButton(GameObject newParent, int siblingIndex, string name, string description, UnityEngine.Events.UnityAction call)
    {
        GameObject customizeButton = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/Customize Splits");
        GameObject button = GameObject.Instantiate(customizeButton, newParent.transform);
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

    private static void CreateSettingsToggle(GameObject newParent, int siblingIndex, string name, string description)
    {
        GameObject aimToggle = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/use movement to aim");
        GameObject toggle = GameObject.Instantiate(aimToggle, newParent.transform);
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

    private static void CreateSettingsHeader(GameObject newParent, int siblingIndex, string name)
    {
        GameObject controlsHeader = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/--Controls Header--");
        GameObject header = GameObject.Instantiate(controlsHeader, newParent.transform);
        header.transform.SetSiblingIndex(siblingIndex);
        header.name = "--" + name + " Header--";
        GameObject text = header.transform.Find("Text").gameObject;
        Object.Destroy(text.GetComponent<MLText>());
        Text textText = text.GetComponent<Text>();
        textText.text = name.ToUpper();
    }

    private static void CreateSettingsOptions(GameObject newParent, int siblingIndex, string name, string description, string[] options)
    {
        GameObject recenterOptions = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/Re-Center Camera");
        GameObject optionsItem = GameObject.Instantiate(recenterOptions, newParent.transform);
        optionsItem.transform.SetSiblingIndex(siblingIndex);
        optionsItem.name = name;
        GameObject label = optionsItem.transform.Find("Label").gameObject;
        Object.Destroy(label.GetComponent<MLText>());
        Text labelText = label.GetComponent<Text>();
        labelText.text = name.ToLower();
        UIDescription descript = optionsItem.GetComponent<UIDescription>();
        descript.document = null;
        descript.descriptionText = description;
        SettingOptions settingOptions = optionsItem.GetComponent<SettingOptions>();
        settingOptions.key = name.ToLower();
        SelectOptions selectOptions = settingOptions.selectOptions;
        selectOptions.options = options;
    }

    internal static CheckfinderBehavior GetCheckfinderBehavior() => (CheckfinderBehavior)Settings.s.ReadInt("megaphone and texting logic?");

    internal static bool IsPrologueToBeSkipped() => Settings.s.ReadBool("skip prologue", true);

    internal static bool PauseForItemGet() => Settings.s.ReadBool("Pause Speedrun for Item Get Dialogues".ToLower(), true);
    internal static bool HideSpeedrunTimer() => Settings.s.ReadBool("hide speedrun timer", true);

    internal static bool IsCollectCountedAsChecked() => Settings.s.ReadBool("!collect counts as checked", true);
}