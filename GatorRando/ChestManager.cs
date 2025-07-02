using GatorRando.Archipelago;

namespace GatorRando;

public static class ChestManager
{
    public static bool CheckIfChestBreakable()
    {
        if (Options.GetOptionBool(Options.Option.LockChestsBehindKey))
        {
            return ItemHandling.IsItemUnlocked("key");
        }
        else
        {
            return true;
        }
    }

}