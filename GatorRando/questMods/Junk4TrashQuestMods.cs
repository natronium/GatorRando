using System.Collections.Generic;
using System.Linq;
using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando.QuestMods;

static class Junk4TrashQuestMods
{
    public static void HideCollectedShopLocations()
    {
        GameObject junkShopObject = Util.GetByPath("East (Creeklands)/Junk Shop/Cool Shop");
        JunkShop junkShop = junkShopObject.GetComponent<JunkShop>();
        List<string> junk4trashItems = ["Shield_Stretch", "Shield_TrashCanLid", "Item_StickyHand", "Item_PaintGun", "Sword_Wrench", "Sword_Grabby"];
        foreach (string item in junk4trashItems)
        {
            if (LocationHandling.IsLocationCollected(item))
            {
                foreach (int i in Enumerable.Range(0, junkShop.shopItems.Length))
                {
                    JunkShop.ShopItem shopItem = junkShop.shopItems[i];
                    if (shopItem.item.name == item)
                    {
                        shopItem.isHidden = true;
                        junkShop.shopItems[i] = shopItem;
                        break;
                    }
                }
            }
        }
        QuestStates royQuestStates = Util.GetByPath("East (Creeklands)/Junk Shop").GetComponent<QuestStates>();
        if (royQuestStates.StateID <= 3)
        {
            junkShop.UpdateInventory(); //Don't update the inventory if at max count
        }
    }
}