
using System.Collections.Generic;
using UnityEngine;

namespace GatorRando.QuestMods;

static class QuestItems
{
    private static readonly List<ItemObject> CustomItemObjects = [];

    // private static ItemObject GenerateItemObject(string name, Sprite sprite) =>
    //     new()
    //     {
    //         id = name,
    //         name = name,
    //         sprite = sprite
    //     };

    // private static ItemObject GenerateItemObjectOld(string name, Sprite sprite) =>
    //     new()
    //     {
    //         id = name,
    //         name = name,
    //         sprite = sprite
    //     };
    public static void AddItems()
    {
        DSItem rock = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence").GetComponent<DSItem>();
        CustomItemObjects.Add(Util.GenerateItemObject("BEACH ROCK", rock.itemSprite));

        DSItem cheese_sandwich = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence").GetComponent<DSItem>();
        CustomItemObjects.Add(Util.GenerateItemObject("HALF A CHEESE SANDWICH", cheese_sandwich.itemSprite));

        DSItem pot = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid").GetComponent<DSItem>();
        CustomItemObjects.Add(Util.GenerateItemObject("POT?", pot.itemSprite));

        DSItem ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream").GetComponent<DSItem>();
        CustomItemObjects.Add(Util.GenerateItemObject("ICE CREAM", ice_cream.itemSprite));

        DSItem clippings = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence").GetComponent<DSItem>();
        CustomItemObjects.Add(Util.GenerateItemObject("CLIPPINGS", clippings.itemSprite));

        DSItem water_item = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence").GetComponent<DSItem>();
        CustomItemObjects.Add(Util.GenerateItemObject("WATER", water_item.itemSprite));

        ItemObject pencil = Util.FindItemObjectByName("Sword_Pencil");
        CustomItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil", pencil.sprite));
    }
}