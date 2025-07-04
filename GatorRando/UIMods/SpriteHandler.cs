using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace GatorRando.UIMods;

public static class SpriteHandler
{
    private static Sprite[] builtinSprites;
    private static readonly List<Sprite> newSprites = [];
    private static readonly Dictionary<string, string> existingSpritePaths = new()
    {
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_core_bracelet_blue.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_core_bracelet_magenta.png"},
        {"Craft Stuff", "Assets/UI/Images/Item Sprites/Itemsprite_core_crafting.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_core_purple.png"},
        {"Shirt", "Assets/UI/Images/Item Sprites/Itemsprite_core_shirt.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_core_yellow.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_basic.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_beret.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_bucket.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_cowboy.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_detective.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_frills.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_ninja.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_princess.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_skate.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_slime.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_spacehelmet.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_hat_vampire.png"},
        {"POT?", "Assets/UI/Images/Item Sprites/Itemsprite_quest_pot.png"},
        {"QuestItem_Retainer", "Assets/UI/Images/Item Sprites/Itemsprite_quest_retainer.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_bigleaf.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_chessboard.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_innertube.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_martin.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_palette.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_platter.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_potlid.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_scooterboardblue.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_scooterboardgreen.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_skateboard.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_tower.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_trampoline.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_shield_trashcanlid.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_balloon.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_bowlingbomb.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_camera.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_gum.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_megaphone.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_paintblaster.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_paperstars.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_ragdoll.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_rock.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_spaceblaster.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_stickyhand.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_special_textjill.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_cavemanhammer.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_grabber.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_lasersword.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_net.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_nunchucks.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_paintbrush.png"},
        {"Thrown_Pencil", "Assets/UI/Images/Item Sprites/Itemsprite_sword_pencil.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_princesswand.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_spear.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_stick.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_wooden.png"},
        // {"ohai", "Assets/UI/Images/Item Sprites/Itemsprite_sword_wrench.png"},

        // LOOKING FOR GatorMewhenyouaremyfriend
        // Itemsprite_quest_icecream
        // Itemsprite_quest_clippings
        // Itemsprite_quest_bucketfull
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
    };

    public readonly struct AddedTexture(string path, int width, int height)
    {
        public readonly string path = path;
        public readonly int width = width;
        public readonly int height = height;
    }


    static Texture2D LoadTextureForName(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        Texture2D texture2D = new(200, 200);
        using (var memstream = new MemoryStream())
        {
            assembly.GetManifestResourceStream(newSpriteInformation[name].path).CopyTo(memstream);
            texture2D.LoadImage(memstream.ToArray());
        }
        return texture2D;
    }
    
    //TODO Retrieve Sprites in a way that does not result in them becoming unloaded
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
            // if (existingSpritePaths.ContainsKey(name))
            // {
            //     return Addressables.LoadAssetAsync<Sprite>(existingSpritePaths[name]);
            // } 

            // Plugin.LogDebug($"Loading sprite from game itself");
            ItemObject itemObject = Util.FindItemObjectByName(name);
            if (itemObject != null)
            {
                return itemObject.sprite;
            }
            else
            {
                builtinSprites ??= Resources.FindObjectsOfTypeAll<Sprite>();
                if (name.Contains("Craft Stuff"))
                {
                    return builtinSprites.First(sprite => sprite.name == "Itemsprite_core_crafting");
                }
                else if (name.Contains("Friend"))
                {
                    return builtinSprites.First(sprite => sprite.name == "GatorMewhenyouaremyfriend");
                }
                Plugin.LogWarn("No sprite found, using placeholder!");
                return Util.FindItemObjectByName("Placeholder").sprite; // Should not appear
            }
        }
    }
}