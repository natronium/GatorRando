using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

static class InventoryMods
{
    public static void AddQuestItemTab()
    {
        // Create a new tab in the inventory screen for Quest Items
        // See UISwapItemsMenuPatch for prefix that populates this new tab
        GameObject Tabs = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/Tabs");
        GameObject QuestItemTab = GameObject.Instantiate(Tabs.transform.Find("Item Tab").gameObject,Tabs.transform);
        UITabNavigation TabNav = Tabs.GetComponent<UITabNavigation>();
        Toggle QuestItemTabToggle = QuestItemTab.GetComponent<Toggle>();
        TabNav.tabs = TabNav.tabs.Add<Toggle>(QuestItemTabToggle);
        QuestItemTab.name = "Quest Item Tab";

        GameObject TabContents = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Contents Mask/Tab Contents");
        GameObject QuestItemGrid = GameObject.Instantiate(TabContents.transform.Find("Item Grid").gameObject,TabContents.transform);
        QuestItemGrid.name = "Quest Item Grid";
        QuestItemTabToggle.onValueChanged.ObliteratePersistentListenerByIndex(0);
        QuestItemTabToggle.onValueChanged.AddListener(QuestItemGrid.SetActive);

        // Reposition the Tabs
        Tabs.transform.localPosition.Set(-25, Tabs.transform.localPosition.y, Tabs.transform.localPosition.z);
        GameObject TabButtomPrompsLeft = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/TabButtonPrompts/Left");
        GameObject TabButtomPrompsRight = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/TabButtonPrompts/Right");
        TabButtomPrompsLeft.transform.localPosition.Set(-275,TabButtomPrompsLeft.transform.localPosition.y, TabButtomPrompsLeft.transform.localPosition.z);
        TabButtomPrompsRight.transform.localPosition.Set(300,TabButtomPrompsRight.transform.localPosition.y, TabButtomPrompsRight.transform.localPosition.z);
        


    }

}

