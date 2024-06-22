using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GatorRando.QuestMods;

static class Junk4TrashQuestMods
{
    public static void HideCollectedItems()
    {
        GameObject junk_shop_object = Util.GetByPath("East (Creeklands)/Junk Shop/Cool Shop");
        JunkShop junk_shop = junk_shop_object.GetComponent<JunkShop>();
        List<string> junk4trash_items = ["Shield_Stretch", "Shield_TrashCanLid", "Item_StickyHand", "Item_PaintGun", "Sword_Wrench", "Sword_Grabby"];
        foreach (string item in junk4trash_items)
        {
            if (ArchipelagoManager.LocationIsCollected(item))
            {
                foreach (int i in Enumerable.Range(0, junk_shop.shopItems.Length))
                {
                    JunkShop.ShopItem shop_item = junk_shop.shopItems[i];
                    if (shop_item.item.name == item)
                    {
                        shop_item.isHidden = true;
                        junk_shop.shopItems[i] = shop_item;
                        break;
                    }
                }
            }
        }
    }
}