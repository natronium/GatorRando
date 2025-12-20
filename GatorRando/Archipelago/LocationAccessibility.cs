using System;
using System.Collections.Generic;
using System.Linq;
using GatorRando.Data;

namespace GatorRando.Archipelago;

public static class LocationAccessibilty
{
    private static readonly string[] excludedNPCs = ["NPC_LunchSwapCardinal", "NPC_Bee", "NPC_Ninja_Tiger", "NPC_SwimSheep", "Dialogue Actor No Longer Exists"];
    private static readonly string[] filteredNPCs = ["NPC_WannaBeHawk", "NPC_BigSis", "NPC_Destroy_Elephant", "NPC_Chess_Eagle", "NPC_Warrior_Beaver", "NPC_Obstacle_Ostrich",
            "NPC_Theatre_Cow1", "NPC_Theatre_Cow2", "NPC_Theatre_Cow3", "NPC_FetchVulture", "NPC_SurfOpossum", "NPC_TripLizard", "Signs"];

    public static bool IsNPCinExcludedOrFiltered(string npcId)
    {
        return excludedNPCs.Contains(npcId) || filteredNPCs.Contains(npcId);
    }
    private static readonly List<long> AccessibleLocations = [];

    public static void UpdateAccessibleLocations()
    {
        IEnumerable<long> inaccessibleIds = Locations.locationData.Where(data => !AccessibleLocations.Contains(data.apLocationId)).Select(data => data.apLocationId);
        foreach (long inaccessibleId in inaccessibleIds)
        {

            if (Rules.GatorRules.Rules[inaccessibleId].Evaluate())
            {
                AccessibleLocations.Add(inaccessibleId);
            }
        }
    }

    public static bool IsLocationAccessible(string gatorName)
    {
        if (excludedNPCs.Contains(gatorName) || LocationHandling.IsLocationCollected(gatorName))
        {
            return false;
        }
        try
        {
            return AccessibleLocations.Contains(LocationHandling.GetLocationApId(gatorName));
        }
        catch (InvalidOperationException)
        {
            Plugin.LogWarn($"Tried to check accessibility of location {gatorName}, which did not correspond to an AP location.");
            return false;
        }
    }

