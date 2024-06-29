using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Yoga;

namespace GatorRando.UIMods;

static class InventoryMods
{
    public static void AddQuestItemTab()
    {
        GameObject Tabs = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Area/Tabs");
        GameObject QuestItemTab = GameObject.Instantiate(Tabs.transform.Find("Item Tab").gameObject,Tabs.transform);
        UITabNavigation TabNav = Tabs.GetComponent<UITabNavigation>();
        TabNav.tabs = TabNav.tabs.Add<Toggle>(QuestItemTab.GetComponent<Toggle>());
        QuestItemTab.name = "Quest Item Tab";
        

        GameObject TabContents = Util.GetByPath("Canvas/Items Menu (Tabs)/LeftArea/Tab Contents Mask/Tab Contents");
        GameObject QuestItemGrid = GameObject.Instantiate(TabContents.transform.Find("Item Grid").gameObject,TabContents.transform);
        QuestItemGrid.name = "Quest Item Grid";
        //TODO: Change on Value Changed to QuestItemGrid activate
        // QuestItemTab.GetComponent

        // Patch UISwapItemsMenu to add a postfix that LoadsElements for our new QuestItemGrid using a populated list of ItemObjects that the player has already received
        

    }
}