
using System.Collections.Generic;

namespace GatorRando.QuestMods;

static class QuestItems
{
    public static readonly List<ItemObject> QuestItemObjects = [];
    public static void AddItems()
    {
        DSItem rock = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence").GetComponent<DSItem>();
        QuestItemObjects.Add(Util.GenerateItemObject("BEACH ROCK", rock.itemSprite));

        DSItem cheese_sandwich = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence").GetComponent<DSItem>();
        QuestItemObjects.Add(Util.GenerateItemObject("HALF A CHEESE SANDWICH", cheese_sandwich.itemSprite));

        DSItem pot = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid").GetComponent<DSItem>();
        QuestItemObjects.Add(Util.GenerateItemObject("POT?", pot.itemSprite));

        DSItem ice_cream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream").GetComponent<DSItem>();
        QuestItemObjects.Add(Util.GenerateItemObject("ICE CREAM", ice_cream.itemSprite));

        DSItem clippings = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence").GetComponent<DSItem>();
        QuestItemObjects.Add(Util.GenerateItemObject("CLIPPINGS", clippings.itemSprite));

        DSItem water_item = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence").GetComponent<DSItem>();
        QuestItemObjects.Add(Util.GenerateItemObject("WATER", water_item.itemSprite));

        ItemObject pencil = Util.FindItemObjectByName("Sword_Pencil");
        QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil", pencil.sprite));

        QuestItemObjects.Add(Util.FindItemObjectByName("QuestItem_Retainer"));
    }
}