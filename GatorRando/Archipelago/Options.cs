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

    public enum GoalChoice
    {
        Main,
        ITD,
        Both,
    }

    public static GoalChoice GetGoalChoice()
    {
        try
        {
            return ConnectionManager.GetSlotDataOption("goal") switch
			{
				"0" => GoalChoice.Main,
                "1" => GoalChoice.ITD,
                "2" => GoalChoice.Both,
                _ => GoalChoice.Main,
			};
        }
        catch (KeyNotFoundException)
        {
            // if game was not generated with an option, assume it is old default
            return GoalChoice.Main;
        }
    }
    
    public static bool GetOptionBool(Option option) => TryGetOptionBool(option);

    public enum Option
    {
        StartWithFreeplay,
        AwkwardProgression,
        RequireVerticalForITD,
        RequireShieldFlip,
        HarderRangedQuests,
        LockPotsBehindItems,
        LockChestsBehindKey,
        LockRacesBehindFlag,
    }

    public static string OptionName(Option option) => option switch
    {
        Option.StartWithFreeplay => "start_with_freeplay",
        Option.AwkwardProgression => "awkward_progression",
        Option.RequireVerticalForITD => "require_vertical_for_itd",
        Option.RequireShieldFlip => "require_shield_flip",
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