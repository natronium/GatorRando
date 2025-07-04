using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace GatorRando.UIMods;

public static class Texture2DHandler
{
    private static Sprite[] builtinSprites;
    private static readonly List<Sprite> newSprites = [];

    private static readonly Dictionary<string, string> texturePaths = new()
    {
        {"Guitar","GatorRando.Sprites.guitar.png"},
        {"Key","GatorRando.Sprites.key.png"},
        {"Oar","GatorRando.Sprites.oar.png"},
        {"TabShort","GatorRando.Sprites.quest_item_tab_AP_down.png"},
        {"TabLong","GatorRando.Sprites.quest_item_tab_AP_up.png"},
        {"SleepMask","GatorRando.Sprites.sleep_mask.png"},
        {"Tiger","GatorRando.Sprites.tiger.png"},
        {"Archipelago","GatorRando.Sprites.archipelago_sticker_style.png"},
        {"Flag","GatorRando.Sprites.checkered_flag.png"},
        {"Socks","GatorRando.Sprites.giant_socks.png"},
    };


    static Texture2D LoadTextureForName(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        Texture2D texture2D = new(200, 200);
        using (var memstream = new MemoryStream())
        {
            assembly.GetManifestResourceStream(texturePaths[name]).CopyTo(memstream);
            texture2D.LoadImage(memstream.ToArray());
        }
        return texture2D;
    }
    
    //TODO Retrieve Sprites in a way that does not result in them becoming unloaded
    public static Sprite GetSpriteForItem(string name)
    {
        Plugin.LogDebug($"Looking for sprite for {name}");
        if (texturePaths.ContainsKey(name))
        {
            Plugin.LogDebug($"This sprite should be at {texturePaths[name]}");
            if (newSprites.Any(sprite => sprite.name == name))
            {
                return newSprites.First(sprite => sprite.name == name);
            }
            else
            {
                Texture2D texture2D = LoadTextureForName(name);
                Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, 200, 200), new Vector2(0.5f, 0.5f));
                sprite.name = name;
                newSprites.Add(sprite);
                return sprite;
            }

        }
        else
        {
            Plugin.LogDebug($"Loading sprite from game itself");
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
                return Util.FindItemObjectByName("Placeholder").sprite; //TODO: AP Item Sprite
            }
        }
    }
}