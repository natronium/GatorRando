namespace GatorRando;

public static class ChestManager
{
    public static bool CheckIfChestBreakable()
    {
        if (ArchipelagoManager.GetOptionBool(ArchipelagoManager.Option.LockChestsBehindKey))
        {
            return ArchipelagoManager.IsItemUnlocked("key");
        }
        else
        {
            return true;
        }
    }

}