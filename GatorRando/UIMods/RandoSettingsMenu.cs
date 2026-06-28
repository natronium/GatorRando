using GatorRando.Archipelago;
using GatorRando.QuestMods;
using System.Collections.Generic;
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

    internal static UISettings GetUISettings()
    {
        return newSettingsMenu.gameObject.GetComponent<UISettings>();
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
            //Text fields for server address and port
            CreateStringSetting(viewportContent, 4, SaveManager.slotNameString, "type in slot name", 16, true, true);
            CreateStringSetting(viewportContent, 5, SaveManager.serverString, "type in Archipelago server address formated as address:port", 999, true, true);
            CreateStringSetting(viewportContent, 6, SaveManager.passwordString, "type in Archipelago server password (if no password, leave blank)", 30, true, true, InputField.ContentType.Password);
            //Connect button
            CreateSettingsButton(viewportContent,
                            7,
                            "Connect To Server",
                            "connect to Archipelago game server using player name, server address, and port set above",
                            StateManager.AttemptConnection
            );

            // Add Toggle so that players can choose whether they want !collect-ed locations to count as checked or not
            CreateSettingsToggle(viewportContent, 8, boolRandoSettings[BoolRandoSetting.Collect].SettingsKey, "set before connecting to server. if checked, locations that are !collect-ed by other seeds count as checked for advancing quests." +
            "if unchecked, uses what locations as saved in the save file.");

            CreateSettingsToggle(viewportContent, 9, boolRandoSettings[BoolRandoSetting.Prologue].SettingsKey, "set before starting a new game. If true, will skip the prologue and set the player name to the slot name.");
            CreateSettingsToggle(viewportContent, 10, boolRandoSettings[BoolRandoSetting.SkipFinalSequences].SettingsKey, "set before loading into a game. If true, goal will trigger on talking to your friends at the Playground when the flashback would start."); // TODO: reword based on DLC

            CreateSettingsToggle(viewportContent, 11, boolRandoSettings[BoolRandoSetting.DeathLink].SettingsKey, "When you receive a DeathLink from another game, your character will ragdoll. Note: you cannot send DeathLinks to other players. This option must be toggled on the main menu.");

            CreateSettingsToggle(viewportContent, 12, boolRandoSettings[BoolRandoSetting.TrapLink].SettingsKey, "Send and receive linkable traps. This option must be toggled on the main menu.");

            //Delete all saves button
            CreateSettingsButton(viewportContent,
                            40,
                            "Delete all AP Saves",
                            "delete all AP saves for Lil Gator Game. useful for cleaning up old runs",
                            SaveManager.EraseAllAPSaveData
            );
        }
        else if (SceneManager.GetActiveScene().name == "Island")
        {
            GameObject act1 = Util.GetByPath("NorthWest (Tutorial Island)/Act 1");
            QuestStates act1QuestStates = act1.GetComponent<QuestStates>();
            if (act1QuestStates.StateID < 3 && Options.GetOptionBool(Options.Option.StartWithFreeplay))
            {
                CreateSettingsButton(viewportContent,
                                4,
                                "Retry Applying Freeplay",
                                "if freeplay is turned on in your yaml, but the barrier didn't fall, use this button to retry lowering the tutorial barrier",
                                TutorialQuestMods.AdvanceToEndOfTutorial
                );
            }
        }
        CreateSettingsToggle(viewportContent, 13, boolRandoSettings[BoolRandoSetting.PauseItemGet].SettingsKey, "If speedrun mode is on, skips through dialogue normally except dialogues that show what item you found");
        CreateSettingsToggle(viewportContent, 14, boolRandoSettings[BoolRandoSetting.SpeedrunTimer].SettingsKey, "Shows the speedrun timer (regardless of whether Speedrun Mode is on)");
        CreateSettingsOptions(viewportContent, 15, "Megaphone and Texting Logic?", "The megaphone helps you find friends' quests. Texting with Jill helps you find pots, chests, races, and cardboard." +
            "This setting changes how these tools work. \"logic\": use randomizer logic to show only valid checks, \"checks only\": show all possible checks, \"original\": original behavior", ["logic", "checks only", "original"]);
        CreateSettingsToggle(viewportContent, 16, boolRandoSettings[BoolRandoSetting.Minimap].SettingsKey, "Shows the minimap and related navigation tools");

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
        button.SetActive(true);
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

    internal enum BoolRandoSetting
    {
        Prologue,
        PauseItemGet,
        SpeedrunTimer,
        Collect,
        SkipFinalSequences,
        DeathLink,
        TrapLink,
        Minimap
    }

    private class BoolRandoKeyDefault
    {
        public readonly string SettingsKey;
        public readonly bool DefaultValue;

        internal BoolRandoKeyDefault(string settingsKey, bool defaultValue)
        {
            SettingsKey = settingsKey;
            DefaultValue = defaultValue;
        }
    }

    private static readonly Dictionary<BoolRandoSetting, BoolRandoKeyDefault> boolRandoSettings = new () {
        {BoolRandoSetting.Prologue, new("skip prologue", true)},
        {BoolRandoSetting.PauseItemGet, new("Pause Speedrun Mode for Item Get Dialogues".ToLower(), true)},
        {BoolRandoSetting.SpeedrunTimer, new("show speedrun timer",false)},
        {BoolRandoSetting.Collect, new("!collect counts as checked",true)},
        {BoolRandoSetting.SkipFinalSequences, new("skip final sequence(s)",false)},
        {BoolRandoSetting.DeathLink, new("ragdoll on deathlink",false)},
        {BoolRandoSetting.TrapLink, new("traplink",false)},
        {BoolRandoSetting.Minimap, new("show minimap",true)},
    };

    internal static bool GetBoolRandoSetting(BoolRandoSetting randoSetting)
    {
        BoolRandoKeyDefault keyDefault = boolRandoSettings[randoSetting];
        return Settings.s != null ? Settings.s.ReadBool(keyDefault.SettingsKey, keyDefault.DefaultValue) : keyDefault.DefaultValue;
    }

    internal static CheckfinderBehavior GetCheckfinderBehavior()
    {
        if (Settings.s != null)
        {
            return (CheckfinderBehavior)Settings.s.ReadInt("megaphone and texting logic?"); ;
        }
        else
        {
            return CheckfinderBehavior.Original;
        }
    }
}