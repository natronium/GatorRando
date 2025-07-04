using System.Collections.Generic;

namespace GatorRando.Data;

public static class Items
{
    public enum ClientItemType
    {
        Friend,
        CraftStuff,
        Bracelet,
        Item,
        Craft,
    }

    public enum ItemGroup
    {
        Friends,
        CraftingMaterials,
        Traversal,
        Hat,
        QuestItem,
        Sword,
        Shield,
        Ranged,
        Craft,
        Item,
        CardboardDestroyer,
        Unlock
    }

    public static string GetItemGroupName(ItemGroup group) => group switch
    {
        ItemGroup.Friends => "Friends",
        ItemGroup.CraftingMaterials => "Crafting Materials",
        ItemGroup.Traversal => "Traversal",
        ItemGroup.Hat => "Hat",
        ItemGroup.QuestItem => "Quest Item",
        ItemGroup.Sword => "Sword",
        ItemGroup.Shield => "Shield",
        ItemGroup.Ranged => "Ranged",
        ItemGroup.Craft => "Craft",
        ItemGroup.Item => "Item",
        ItemGroup.CardboardDestroyer => "Cardboard Destroyer",
        ItemGroup.Unlock => "Unlock",
        _ => throw new System.NotImplementedException(),
    };

    public readonly struct Item(
        string name,
        long apItemId,
        string clientNameId,
        int? clientResourceAmount,
        ClientItemType clientItemType,
        ItemGroup[] itemGroups
    )
    {
        public readonly string name = name;
        public readonly long apItemId = apItemId;
        public readonly string clientNameId = clientNameId;
        public readonly int? clientResourceAmount = clientResourceAmount;
        public readonly ClientItemType clientItemType = clientItemType;
        public readonly ItemGroup[] itemGroups = itemGroups;
    }
    public readonly static List<Item> itemData = [
        new Item("Friend", 100000001, "", 1, ClientItemType.Friend, [ItemGroup.Friends]),
        new Item("Friend x2", 100000002, "", 2, ClientItemType.Friend, [ItemGroup.Friends]),
        new Item("Friend x3", 100000003, "", 3, ClientItemType.Friend, [ItemGroup.Friends]),
        new Item("Friend x4", 100000004, "", 4, ClientItemType.Friend, [ItemGroup.Friends]),
        new Item("Craft Stuff x15", 100000005, "", 15, ClientItemType.CraftStuff, [ItemGroup.CraftingMaterials]),
        new Item("Craft Stuff x30", 100000006, "", 30, ClientItemType.CraftStuff, [ItemGroup.CraftingMaterials]),
        new Item("Bracelet", 100000007, "Bracelet", null, ClientItemType.Item, [ItemGroup.Traversal]),
        new Item("Glider", 100000008, "Shirt", null, ClientItemType.Item, [ItemGroup.Traversal]),
        new Item("Retainer", 100000009, "QuestItem_Retainer", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Magic Ore", 100000010, "BEACH ROCK", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Broken Scooter Board", 100000011, "Shield_ScooterBoardGreen", null, ClientItemType.Craft, [ItemGroup.QuestItem, ItemGroup.Craft]),
        new Item("Cheese Sandwich", 100000012, "HALF A CHEESE SANDWICH", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Pot?", 100000013, "POT?", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Sorbet", 100000014, "ICE CREAM", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Grass Clippings", 100000015, "CLIPPINGS", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Water", 100000016, "WATER", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Pointy Floppy Thing", 100000017, "Hat_Basic", null, ClientItemType.Craft, [ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Slime Hat", 100000018, "Hat_Slime", null, ClientItemType.Craft, [ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Artsy Beret", 100000019, "Hat_Beret", null, ClientItemType.Craft, [ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Space Dome", 100000020, "Hat_Space", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.Hat]),
        new Item("Plastic Fangs", 100000021, "Hat_Vampire", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.Hat]),
        new Item("Western Wide Brim", 100000022, "Hat_Western", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.Hat]),
        new Item("Bucket", 100000023, "Hat_Bucket", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.Hat]),
        new Item("Detective Cowl", 100000024, "Hat_DetectiveCowl", null, ClientItemType.Craft, [ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Skater Helmet", 100000025, "Hat_SkateHelmet", null, ClientItemType.Craft, [ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Princess Tiara", 100000026, "Hat_Princess", null, ClientItemType.Craft, [ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Ninja Headband", 100000027, "Hat_Ninja", null, ClientItemType.Craft, [ItemGroup.Traversal, ItemGroup.Hat, ItemGroup.Craft]),
        new Item("Stick", 100000028, "Sword_Stick", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Sword", 100000029, "Sword_Wood", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Paintbrush", 100000030, "Sword_Paintbrush", null, ClientItemType.Craft, [ItemGroup.Sword, ItemGroup.Craft]),
        new Item("Cardboard Spear", 100000031, "Sword_CBSpear", null, ClientItemType.Craft, [ItemGroup.Sword, ItemGroup.Craft]),
        new Item("Grabby Hand", 100000032, "Sword_Grabby", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Laser Sword", 100000033, "Sword_Laser", null, ClientItemType.Craft, [ItemGroup.Sword, ItemGroup.Craft]),
        new Item("Bug Net", 100000034, "Sword_Net", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Nunchaku", 100000035, "Sword_Nunchucks", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Thrown Pencil", 100000036, "Thrown_Pencil", null, ClientItemType.Item, [ItemGroup.QuestItem]),
        new Item("Oversized Pencil", 100000037, "Sword_Pencil", null, ClientItemType.Craft, [ItemGroup.Sword, ItemGroup.Craft]),
        new Item("Wrench", 100000038, "Sword_Wrench", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Paleolithic Tool", 100000039, "Sword_RockHammer", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Sword]),
        new Item("Princess Wand", 100000040, "Sword_Wand", null, ClientItemType.Craft, [ItemGroup.Sword, ItemGroup.Craft]),
        new Item("Pot Lid", 100000041, "Shield_PotLid", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Shield]),
        new Item("Art Palette", 100000042, "Shield_Palette", null, ClientItemType.Craft, [ItemGroup.Craft, ItemGroup.Shield]),
        new Item("Inner Tube", 100000043, "Shield_Tube", null, ClientItemType.Craft, [ItemGroup.Craft, ItemGroup.Shield]),
        new Item("Platter", 100000044, "Shield_Platter", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Shield]),
        new Item("Skateboard", 100000045, "Shield_Skateboard", null, ClientItemType.Craft, [ItemGroup.Craft, ItemGroup.Shield]),
        new Item("Martin", 100000046, "Shield_Martin", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Shield]),
        new Item("Chessboard", 100000047, "Shield_Chessboard", null, ClientItemType.Craft, [ItemGroup.Craft, ItemGroup.Shield]),
        new Item("Big Leaf", 100000048, "Shield_Leaf", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Shield]),
        new Item("Trampoline", 100000049, "Shield_Stretch", null, ClientItemType.Item, [ItemGroup.Traversal, ItemGroup.CardboardDestroyer, ItemGroup.Item]),
        new Item("Tower Shield", 100000050, "Shield_TowerShield", null, ClientItemType.Craft, [ItemGroup.Craft, ItemGroup.Shield]),
        new Item("Trash Can Lid", 100000051, "Shield_TrashCanLid", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer, ItemGroup.Shield]),
        new Item("Blue Scooter Board", 100000052, "Shield_ScooterBoardBlue", null, ClientItemType.Craft, [ItemGroup.Craft, ItemGroup.Shield]),
        new Item("Ragdoll", 100000053, "Item_Ragdoll", null, ClientItemType.Craft, [ItemGroup.Craft]),
        new Item("Balloon", 100000054, "Item_Balloon", null, ClientItemType.Item, [ItemGroup.Traversal, ItemGroup.Item]),
        new Item("Skipping Rock", 100000055, "Rock", null, ClientItemType.Item, [ItemGroup.Ranged, ItemGroup.CardboardDestroyer, ItemGroup.Item]),
        new Item("Space Blaster", 100000056, "Item_SpaceGun", null, ClientItemType.Item, [ItemGroup.Ranged, ItemGroup.CardboardDestroyer, ItemGroup.Item]),
        new Item("Shuriken", 100000057, "Item_Shuriken", null, ClientItemType.Item, [ItemGroup.Ranged, ItemGroup.CardboardDestroyer, ItemGroup.Item]),
        new Item("Bowling Bomb", 100000058, "Item_Bomb", null, ClientItemType.Item, [ItemGroup.Item, ItemGroup.CardboardDestroyer]),
        new Item("Bubble Gum", 100000059, "Item_Gum", null, ClientItemType.Item, [ItemGroup.Traversal, ItemGroup.Item]),
        new Item("Sticky Hand", 100000060, "Item_StickyHand", null, ClientItemType.Item, [ItemGroup.Item]),
        new Item("Paint Blaster", 100000061, "Item_PaintGun", null, ClientItemType.Item, [ItemGroup.Ranged, ItemGroup.CardboardDestroyer, ItemGroup.Item]),
        new Item("An Actual Digital Camera", 100000062, "Item_Camera", null, ClientItemType.Craft, [ItemGroup.Craft]),
        new Item("Megaphone", 100000063, "Item_SearchNPCs", null, ClientItemType.Item, [ItemGroup.Item]),
        new Item("Texting With Jill", 100000064, "Item_SearchObjects", null, ClientItemType.Item, [ItemGroup.Item]),
        new Item("Oar", 100000065, "oar", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Sleep Mask", 100000066, "mask", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Giant Socks", 100000067, "giant_socks", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Tiger Form", 100000068, "tiger_form", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Guitar of Space", 100000069, "guitar", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Key", 100000070, "key", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Finish Flag", 100000071, "flag", null, ClientItemType.Item, [ItemGroup.Unlock]),
        new Item("Out of Logic Item", 999999999, "ool", null, ClientItemType.Item, []),
        ];
        
}