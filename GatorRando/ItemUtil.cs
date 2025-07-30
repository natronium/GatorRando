using System;
using GatorRando.Archipelago;

namespace GatorRando;

public static class ItemUtil
{
    public static bool needToRefreshPlayerItemManager;
    public static bool refreshQueued = false;
    public static DateTime refreshRequestedTime;
    public static TimeSpan delayBeforeRefresh = TimeSpan.FromSeconds(1);
    public static void GiveFriends(int amount)
    {
        ItemResource popresource = Util.FindItemResourceByName("Population");
        popresource.Amount += amount;
    }

    public static void GiveCraftStuff(int amount)
    {
        ItemResource matresource = Util.FindItemResourceByName("CraftingMaterial");
        matresource.Amount += amount;
    }

    public static void GiveItem(string item)
    {
        ItemObject itemObject = Util.FindItemObjectByName(item);
        if (itemObject != null)
        {
            if (item == "Bracelet")
            {
                ItemManager.i.BraceletsCollected++;
                needToRefreshPlayerItemManager = true;
            }
            ItemManager.i.GiveItem(itemObject);
            UIMenus.craftNotification.LoadItems([itemObject]);
            return;
        }
        if (item == "Glider" || item == "Shirt")
        {
            needToRefreshPlayerItemManager = true;
            ItemManager.i.SetUnlocked(item);
        }
        else
        {
            ItemManager.i.UnlockItem(item);
        }
        
        PlayerItemManager.p.Refresh(); // Originally only for "Shirt" not usuable on receipt but should result in updated visual for bracelets
    }

    public static void GiveCraft(string item)
    {
        // Gives a recipe instead of the item
        ItemObject itemObject = Util.FindItemObjectByName(item);
        UIMenus.craftNotification.LoadItems([itemObject]);
        itemObject.hasShopEntry = true;
        itemObject.IsShopUnlocked = true;
    }

    public static void RefreshPlayerItemManagerIfNeeded()
    {
        if (needToRefreshPlayerItemManager)
        {
            refreshRequestedTime = DateTime.Now;
            needToRefreshPlayerItemManager = false;
            refreshQueued = true;
        }
        if (refreshQueued & (DateTime.Now - refreshRequestedTime > delayBeforeRefresh))
        {
            if (ItemHandling.IsItemUnlocked("Shirt"))
            {
                ItemManager.i.SetUnlocked("Glider");
            }
            PlayerItemManager.p.Refresh();
            refreshQueued = false;
        }
        
    }
}