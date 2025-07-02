using GatorRando.Archipelago;

namespace GatorRando;

public static class RaceManager
{
    public static bool CheckIfRaceAvailable()
    {
        if (Options.GetOptionBool(Options.Option.LockRacesBehindFlag))
        {
            return ItemHandling.IsItemUnlocked("flag");
        }
        else
        {
            return true;
        }
    }
}