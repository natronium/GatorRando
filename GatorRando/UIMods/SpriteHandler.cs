using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using System;

namespace GatorRando.UIMods;

public static class SpriteHandler
{
    private static Sprite[] builtinSprites;
    private static readonly List<Sprite> newSprites = [];
    private static readonly Dictionary<string, Sprite> loadedAssetSprites = [];
    private static readonly Dictionary<string, Sprite> duplicatedSprites = [];
    private static readonly Dictionary<string, string> existingSpritePaths = new()
    {
        {"Bracelet", "Assets/UI/Images/Item Sprites/Itemsprite_core_bracelet_blue.png"},
        {"Bracelet Magenta", "Assets/UI/Images/Item Sprites/Itemsprite_core_bracelet_magenta.png"},
        {"Craft Stuff", "Assets/UI/Images/Item Sprites/Itemsprite_core_crafting.png"},
        {"Bracelet Purple", "Assets/UI/Images/Item Sprites/Itemsprite_core_purple.png"},
        {"Shirt", "Assets/UI/Images/Item Sprites/Itemsprite_core_shirt.png"},
        {"Bracelet Yellow", "Assets/UI/Images/Item Sprites/Itemsprite_core_yellow.png"},
        {"Hat_Basic", "Assets/UI/Images/Item Sprites/Itemsprite_hat_basic.png"},
        {"Hat_Beret", "Assets/UI/Images/Item Sprites/Itemsprite_hat_beret.png"},
        {"Hat_Bucket", "Assets/UI/Images/Item Sprites/Itemsprite_hat_bucket.png"},
        {"Hat_Western", "Assets/UI/Images/Item Sprites/Itemsprite_hat_cowboy.png"},
        {"Hat_DetectiveCowl", "Assets/UI/Images/Item Sprites/Itemsprite_hat_detective.png"},
        {"Hat_Frills", "Assets/UI/Images/Item Sprites/Itemsprite_hat_frills.png"},
        {"Hat_Ninja", "Assets/UI/Images/Item Sprites/Itemsprite_hat_ninja.png"},
        {"Hat_Princess", "Assets/UI/Images/Item Sprites/Itemsprite_hat_princess.png"},
        {"Hat_SkateHelmet", "Assets/UI/Images/Item Sprites/Itemsprite_hat_skate.png"},
        {"Hat_Slime", "Assets/UI/Images/Item Sprites/Itemsprite_hat_slime.png"},
        {"Hat_Space", "Assets/UI/Images/Item Sprites/Itemsprite_hat_spacehelmet.png"},
        {"Hat_Vampire", "Assets/UI/Images/Item Sprites/Itemsprite_hat_vampire.png"},
        {"POT?", "Assets/UI/Images/Item Sprites/Itemsprite_quest_pot.png"},
        {"QuestItem_Retainer", "Assets/UI/Images/Item Sprites/Itemsprite_quest_retainer.png"},
        {"Shield_Leaf", "Assets/UI/Images/Item Sprites/Itemsprite_shield_bigleaf.png"},
        {"Shield_Chessboard", "Assets/UI/Images/Item Sprites/Itemsprite_shield_chessboard.png"},
        {"Shield_Tube", "Assets/UI/Images/Item Sprites/Itemsprite_shield_innertube.png"},
        {"Shield_Martin", "Assets/UI/Images/Item Sprites/Itemsprite_shield_martin.png"},
        {"Shield_Palette", "Assets/UI/Images/Item Sprites/Itemsprite_shield_palette.png"},
        {"Shield_Platter", "Assets/UI/Images/Item Sprites/Itemsprite_shield_platter.png"},
        {"Shield_PotLid", "Assets/UI/Images/Item Sprites/Itemsprite_shield_potlid.png"},
        {"Shield_ScooterBoardBlue", "Assets/UI/Images/Item Sprites/Itemsprite_shield_scooterboardblue.png"},
        {"Shield_ScooterBoardGreen", "Assets/UI/Images/Item Sprites/Itemsprite_shield_scooterboardgreen.png"},
        {"Shield_Skateboard", "Assets/UI/Images/Item Sprites/Itemsprite_shield_skateboard.png"},
        {"Shield_TowerShield", "Assets/UI/Images/Item Sprites/Itemsprite_shield_tower.png"},
        {"Shield_Stretch", "Assets/UI/Images/Item Sprites/Itemsprite_shield_trampoline.png"},
        {"Shield_TrashCanLid", "Assets/UI/Images/Item Sprites/Itemsprite_shield_trashcanlid.png"},
        {"Item_Balloon", "Assets/UI/Images/Item Sprites/Itemsprite_special_balloon.png"},
        {"Item_Bomb", "Assets/UI/Images/Item Sprites/Itemsprite_special_bowlingbomb.png"},
        {"Item_Camera", "Assets/UI/Images/Item Sprites/Itemsprite_special_camera.png"},
        {"Item_Gum", "Assets/UI/Images/Item Sprites/Itemsprite_special_gum.png"},
        {"Item_SearchNPCs", "Assets/UI/Images/Item Sprites/Itemsprite_special_megaphone.png"},
        {"Item_PaintGun", "Assets/UI/Images/Item Sprites/Itemsprite_special_paintblaster.png"},
        {"Item_Shuriken", "Assets/UI/Images/Item Sprites/Itemsprite_special_paperstars.png"},
        {"Item_Ragdoll", "Assets/UI/Images/Item Sprites/Itemsprite_special_ragdoll.png"},
        {"Rock", "Assets/UI/Images/Item Sprites/Itemsprite_special_rock.png"},
        {"Item_SpaceGun", "Assets/UI/Images/Item Sprites/Itemsprite_special_spaceblaster.png"},
        {"Item_StickyHand", "Assets/UI/Images/Item Sprites/Itemsprite_special_stickyhand.png"},
        {"Item_SearchObjects", "Assets/UI/Images/Item Sprites/Itemsprite_special_textjill.png"},
        {"Sword_RockHammer", "Assets/UI/Images/Item Sprites/Itemsprite_sword_cavemanhammer.png"},
        {"Sword_Grabby", "Assets/UI/Images/Item Sprites/Itemsprite_sword_grabber.png"},
        {"Sword_Laser", "Assets/UI/Images/Item Sprites/Itemsprite_sword_lasersword.png"},
        {"Sword_Net", "Assets/UI/Images/Item Sprites/Itemsprite_sword_net.png"},
        {"Sword_Nunchucks", "Assets/UI/Images/Item Sprites/Itemsprite_sword_nunchucks.png"},
        {"Sword_Paintbrush", "Assets/UI/Images/Item Sprites/Itemsprite_sword_paintbrush.png"},
        {"Sword_Pencil", "Assets/UI/Images/Item Sprites/Itemsprite_sword_pencil.png"},
        {"Sword_Wand", "Assets/UI/Images/Item Sprites/Itemsprite_sword_princesswand.png"},
        {"Sword_CBSpear", "Assets/UI/Images/Item Sprites/Itemsprite_sword_spear.png"},
        {"Sword_Stick", "Assets/UI/Images/Item Sprites/Itemsprite_sword_stick.png"},
        {"Sword_Wood", "Assets/UI/Images/Item Sprites/Itemsprite_sword_wooden.png"},
        {"Sword_Wrench", "Assets/UI/Images/Item Sprites/Itemsprite_sword_wrench.png"},
    };
    private static readonly Dictionary<string, AddedTexture> newSpriteInformation = new()
    {
        {"Guitar", new AddedTexture("GatorRando.Sprites.guitar.png", 200, 200)},
        {"Key", new AddedTexture("GatorRando.Sprites.key.png", 200, 200)},
        {"Oar", new AddedTexture("GatorRando.Sprites.oar.png", 200, 200)},
        {"QuestActiveTab", new AddedTexture("GatorRando.Sprites.quest_item_tab_AP_down.png", 103, 134)},
        {"QuestInactiveTab", new AddedTexture("GatorRando.Sprites.quest_item_tab_AP_up.png", 103, 203)},
        {"SleepMask", new AddedTexture("GatorRando.Sprites.sleep_mask.png", 200, 200)},
        {"Tiger", new AddedTexture("GatorRando.Sprites.tiger.png", 200, 200)},
        {"Archipelago", new AddedTexture("GatorRando.Sprites.archipelago_sticker_style.png", 200, 200)},
        {"Flag", new AddedTexture("GatorRando.Sprites.checkered_flag.png", 200, 200)},
        {"Socks", new AddedTexture("GatorRando.Sprites.giant_socks.png", 200, 200)},
        {"Map", new AddedTexture("GatorRando.Sprites.map_3.png", 2048, 2048)},
        {"Red Square", new AddedTexture("GatorRando.Sprites.redsquare.png", 16, 16)},
        {"Green Square", new AddedTexture("GatorRando.Sprites.greensquare.png", 16, 16)},
        {"Wood Frame", new AddedTexture("GatorRando.Sprites.woodframe.png", 96, 96)},
        {"Poster", new AddedTexture("GatorRando.Sprites.poster.png", 32, 32)},
    };

