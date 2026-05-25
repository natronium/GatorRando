
using System.Collections.Generic;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using UnityEngine;

namespace GatorRando.QuestMods;

public static class QuestItems
{
    public static readonly List<ItemObject> QuestItemObjects = [];
    internal static void AddAPQuestItems()
    {
        if (QuestItemObjects.Count == 0)
        {
            ItemObject glider = Util.GenerateItemObject("Glider", SpriteHandler.GetSpriteForItem("Shirt"));
            glider.IsUnlocked = ItemHandling.IsItemUnlocked("Shirt");
            QuestItemObjects.Add(glider);
            PlayerItemManager.p.gliderItem = glider; 
            QuestItemObjects.Add(Util.GenerateItemObject("QuestItem_Retainer", SpriteHandler.GetSpriteForItem("QuestItem_Retainer")));
            QuestItemObjects.Add(Util.GenerateItemObject("Broken Scooter Board", SpriteHandler.GetSpriteForItem("Shield_ScooterBoardGreen")));
            QuestItemObjects.Add(Util.GenerateItemObject("BEACH ROCK", SpriteHandler.GetSpriteForItem("BEACH ROCK")));
            QuestItemObjects.Add(Util.GenerateItemObject("HALF A CHEESE SANDWICH", SpriteHandler.GetSpriteForItem("HALF A CHEESE SANDWICH")));
            QuestItemObjects.Add(Util.GenerateItemObject("POT?", SpriteHandler.GetSpriteForItem("POT?")));
            QuestItemObjects.Add(Util.GenerateItemObject("ICE CREAM", SpriteHandler.GetSpriteForItem("ICE CREAM")));
            QuestItemObjects.Add(Util.GenerateItemObject("CLIPPINGS", SpriteHandler.GetSpriteForItem("CLIPPINGS")));
            QuestItemObjects.Add(Util.GenerateItemObject("WATER", SpriteHandler.GetSpriteForItem("WATER")));
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
        }
    }
}