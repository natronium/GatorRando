namespace GatorRando;

public static class RaceManager
{
    public static bool CheckIfRaceAvailable()
    {
        if (ArchipelagoManager.GetOptionBool(ArchipelagoManager.Option.LockRacesBehindFlag))
        {
            return ArchipelagoManager.IsItemUnlocked("flag");
        }
        else
        {
            return true;
        }
    }
}