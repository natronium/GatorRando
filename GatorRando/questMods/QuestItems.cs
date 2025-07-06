
using System.Collections.Generic;
using GatorRando.UIMods;
using UnityEngine;

namespace GatorRando.QuestMods;

static class QuestItems
{
    public static readonly List<ItemObject> QuestItemObjects = [];
    public static void AddItems()
    {
        if (QuestItemObjects.Count == 0)
        {
            QuestItemObjects.Add(Util.GenerateItemObject("Shirt", SpriteHandler.GetSpriteForItem("Shirt")));
            QuestItemObjects.Add(Util.GenerateItemObject("QuestItem_Retainer", SpriteHandler.GetSpriteForItem("QuestItem_Retainer")));
            QuestItemObjects.Add(Util.GenerateItemObject("Broken Scooter Board", SpriteHandler.GetSpriteForItem("Shield_ScooterBoardGreen")));

            // QuestItemObjects.Add(Util.FindItemObjectByName("QuestItem_Retainer"));

            // DSItem rock = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Engineer/Rock Get Sequence").GetComponent<DSItem>();
            QuestItemObjects.Add(Util.GenerateItemObject("BEACH ROCK", SpriteHandler.GetSpriteForItem("BEACH ROCK")));

            // DSItem cheese_sandwich = Util.GetByPath("West (Forest)/Prep Quest/Subquests/Economist/Loot Get Sequence").GetComponent<DSItem>();
            QuestItemObjects.Add(Util.GenerateItemObject("HALF A CHEESE SANDWICH", SpriteHandler.GetSpriteForItem("HALF A CHEESE SANDWICH")));

            // DSItem pot = Util.GetByPath("NorthWest (Tutorial Island)/Act 1/Quests/Martin Quest/Get Pot Lid").GetComponent<DSItem>();
            QuestItemObjects.Add(Util.GenerateItemObject("POT?", SpriteHandler.GetSpriteForItem("POT?")));

            // DSItem iceCream = Util.GetByPath("North (Mountain)/Theatre Quest/Subquests/Vampire/Get Ice Cream").GetComponent<DSItem>();
            QuestItemObjects.Add(Util.GenerateItemObject("ICE CREAM", SpriteHandler.GetSpriteForItem("ICE CREAM")));

            // DSItem clippings = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Grass Sequence").GetComponent<DSItem>();
            QuestItemObjects.Add(Util.GenerateItemObject("CLIPPINGS", SpriteHandler.GetSpriteForItem("CLIPPINGS")));

            // DSItem waterItem = Util.GetByPath("East (Creeklands)/Cool Kids Quest/Subquests/Boar Quest/Got Enough Water Sequence").GetComponent<DSItem>();
            QuestItemObjects.Add(Util.GenerateItemObject("WATER", SpriteHandler.GetSpriteForItem("WATER")));

            // ItemObject pencil = Util.FindItemObjectByName("Sword_Pencil");
            // QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil", pencil.sprite));
            // QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil_2", pencil.sprite));
            // QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil_3", pencil.sprite));

            Sprite pencil = SpriteHandler.GetSpriteForItem("Thrown_Pencil");
            QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil", pencil));
            QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil_2", pencil));
            QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil_3", pencil));

            QuestItemObjects.Add(Util.GenerateItemObject("Key", SpriteHandler.GetSpriteForItem("Key")));
            QuestItemObjects.Add(Util.GenerateItemObject("Flag", SpriteHandler.GetSpriteForItem("Flag")));
            QuestItemObjects.Add(Util.GenerateItemObject("SleepMask", SpriteHandler.GetSpriteForItem("SleepMask")));
            QuestItemObjects.Add(Util.GenerateItemObject("Tiger", SpriteHandler.GetSpriteForItem("Tiger")));
            QuestItemObjects.Add(Util.GenerateItemObject("Socks", SpriteHandler.GetSpriteForItem("Socks")));
            QuestItemObjects.Add(Util.GenerateItemObject("Guitar", SpriteHandler.GetSpriteForItem("Guitar")));
            QuestItemObjects.Add(Util.GenerateItemObject("Oar", SpriteHandler.GetSpriteForItem("Oar")));

            QuestItemObjects.Add(Util.GenerateItemObject("Archipelago", SpriteHandler.GetSpriteForItem("Archipelago")));

            
        }
    }
}