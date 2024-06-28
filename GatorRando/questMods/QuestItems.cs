
using UnityEngine;

namespace GatorRando.QuestMods;

static class QuestItems
{
    public static void Items()
    {
        GameObject rock_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence");
        DSItem rock = rock_seq.GetComponent<DSItem>();
        ItemObject magic_ore = new()
        {
            name = "BEACH ROCK",
            sprite = rock.itemSprite
        };

        GameObject loot_seq = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence");
        DSItem cheese_sandwich = loot_seq.GetComponent<DSItem>();
        ItemObject sandwich = new()
        {
            name = "HALF A CHEESE SANDWICH",
            sprite = cheese_sandwich.itemSprite
        };

        GameObject get_pot_lid = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid");
        DSItem pot = get_pot_lid.GetComponent<DSItem>();
        ItemObject pot_q = new()
        {
            name = "POT?",
            sprite = pot.itemSprite
        };

        GameObject get_ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream");
        DSItem ice_cream = get_ice_cream.GetComponent<DSItem>();
        ItemObject sorbet = new()
        {
            name = "ICE CREAM",
            sprite = ice_cream.itemSprite
        };

        GameObject grass_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence");
        DSItem clippings = grass_seq.GetComponent<DSItem>();
        ItemObject grass_clippings = new()
        {
            name = "CLIPPINGS",
            sprite = clippings.itemSprite
        };

        GameObject water_seq = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence");
        DSItem water_item = water_seq.GetComponent<DSItem>();
        ItemObject water = new()
        {
            name = "WATER",
            sprite = water_item.itemSprite
        };

        ItemObject pencil = Util.FindItemObjectByName("Sword_Pencil");
        ItemObject thrown_pencil = new()
        {
            name = "Thrown_Pencil",
            sprite = pencil.sprite
        };



    }
}