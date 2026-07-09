
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
            ItemObject glider = Util.GenerateItemObject("Glider", SpriteHandler.GetSpriteForItem("Shirt"), "Glider", "jump twice to glide");
            QuestItemObjects.Add(Util.GenerateItemObject("CHARM KEYCHAIN", SpriteHandler.GetSpriteForItem("CHARM KEYCHAIN"),"Charm Keychain","now i can use charm points"));
            glider.IsUnlocked = ItemHandling.IsItemUnlocked("Shirt");
            QuestItemObjects.Add(glider);
            PlayerItemManager.p.gliderItem = glider; 
            QuestItemObjects.Add(Util.GenerateItemObject("QuestItem_Retainer", SpriteHandler.GetSpriteForItem("QuestItem_Retainer"), "Retainer", "i should return the retainer to Becca"));
            QuestItemObjects.Add(Util.GenerateItemObject("Broken Scooter Board", SpriteHandler.GetSpriteForItem("Shield_ScooterBoardGreen"), "Broken Scooter Board", "i should return the broken scooter to Kaden"));
            QuestItemObjects.Add(Util.GenerateItemObject("BEACH ROCK", SpriteHandler.GetSpriteForItem("BEACH ROCK"), "Magic Ore", "i should return the magic ore to Susanne"));
            QuestItemObjects.Add(Util.GenerateItemObject("HALF A CHEESE SANDWICH", SpriteHandler.GetSpriteForItem("HALF A CHEESE SANDWICH"), "HALF A CHEESE SANDWICH", "i should trade the yellow triangle to Gene"));
            QuestItemObjects.Add(Util.GenerateItemObject("POT?", SpriteHandler.GetSpriteForItem("POT?"), "Pot?", "i should give this to Martin on our starting island"));
            QuestItemObjects.Add(Util.GenerateItemObject("ICE CREAM", SpriteHandler.GetSpriteForItem("ICE CREAM"), "Sorbet", "i should give the sorbet to Esme"));
            QuestItemObjects.Add(Util.GenerateItemObject("CLIPPINGS", SpriteHandler.GetSpriteForItem("CLIPPINGS"), "Grass Clippings", "i should help Jada support magnolia"));
            QuestItemObjects.Add(Util.GenerateItemObject("WATER", SpriteHandler.GetSpriteForItem("WATER"), "Water", "i should help Jada water magnolia"));
            Sprite pencil = SpriteHandler.GetSpriteForItem("Thrown_Pencil");
            string pencilName = "Thrown Pencil";
            string pencilDescription = "i should give Sam back his thrown pencils";
            QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil", pencil, pencilName, pencilDescription));
            QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil_2", pencil, pencilName, pencilDescription));
            QuestItemObjects.Add(Util.GenerateItemObject("Thrown_Pencil_3", pencil, pencilName, pencilDescription));

            QuestItemObjects.Add(Util.GenerateItemObject("Key", SpriteHandler.GetSpriteForItem("Key"), "Key", "i can unlock chests with my key"));
            QuestItemObjects.Add(Util.GenerateItemObject("Flag", SpriteHandler.GetSpriteForItem("Flag"), "Flag", "i can run races with my finish flag"));
            QuestItemObjects.Add(Util.GenerateItemObject("SleepMask", SpriteHandler.GetSpriteForItem("SleepMask"), "Sleep Mask", "i can help tired monsters sleep through the night with my sleep mask (and break some pots)"));
            QuestItemObjects.Add(Util.GenerateItemObject("Tiger", SpriteHandler.GetSpriteForItem("Tiger"), "Tiger Form", "i can change into a tiger to rescue the Dawn Prince  (and break some pots)"));
            QuestItemObjects.Add(Util.GenerateItemObject("Socks", SpriteHandler.GetSpriteForItem("Socks"), "Giant Socks", "i can use my giant socks to become big and stomp around (and break some pots)"));
            QuestItemObjects.Add(Util.GenerateItemObject("Guitar", SpriteHandler.GetSpriteForItem("Guitar"), "Guitar of Space", "i can play my guitar of space to open the Sacred Place of Space (and break some pots)"));
            QuestItemObjects.Add(Util.GenerateItemObject("Oar", SpriteHandler.GetSpriteForItem("Oar"),"Oar","i can paddle through the fierce pond with my oar to calm the breeze (and break some pots)"));

            QuestItemObjects.Add(Util.GenerateItemObject("First Queen Letter", SpriteHandler.GetSpriteForItem("First Queen Letter"),"Queen's Secret Letter","i should give this letter to either jane or lola"));
            QuestItemObjects.Add(Util.GenerateItemObject("Second Queen Letter", SpriteHandler.GetSpriteForItem("Second Queen Letter"),"Other Queen's Secret Letter","i should give this letter to either jane or lola"));
            QuestItemObjects.Add(Util.GenerateItemObject("Clam", SpriteHandler.GetSpriteForItem("Clam"),"Clam","i can now carry val to safety"));
            QuestItemObjects.Add(Util.GenerateItemObject("Thorny", SpriteHandler.GetSpriteForItem("Thorny"),"Bramble Stalker","i found the bramble stalker and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Bubbly", SpriteHandler.GetSpriteForItem("Bubbly"),"Orbhound","i found the orbhound and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Cakey", SpriteHandler.GetSpriteForItem("Cakey"),"Iced Liar","i found the iced liar and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Holy", SpriteHandler.GetSpriteForItem("Holy"),"Bird of Big Island","i found the bird of big island and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Finny", SpriteHandler.GetSpriteForItem("Finny"),"Gulfcoaster","i found the gulfcoaster and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Drippy", SpriteHandler.GetSpriteForItem("Drippy"),"Wellerdropple","i found the wellerdropple and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Floofy", SpriteHandler.GetSpriteForItem("Floofy"),"Barber Worm","i found the barber worm and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Treey", SpriteHandler.GetSpriteForItem("Treey"),"Yarrowling","i found the yarrowling and should bring it to nodd"));
            QuestItemObjects.Add(Util.GenerateItemObject("Looky", SpriteHandler.GetSpriteForItem("Looky"),"Peep Goblin","i found the peep goblin and should bring it to nodd"));

        }
    }
}