    public static bool IsLocationACheck(string gatorName)
    {
        if (excludedNPCs.Contains(gatorName) || LocationHandling.IsLocationCollected(gatorName))
        {
            return false;
        }
        try
        {
            Locations.locationData.First(entry => entry.clientNameId == gatorName);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        return true;
    }

    public static string ConvertDAToGatorName(DialogueActor dialogueActor)
    {
        try
        {
            string gatorName = dialogueActor.profile.id switch
            {
                "NPC_Tut_Dog" => DetectJillVariant(dialogueActor),
                "NPC_Tut_Frog" => DetectAveryVariant(dialogueActor),
                "NPC_Tut_Horse" => DetectMartinVariant(dialogueActor),
                "NPC_Cool_Goose" when dialogueActor.transform.parent.name != "Goose Quest" => "Unhandled Duke",
                "NPC_Cool_Boar" when dialogueActor.transform.parent.name != "Boar Quest" => "Unhandled Jada",
                "NPC_Cool_Wolf" when dialogueActor.transform.parent.name != "Wolf Quest" => "Unhandled Lucas",
                "NPC_Susanne" when dialogueActor.transform.parent.name != "Character" => "Unhandled Susanne",
                "NPC_Gene" when dialogueActor.transform.parent.name != "Character" => "Unhandled Gene",
                "NPC_Antone" when dialogueActor.transform.parent.name != "Character" => "Unhandled Antone",
                "NPC_Theatre_Cowboy" when dialogueActor.transform.parent.name != "Cowfolk" => "Unhandled Velma",
                "NPC_Theatre_Space" when dialogueActor.transform.parent.name != "Space!!!" => "Unhandled Andromeda",
                "NPC_Theatre_Bat" when dialogueActor.transform.parent.name != "Vampire" => "Unhandled Esme",
                string name => name
            };
            return gatorName;
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    private static string DetectMartinVariant(DialogueActor dialogueActor)
    {
        try
        {
            return dialogueActor.transform.parent.name switch
            {
                "Martin Quest" => "Tutorial Martin",
                "Cool CoolKids" => "Cool Kid Introduction Martin",
                _ => "Unhandled Martin"
            };
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    private static string DetectAveryVariant(DialogueActor dialogueActor)
    {
        try
        {
            return dialogueActor.transform.parent.name switch
            {
                "Avery Quest" => "Tutorial Avery",
                "Introduction" => "Theatre Introduction Avery",
                "Unhappy" => "Theatre Finale Avery",
                _ => "Unhandled Avery"
            };
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    private static string DetectJillVariant(DialogueActor dialogueActor)
    {
        try
        {
            return dialogueActor.transform.parent.name switch
            {
                "Fantasy" => "Tutorial Stick Jill",
                "Studying" => "Tutorial End Jill",
                "Intro Actors" => "Prep Introduction Jill",
                "Sad Jill" => "Prep Pre-Finale Jill",
                "End Actors" => "Prep Finale Jill",
                _ => "Unhandled Jill"
            };
        }
        catch (NullReferenceException)
        {
            return "Dialogue Actor No Longer Exists";
        }
    }

    public static bool IsMainQuestAccessible(DialogueActor dialogueActor)
    {
        if (IsMainQuestACheck(dialogueActor))
        {
            string gatorName = ConvertDAToGatorName(dialogueActor);
            string[] checks = ChecksForNPC(gatorName);
            return checks.Select(IsLocationAccessible).Any(b => b);
        }
        return false;
    }

    public static bool IsMainQuestACheck(DialogueActor dialogueActor)
    {
        string gatorName = ConvertDAToGatorName(dialogueActor);
        // filter out non-main quest NPCs from actors pulled to populate the Main Quest list
        if (gatorName == "" || gatorName.Contains("Unhandled") || filteredNPCs.Contains(gatorName))
        {
            return false;
        }
        string[] checks = ChecksForNPC(gatorName);
        return checks.Select(IsLocationACheck).Any(b => b);
    }

    public static bool IsNPCACheck(DialogueActor dialogueActor)
    {
        string gatorName = dialogueActor.profile.id;
        if (gatorName == "NPC_BraceletMonkey")
        {
            gatorName = WhichBraceletMonkey(dialogueActor);
        }
        string[] checks = ChecksForNPC(gatorName);
        return checks.Select(IsLocationACheck).Any(b => b);
    }

    public static bool IsNPCAccessible(DialogueActor dialogueActor)
    {
        string gatorName = dialogueActor.profile.id;
        if (gatorName == "NPC_BraceletMonkey")
        {
            gatorName = WhichBraceletMonkey(dialogueActor);
        }
        if (gatorName == "Dialogue Actor No Longer Exists")
        {
            return false;
        }
        string[] checks = ChecksForNPC(gatorName);
        return checks.Select(IsLocationAccessible).Any(b => b);
    }

    private static string WhichBraceletMonkey(DialogueActor dialogueActor)
    {
        try
        {
            if (dialogueActor.transform.parent.parent.name == "North (Mountain)")
            {
                return "BraceletShop3";
            }
            else
            {
                return dialogueActor.transform.parent.parent.parent.name switch
                {
                    "NorthWest (Tutorial Island)" => "BraceletShop2",
                    "East (Creeklands)" => "BraceletShop1",
                    "West (Forest)" => "BraceletShop0",
                    _ => "Unhandled"
                };
            }
        }
        catch (NullReferenceException)
        {
            // When a Bracelet Monkey has been purchased, they need to be excluded from the check
            return "AlreadyPurchased";
        }
    }

    public static bool IsLocationAccessible(PersistentObject gatorObject)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(gatorObject);
        
        if (persistentObjectType == Util.PersistentObjectType.Pot || persistentObjectType == Util.PersistentObjectType.Chest || persistentObjectType == Util.PersistentObjectType.Race)
        {
            int gatorID = LocationHandling.ConvertTannerIds(gatorObject.id);
            try
            {
                if (LocationHandling.IsLocationCollected(gatorID))
                {
                    return false;
                }
                return AccessibleLocations.Contains(LocationHandling.GetLocationApId(gatorID));
            }
            catch (InvalidOperationException)
            {
                // Plugin.LogWarn($"Tried to check accessibility of location {gatorID}, which did not correspond to an AP location.");
                return false;
            }
        }
        else if (persistentObjectType == Util.PersistentObjectType.Cardboard)
        {
            return false;
        }
        else
        {
            Plugin.LogWarn($"Tried to check accessibility of location {gatorObject.id}, which is not a Pot, Race, Chest, or a BreakableObject.");
            return false;
        }
    }
    public static bool IsLocationACheck(PersistentObject gatorObject)
    {
        Util.PersistentObjectType persistentObjectType = Util.GetPersistentObjectType(gatorObject);
        if (persistentObjectType == Util.PersistentObjectType.Pot || persistentObjectType == Util.PersistentObjectType.Chest || persistentObjectType == Util.PersistentObjectType.Race)
        {
            return true;
        }
        else if (persistentObjectType == Util.PersistentObjectType.Cardboard)
        {
            return false;
        }
        else
        {
            Plugin.LogWarn($"Tried to check if location {gatorObject.id} is a check, which is not a Pot, Race, Chest, or a BreakableObject.");
            return false;
        }
    }

    public static string[] ChecksForNPC(string gatorName)
    {
        return gatorName switch
        {
            "NPC_TutIsland_Duck" => ["NPC_TutIsland_Duck", "Sword_Wood"],
            "NPC_TutIsland_Bear" => ["NPC_TutIsland_Bear", "Item_Ragdoll"],
            "NPC_TutIsland_Giraffe" => ["NPC_TutIsland_Giraffe", "Hat_Slime"],
            "NPC_Balloon_Owl" => ["NPC_Balloon_Owl", "Item_Balloon"],
            "NPC_FetchVulture" => ["NPC_FetchVulture", "BROKEN WHEELIE THINGY", "Shield_ScooterBoardBlue"],
            "NPC_SurfOpossum" => ["NPC_SurfOpossum", "Shield_Tube"],
            "NPC_Chess_Eagle" => ["NPC_Chess_Eagle", "Shield_Chessboard"],
            "NPC_Clumsy_Jackal" => ["NPC_Clumsy_Jackal", "Sword_Pencil", "Thrown_Pencil_1", "Thrown_Pencil_2", "Thrown_Pencil_3"],
            "NPC_Old_Man_Sloth" => ["NPC_Old_Man_Sloth", "Item_Gum"],
            "NPC_Skate_Pug" => ["NPC_Skate_Pug", "Shield_Skateboard"],
            "NPC_Rescue_Robin" => ["NPC_Rescue_Robin", "Shield_Palette"],
            "NPC_Fetch_Shark" => ["NPC_Fetch_Shark", "QuestItem_Retainer"],
            "NPC_Destroy_Rabbit" => ["NPC_Destroy_Rabbit", "Sword_Paintbrush"],
            "NPC_Ninja_Crane" => ["NPC_Ninja_Crane", "Item_Shuriken"],
            "NPC_Ninja_Anteater" => ["NPC_Ninja_Anteater", "Sword_Nunchucks", "Hat_Ninja"],
            "NPC_Obstacle_Ostrich" => ["NPC_Obstacle_Ostrich", "Hat_SkateHelmet"],
            "NPC_LunchSwapSkink" => ["NPC_LunchSwapSkink", "Hat_Princess"],
            "NPC_Warrior_Beaver" => ["NPC_Warrior_Beaver", "Shield_TowerShield"],
            "NPC_Shy_Cat" => ["NPC_Shy_Cat", "Item_Camera"],
            "NPC_TutuCat" => ["NPC_TutuCat", "Sword_Wand"],
            "NPC_BombBowlMole" => ["NPC_BombBowlMole", "Item_Bomb"],
            "NPC_BigWhale" => ["NPC_BigWhale", "Hat_Space"],
            "NPC_Rock_Fox" => ["NPC_Rock_Fox", "Rock"],
            "NPC_Mistakes_Rat" => ["NPC_Mistakes_Rat", "Hat_Beret"],
            "NPC_TripLizard" => ["NPC_TripLizard", "Sword_CBSpear"],
            "NPC_TrashPanda" => ["NPC_TrashPanda", "Item_StickyHand", "Shield_Stretch", "Shield_TrashCanLid", "Sword_Wrench", "Item_PaintGun", "Sword_Grabby"],
            "Tutorial Stick Jill" => ["Sword_Stick"],
            "Tutorial End Jill" => ["NPC_TutIsland_Duck", "Sword_Wood"],
            "Prep Introduction Jill" => ["BEACH ROCK", "Sword_RockHammer", "HALF A CHEESE SANDWICH", "Shield_Platter", "Sword_Net", "NPC_Tut_Dog"],
            "Prep Pre-Finale Jill" => ["NPC_Tut_Dog"],
            "Prep Finale Jill" => ["NPC_Tut_Dog"],
            "NPC_Susanne" => ["BEACH ROCK", "Sword_RockHammer","NPC_Tut_Dog"],
            "NPC_Gene" => ["HALF A CHEESE SANDWICH", "Shield_Platter","NPC_Tut_Dog"],
            "NPC_Antone" => ["Sword_Net","NPC_Tut_Dog"],
            "Tutorial Avery" => ["Hat_Basic", "Shirt"],
            "Theatre Introduction Avery" => ["NPC_Tut_Frog", "Item_SpaceGun", "Sword_Laser", "Hat_Western", "ICE CREAM", "Hat_Vampire", "NPC_Part-Timer"],
            "Theatre Finale Avery" => ["NPC_Tut_Frog"],
            "NPC_Theatre_Cowboy" => ["Hat_Western", "NPC_Tut_Frog"],
            "NPC_Theatre_Space" => ["Item_SpaceGun", "Sword_Laser", "NPC_Tut_Frog"],
            "NPC_Theatre_Bat" => ["ICE CREAM", "Hat_Vampire", "NPC_Part-Timer", "NPC_Tut_Frog"],
            "NPC_Part-Timer" => ["ICE CREAM", "NPC_Tut_Frog"],
            "Tutorial Martin" => ["POT?", "Shield_PotLid"],
            "Cool Kid Introduction Martin" => ["NPC_Tut_Horse", "Shield_Martin", "Hat_DetectiveCowl", "CLIPPINGS", "BUCKET", "WATER", "Shield_Leaf"],
            "NPC_Cool_Goose" => ["Hat_DetectiveCowl", "NPC_Tut_Horse"],
            "NPC_Cool_Boar" => ["CLIPPINGS", "BUCKET", "WATER", "Shield_Leaf", "NPC_Tut_Horse"],
            "NPC_Cool_Wolf" => ["Shield_Martin", "NPC_Tut_Horse"],
            "BraceletShop0" => ["BraceletShop0", "NPC_BraceletMonkey"],
            "BraceletShop1" => ["BraceletShop1", "NPC_BraceletMonkey"],
            "BraceletShop2" => ["BraceletShop2", "NPC_BraceletMonkey"],
            "BraceletShop3" => ["BraceletShop3", "NPC_BraceletMonkey"],
            _ => [gatorName]
        };
    }
}