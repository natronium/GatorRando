namespace GatorRando;
public static class ItemUtil
{
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
            ItemManager.i.GiveItem(itemObject);
            UIMenus.craftNotification.LoadItems([itemObject]);
            return;
        }
        ItemManager.i.UnlockItem(item);
    }

    public static void GiveCraft(string item)
    {
        // Gives a recipe instead of the item
        ItemObject itemObject = Util.FindItemObjectByName(item);
        UIMenus.craftNotification.LoadItems([itemObject]);
        itemObject.hasShopEntry = true;
        itemObject.IsShopUnlocked = true;
    }
}