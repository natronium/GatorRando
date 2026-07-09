using System.Collections.Generic;
using GatorRando.Archipelago;
using GatorRando.UIMods;

namespace GatorRando;

public static class PotManager
{
    public enum PotType
    {
        MC,
        WW,
        LA,
        OoT,
        TP,
    }

    private static readonly Dictionary<PotType, string> PotItem = new()
    {
        {PotType.MC, "Socks"},
        {PotType.WW, "Oar"},
        {PotType.LA, "SleepMask"},
        {PotType.OoT, "Guitar"},
        {PotType.TP, "Tiger"},
    };

    private static readonly Dictionary<int, PotType> surface_pot_mapping = [];
    private static readonly Dictionary<int, PotType> ug_pot_mapping = [];

    private static void PopulateMapping()
    {
        if (surface_pot_mapping.Keys.Count == 0)
        {
            List<int> MC_pots = [88, 190, 233, 372, 407, 426, 1127, 1521, 1543, 1584, 1594, 1595, 2072, 2073]; //14
            List<int> WW_pots = [26, 63, 95, 102, 116, 118, 168, 176, 203, 235, 238, 243, 427, 1157, 1167, 1364, 1382, 1533, 1541, 1597, 1625, 2071]; //22
            List<int> LA_pots = [7, 80, 169, 180, 200, 220, 367, 412, 423, 425, 562, 652, 656, 695, 723, 1350, 1383, 1542, 1695, 1709, 1712, 2075]; //22
            List<int> OoT_pots = [78, 101, 366, 492, 693, 1146, 1159, 1352, 1362, 1432, 1451, 1457, 1518, 1585, 1662, 2013]; //17
            List<int> TP_pots = [52, 70, 83, 191, 217, 226, 229, 546, 547, 696, 722, 1124, 1195, 1351, 1363, 1381, 1445, 1464, 2074]; //18

            foreach (int id in MC_pots)
            {
                surface_pot_mapping.Add(id, PotType.MC);
            }
            foreach (int id in WW_pots)
            {
                surface_pot_mapping.Add(id, PotType.WW);
            }
            foreach (int id in LA_pots)
            {
                surface_pot_mapping.Add(id, PotType.LA);
            }
            foreach (int id in OoT_pots)
            {
                surface_pot_mapping.Add(id, PotType.OoT);
            }
            foreach (int id in TP_pots)
            {
                surface_pot_mapping.Add(id, PotType.TP);
            }
        }
        if (ug_pot_mapping.Keys.Count == 0)
        {
            List<int> UG_WW_pots = [576, 792, 801, 894, 907, 1148, 1153, 1167];
            List<int> UG_LA_pots = [578, 597, 760, 876, 890, 906, 1161, 1334];
            List<int> UG_OoT_pots = [577, 579, 598, 840, 875, 908, 910, 931, 945, 1168, 1170, 1171];
            List<int> UG_TP_pots = [580, 595, 602, 873, 874, 981, 1166, 1180, 1213];

            // foreach (int id in MC_pots)
            // {
            //     surface_pot_mapping.Add(id, PotType.MC);
            // }
            foreach (int id in UG_WW_pots)
            {
                ug_pot_mapping.Add(id, PotType.WW);
            }
            foreach (int id in UG_LA_pots)
            {
                ug_pot_mapping.Add(id, PotType.LA);
            }
            foreach (int id in UG_OoT_pots)
            {
                ug_pot_mapping.Add(id, PotType.OoT);
            }
            foreach (int id in UG_TP_pots)
            {
                ug_pot_mapping.Add(id, PotType.TP);
            }
        }
    }
    private static PotType? GetPotType(int id)
    {
        PotType? potType;
        try
        {
            if (NavigationUI.currentLevel == Data.Locations.Level.Surface)
            {
                potType = surface_pot_mapping[id];
            }
            else
            {
                potType = ug_pot_mapping[id];
            }
        }
        catch (KeyNotFoundException)
        {
            Plugin.LogDebug($"You missed pot {id}");
            potType = null;
        }
        return potType;
    }

    public static bool CheckIfPotBreakable(int id)
    {
        PopulateMapping();
        PotType? potType = GetPotType(id);
        if (potType == null)
        {
            return false;
        }

        if (Options.GetOptionBool(Options.Option.LockPotsBehindItems))
        {
            return ItemHandling.IsItemUnlocked(PotItem[(PotType)potType]);
        }
        else
        {
            return true;
        }
    }

    public static string GetPotString(int id)
    {
        PotType? potType = GetPotType(id);
        if (potType == null)
        {
            return "I'm trying to break a pot I don't know about.";
        }
        if (Options.GetOptionBool(Options.Option.LockPotsBehindItems))
        {
            if (ItemHandling.IsItemUnlocked(PotItem[(PotType)potType]))
            {
                return potType switch
                {
                    PotType.MC => "I use my giant socks to become big and stomp around!",
                    PotType.WW => "I paddle through the fierce pond with my oar to calm the breeze!",
                    PotType.LA => "I help the tired monsters sleep through the night with my sleep mask!",
                    PotType.OoT => "I play my guitar of space to open the Sacred Place of Space!",
                    PotType.TP => "I change into a tiger to rescue the Dawn Prince!",
                    _ => throw new System.NotImplementedException(),
                };
            }
            else
            {
                return potType switch
                {
                    PotType.MC => "I need my GIANT SOCKS to become big and stomp around...",
                    PotType.WW => "I need my OAR to calm the breeze...",
                    PotType.LA => "I need my SLEEP MASK to help the tired monsters sleep...",
                    PotType.OoT => "I need my GUITAR OF SPACE to open the Sacred Place of Space...",
                    PotType.TP => "I need my TIGER FORM to rescue the Dawn Prince...",
                    _ => throw new System.NotImplementedException(),
                };
            }
        }
        return "Pots are easy to break!";
    }

    public static BubbleManager.UnimportantMessageType GetPotUnimportantMessageType(int id)
    {
        PotType? potType = GetPotType(id);
        return potType switch
        {
            PotType.MC => BubbleManager.UnimportantMessageType.MC,
            PotType.WW => BubbleManager.UnimportantMessageType.WW,
            PotType.LA => BubbleManager.UnimportantMessageType.LA,
            PotType.OoT => BubbleManager.UnimportantMessageType.OoT,
            PotType.TP => BubbleManager.UnimportantMessageType.TP,
            _ => throw new System.NotImplementedException(),
        };
    }
}