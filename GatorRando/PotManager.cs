using System.Collections.Generic;

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
        {PotType.MC, "giant_socks"},
        {PotType.WW, "oar"},
        {PotType.LA, "mask"},
        {PotType.OoT, "guitar"},
        {PotType.TP, "tiger_form"},
    };

    static readonly Dictionary<int, PotType> pot_mapping = [];

    private static void PopulateMapping()
    {
        if (pot_mapping.Keys.Count == 0)
        {
            List<int> MC_pots = [88, 190, 233, 372, 407, 426, 1127, 1521, 1543, 1584, 1594, 1595, 2072, 2073]; //14
            List<int> WW_pots = [26, 63, 95, 102, 116, 118, 168, 176, 203, 235, 238, 243, 427, 1157, 1167, 1364, 1382, 1533, 1541, 1597, 1625, 2071]; //22
            List<int> LA_pots = [7, 80, 169, 180, 200, 220, 367, 412, 423, 425, 562, 652, 656, 695, 723, 1350, 1383, 1542, 1695, 1709, 1712, 2075]; //22
            List<int> OoT_pots = [78, 101, 217, 366, 492, 693, 1146, 1159, 1352, 1362, 1432, 1451, 1457, 1518, 1585, 1662, 2013]; //17
            List<int> TP_pots = [52, 70, 83, 191, 226, 229, 546, 547, 696, 722, 1124, 1195, 1351, 1363, 1381, 1445, 1464, 2074]; //18

            foreach (int id in MC_pots)
            {
                pot_mapping.Add(id, PotType.MC);
            }
            foreach (int id in WW_pots)
            {
                pot_mapping.Add(id, PotType.WW);
            }
            foreach (int id in LA_pots)
            {
                pot_mapping.Add(id, PotType.LA);
            }
            foreach (int id in OoT_pots)
            {
                pot_mapping.Add(id, PotType.OoT);
            }
            foreach (int id in TP_pots)
            {
                pot_mapping.Add(id, PotType.TP);
            }
        }
    }

    public static bool CheckIfPotBreakable(int id)
    {
        PopulateMapping();
        PotType potType;
        try
        {
            potType = pot_mapping[id];
        }
        catch (KeyNotFoundException)
        {
            Plugin.LogDebug($"You missed pot {id}");
            return false;
        }

        if (ArchipelagoManager.GetOptionBool(ArchipelagoManager.Option.LockPotsBehindItems))
        {
            return ArchipelagoManager.IsItemUnlocked(PotItem[potType]);
        }
        else
        {
            return true;
        }
    }
}