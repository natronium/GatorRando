using System;
using System.Collections.Generic;
using System.Linq;

namespace GatorRando.Archipelago;

public static class Options
{
    private static bool TryGetOptionBool(Option option)
    {
        try
        {
            return ConnectionManager.GetSlotDataOption(OptionName(option)) != "0";
        }
        catch (KeyNotFoundException)
        {
            // if game was not generated with an option, assume it is false
            return false;
        }
    }
    public static bool GetOptionBool(Option option) => TryGetOptionBool(option);

    public enum Option
    {
        StartWithFreeplay,
        RequireShieldJump,
        HarderRangedQuests,
        LockPotsBehindItems,
        LockChestsBehindKey,
        LockRacesBehindFlag,
    }

    public static string OptionName(Option option) => option switch
    {
        Option.StartWithFreeplay => "start_with_freeplay",
        Option.RequireShieldJump => "require_shield_jump",
        Option.HarderRangedQuests => "harder_ranged_quests",
        Option.LockPotsBehindItems => "lock_pots_behind_items",
        Option.LockChestsBehindKey => "lock_chests_behind_key",
        Option.LockRacesBehindFlag => "lock_races_behind_flag",
        _ => throw new Exception("Invalid enum value for Option"),
    };

    // TODO: Think about what to do if have non-bool options
    public static Dictionary<string, bool> GetOptions() =>
        Enum.GetValues(typeof(Option)).Cast<Option>().ToDictionary(OptionName, GetOptionBool);
}