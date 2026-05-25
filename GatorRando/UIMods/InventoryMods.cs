using System.Linq;
using GatorRando.QuestMods;
using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

internal static class InventoryMods
{
    internal static void AddQuestItemTab()
    {
    //     // Create a new tab in the inventory screen for Quest Items
    //     // See UISwapItemsMenuPatch for prefix that populates this new tab
        GameObject tabs = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/Tabs");
        GameObject questItemTab = GameObject.Instantiate(tabs.transform.Find("Item Tab").gameObject,tabs.transform);
        UITabNavigation tabNav = tabs.GetComponent<UITabNavigation>();
        Toggle questItemTabToggle = questItemTab.GetComponent<Toggle>();
        tabNav.tabs = tabNav.tabs.Add<Toggle>(questItemTabToggle);
        questItemTab.name = "Quest Item Tab";
        
        GameObject tabContents = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Contents Mask/Tab Contents");
        GameObject questItemScrollArea = GameObject.Instantiate(tabContents.transform.Find("Item Scroll Area").gameObject,tabContents.transform);
        questItemScrollArea.name = "Quest Item Scroll Area";
        questItemTabToggle.onValueChanged.ObliteratePersistentListenerByIndex(0);
        questItemTabToggle.onValueChanged.AddListener(questItemScrollArea.SetActive);
        GameObject activeTab = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/Tabs/Quest Item Tab/Inactive Tab/Active Tab");
        GameObject inactiveTab = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/Tabs/Quest Item Tab/Inactive Tab/");
        activeTab.GetComponent<Image>().sprite = SpriteHandler.GetSpriteForItem("QuestActiveTab");
        inactiveTab.GetComponent<Image>().sprite = SpriteHandler.GetSpriteForItem("QuestInactiveTab");
        GameObject questGrid = questItemScrollArea.transform.GetChild(0).GetChild(0).gameObject;
        questGrid.name = "Quest Grid";
        ItemGrid questGridIG = questGrid.GetComponent<ItemGrid>();
        questGridIG.LoadElements([]);
        questItemTabToggle.onValueChanged.AddListener(questGrid.SetActive);

        // Reposition Tabs and button prompts to accommodate additional tab
        tabs.transform.localPosition += new Vector3(-25,0,0);
        GameObject tabButtomPrompsLeft = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/TabButtonPrompts/Left");
        GameObject tabButtomPrompsRight = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/TabButtonPrompts/Right");
        tabButtomPrompsLeft.transform.localPosition += new Vector3(-10,0,0);
        tabButtomPrompsRight.transform.localPosition += new Vector3(33,0,0);
    }

}

