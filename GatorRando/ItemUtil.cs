using System;
using GatorRando.Archipelago;

namespace GatorRando;

public static class ItemUtil
{
    public enum FriendType
    {
        Surface,
        Mines,
        Roots,
        Drip
    }
    public static bool needToRefreshPlayerItemManager;
    public static bool refreshQueued = false;
    public static DateTime refreshRequestedTime;
    public static TimeSpan delayBeforeRefresh = TimeSpan.FromSeconds(1);
    public static void GiveFriends(int amount, FriendType friendType)
    {
        switch (friendType)
        {
            case FriendType.Surface:
                ItemResource popresource = Util.FindItemResourceByName("Population");
        popresource.Amount += amount;
                break;
            case FriendType.Mines:
                ItemResource minespopresource = Util.FindItemResourceByName("Friends_UGMountain");
        minespopresource.Amount += amount;
                break;
            case FriendType.Roots:
                ItemResource rootspopresource = Util.FindItemResourceByName("Friends_UGForest");
        rootspopresource.Amount += amount;
                break;
            case FriendType.Drip:
                ItemResource drippopresource = Util.FindItemResourceByName("Friends_UGWater");
        drippopresource.Amount += amount;
                break;
        }
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

    public static void GiveCryptid(string cryptidName)
    {
        if ((CryptidProfile.State)GameData.g.ReadInt("UG_Cryptid_" + cryptidName, 0) == 0)
        {
            GameData.g.Write("UG_Cryptid_" + cryptidName, (int)CryptidProfile.State.Found);
        }
        ItemObject itemObject = Util.FindItemObjectByName(cryptidName);
        UIMenus.craftNotification.LoadItems([itemObject]);
        // TODO: test this code
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
                PlayerItemManager.p.gliderItem.IsUnlocked = true;
            }
            PlayerItemManager.p.Refresh();
            refreshQueued = false;
        }
        
    }
}