    private static readonly Dictionary<string, string> spritesToDuplicateInformation = new()
    {
        {"Friend", "GatorMewhenyouaremyfriend"},
        {"ICE CREAM", "Itemsprite_quest_icecream"},
        {"CLIPPINGS", "Itemsprite_quest_clippings"},
        {"WATER", "Itemsprite_quest_bucketfull"},
        {"BEACH ROCK", "Itemsprite_quest_cavemanrock"},
        {"HALF A CHEESE SANDWICH", "Itemsprite_quest_halfacheesesandwich"},
    };

    public readonly struct AddedTexture(string path, int width, int height)
    {
        public readonly string path = path;
        public readonly int width = width;
        public readonly int height = height;
    }


    static Texture2D LoadTextureForName(string name)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Texture2D texture2D = new(200, 200);
        using (MemoryStream memstream = new MemoryStream())
        {
            assembly.GetManifestResourceStream(newSpriteInformation[name].path).CopyTo(memstream);
            texture2D.LoadImage(memstream.ToArray());
        }
        return texture2D;
    }

    public static void LoadSprites()
    {
        LoadAssetSprites();
        DuplicateNonAssetLoadSprites();
    }

    private static void LoadAssetSprites()
    {
        if (loadedAssetSprites.Count == 0)
        {
            foreach (string spriteName in existingSpritePaths.Keys)
            {
                loadedAssetSprites[spriteName] = Addressables.LoadAssetAsync<Sprite>(existingSpritePaths[spriteName]).WaitForCompletion();
            }
        }
    }

    private static void DuplicateNonAssetLoadSprites()
    {
        if (duplicatedSprites.Count == 0)
        {
            builtinSprites ??= Resources.FindObjectsOfTypeAll<Sprite>();
            foreach (string itemName in spritesToDuplicateInformation.Keys)
            {
                Sprite existingSprite;
                try
                {
                    existingSprite = builtinSprites.First(sprite => sprite.name == spritesToDuplicateInformation[itemName]);
                }
                catch (InvalidOperationException)
                {
                    Plugin.LogWarn("No sprite found, storing placeholder!");
                    existingSprite = Util.FindItemObjectByName("Placeholder").sprite;
                }

                duplicatedSprites[itemName] = DuplicateSprite(existingSprite.texture);
            }
        }
    }

    private static Sprite DuplicateSprite(Texture2D originalTexture)
    {
        Texture2D copyTexture = new(originalTexture.width, originalTexture.height, textureFormat: originalTexture.format, mipCount: originalTexture.mipmapCount, linear: false);
        Graphics.CopyTexture(originalTexture, copyTexture);
        return Sprite.Create(copyTexture, new Rect(0, 0, originalTexture.width, originalTexture.height), new Vector2(0.5f, 0.5f));
    }

    public static Sprite GetSpriteForItem(string name)
    {
        // Plugin.LogDebug($"Looking for sprite for {name}");
        if (newSpriteInformation.ContainsKey(name))
        {
            // Plugin.LogDebug($"This sprite should be at {texturePaths[name]}");
            if (newSprites.Any(sprite => sprite.name == name))
            {
                return newSprites.First(sprite => sprite.name == name);
            }
            else
            {
                Texture2D texture2D = LoadTextureForName(name);
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, newSpriteInformation[name].width, newSpriteInformation[name].height), new Vector2(0.5f, 0.5f));
                sprite.name = name;
                newSprites.Add(sprite);
                return sprite;
            }

        }
        else
        {
            if (name.Contains("Craft Stuff"))
            {
                name = "Craft Stuff";
            }
            else if (name.Contains("Friend"))
            {
                name = "Friend";
            }
            else if (name.Contains("Thrown_Pencil"))
            {
                name = "Sword_Pencil";
            }

            if (spritesToDuplicateInformation.ContainsKey(name))
            {
                return duplicatedSprites[name];
            }
            else if (existingSpritePaths.ContainsKey(name))
            {
                return loadedAssetSprites[name];
            }
            else
            {
                Plugin.LogWarn("No sprite found, using placeholder!");
                return Util.FindItemObjectByName("Placeholder").sprite; // Should not appear
            }
        }
    }
}
