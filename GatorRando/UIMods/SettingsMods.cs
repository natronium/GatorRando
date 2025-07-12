using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class SettingsMods
{
    public enum CheckfinderBehavior
    {
        Logic,
        ChecksOnly,
        Original
    }

    public static string GetCurrentSettingsPath()
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

    public static string GetCurrentRandoSettingsPath()
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

    public static UISubMenu CreateNewSettingsMenu()
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
                            () => ArchipelagoManager.InitiateNewAPSession(() =>
                            {
                                ReEnableMenuNavigation();
                                Plugin.ApplyAPDependentMods();
                            })
        );

            //Text fields for server address and port
            CreateStringSetting(viewportContent, 4, "Slot Name", "type in slot name", 16, true, true);
            CreateStringSetting(viewportContent, 4, "Server Address:Port", "type in Archipelago server address formated as address:port", 999, true, true);
            CreateStringSetting(viewportContent, 5, "Password", "type in Archipelago server password (if no password, leave blank)", 30, true, true, InputField.ContentType.Password);
        }

        //Borrow character rename for player name
        // ReworkPlayerRename(viewportContent);

        // Add Toggle so that players can choose whether they want !collect-ed locations to count as checked or not
        CreateSettingsToggle(viewportContent, 8, "!collect counts as Checked", "set before connecting to server. if checked, locations that are !collect-ed by other seeds count as checked for advancing quests." +
        "if unchecked, uses what locations as saved in the save file.");
        CreateSettingsOptions(viewportContent, 9, "Megaphone and Texting Logic?", "The megaphone helps you find friends' quests. Texting with Jill helps you find pots, chests, races, and cardboard." +
            "This setting changes how these tools work. \"logic\": use randomizer logic to show only valid checks, \"checks only\": show all possible checks, \"original\": original behavior", ["logic", "checks only", "original"]);





        return newMenu.GetComponent<UISubMenu>();
    }



    public static void ForceIntoSettingsMenu()
    {
        // Force into settings menu
        UIMenus uIMenus = Util.GetByPath("Canvas").GetComponent<UIMenus>();
        uIMenus.OnPause();
        UISubMenu settingsMenu = Util.GetByPath(GetCurrentSettingsPath()).GetComponent<UISubMenu>();
        settingsMenu.Activate();
    }
    private static void ReEnableMenuNavigation()
    {
        Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/Back").SetActive(true);
        Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/Back To Title").SetActive(false);
    }
    public static void Edits()
    {
        // //Header for Display, etc. Settings
        // CreateSettingsHeader(3, "General Settings");

        // //Header for AP Settings
        // CreateSettingsHeader(3, "Archipelago");

        // //Connect button
        // CreateSettingsButton(
        //                     4,
        //                     "Connect To Server",
        //                     "connect to Archipelago game server using player name, server address, and port set above",
        //                     () => ArchipelagoManager.InitiateNewAPSession(() =>
        //                     {
        //                         ReEnableMenuNavigation();
        //                         Plugin.ApplyAPDependentMods();
        //                     })
        // );

        // //Text fields for server address and port
        // CreateStringSetting(4, "Server Address:Port", "type in Archipelago server address formated as address:port", 999, true, true);
        // CreateStringSetting(5, "Password", "type in Archipelago server password (if no password, leave blank)", 30, true, true, InputField.ContentType.Password);

        // //Borrow character rename for player name
        // ReworkPlayerRename();

        // // Disallow players starting gameplay before connecting to server
        // Util.GetByPath(currentSettingsPath + "Viewport/Content/Back").SetActive(false);
        // LoadScene backToTitle = Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<LoadScene>();
        // CreateSettingsButton(
        //                     2,
        //                     "Back To Title",
        //                     "return to title if you are unable to connect to an Archipelago game server (or chose the wrong save file)",
        //                     backToTitle.DoLoadScene
        //                     );

        // // Add Disconnect to Back To Title button
        // Util.GetByPath("Canvas/Pause Menu/Pause Content/Back to Title").GetComponent<Button>().onClick.AddListener(ArchipelagoManager.Disconnect);

        // // Add Toggle so that players can choose whether they want !collect-ed locations to count as checked or not
        // CreateSettingsToggle(8, "!collect counts as Checked", "set before connecting to server. if checked, locations that are !collect-ed by other seeds count as checked for advancing quests." +
        //     "if unchecked, uses what locations as saved in the save file.");
        // CreateSettingsOptions(9, "Megaphone and Texting Logic?","The megaphone helps you find friends' quests. Texting with Jill helps you find pots, chests, races, and cardboard." +
        //     "This setting changes how these tools work. \"logic\": use randomizer logic to show only valid checks, \"checks only\": show all possible checks, \"original\": original behavior", ["logic","checks only","original"]);
    }

    // private static void ReworkPlayerRename(GameObject newParent)
    // {
    //     GameObject originalPlayerName = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/Rename Character");
    //     GameObject playerName = GameObject.Instantiate(originalPlayerName, newParent.transform);
    //     playerName.transform.SetSiblingIndex(4);
    //     GameObject playerLabel = playerName.transform.Find("Label").gameObject;
    //     Object.Destroy(playerLabel.GetComponent<MLText>());
    //     Text playerLabelText = playerLabel.GetComponent<Text>();
    //     playerLabelText.text = "Set Slot Name: " + GameData.g.gameSaveData.playerName; // Get player name and display it here
    //     UIDescription playerDescript = playerName.AddComponent(typeof(UIDescription)) as UIDescription;
    //     playerDescript.descriptionText = "set player name to AP slot name";
    //     GameObject customizeButton = Util.GetByPath(GetCurrentSettingsPath() + "Viewport/Content/Customize Splits");
    //     UIDescription descript = customizeButton.GetComponent<UIDescription>();
    //     playerDescript.prefab = descript.prefab;
    //     playerName.SetActive(true);
    //     playerName.name = "Slot Name";
    // }

    private static void CreateStringSetting(GameObject newParent, int siblingIndex, string name, string description, int charLimit, bool shrinkToFit, bool saveToGameData, InputField.ContentType contentType = InputField.ContentType.Standard)
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
        input.saveToGameData = saveToGameData;
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

    public static CheckfinderBehavior GetCheckfinderBehavior() => (CheckfinderBehavior)Settings.s.ReadInt("megaphone and texting logic?");
}