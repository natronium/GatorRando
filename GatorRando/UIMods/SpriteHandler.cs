using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using GatorRando.Archipelago;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GatorRando.UIMods;

public static class SpriteHandler
{
    private static UnityAction<Scene, LoadSceneMode> onSceneLoaded;
    private const string mainMenuScene = "151139d96e5e2984c9d8feea8124d010";
    private static readonly AssetReference mainMenuAsset = new AssetReference(mainMenuScene);
    private const string islandScene = "7560b99e56838c44ea0585ca5de4d984";
    private static readonly AssetReference islandAsset = new AssetReference(islandScene);
    private const string undergroundScene = "1a52a7ac4a966f240abd7feb9c779591";
    private static readonly AssetReference undergroundAsset = new AssetReference(undergroundScene);

    private static readonly string cachedSpritesPath = Path.Combine(Application.persistentDataPath, "AP_Sprites");
    private enum SceneName
    {
        Island,
        Underground,
        MainMenu
    }
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
        {"Dash_Flashstep", "Assets/UI/Images/Item Sprites/UG/Itemsprite_dash_flashstep.png"},
        {"Dash_Soldier", "Assets/UI/Images/Item Sprites/UG/Itemsprite_dash_soldier.png"},
        {"FireworkJetpack", "Assets/UI/Images/Item Sprites/UG/Itemsprite_firework.png"},
        {"Glowing Gunk", "Assets/UI/Images/Item Sprites/UG/Itemsprite_glowinggunk.png"},
        {"Hat_Bone", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_bone.png"},
        {"Hat_Ghost", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_ghost.png"},
        {"Hat_Mining", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_mining.png"},
        {"Hat_Pirate", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_pirate.png"},
        {"Hat_Propeller", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_propeller.png"},
        {"Hat_Scarf", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_scarf.png"},
        {"Hat_SuperSpikes", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_superspikes.png"},
        {"Hat_Viking", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_viking.png"},
        {"Hat_Wizard", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hat_wizard.png"},
        {"Hover_Propeller", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hover_propeller.png"},
        {"Hover_Quarterstaff", "Assets/UI/Images/Item Sprites/UG/Itemsprite_hover_quarterstaff.png"},
        {"BattleTop", "Assets/UI/Images/Item Sprites/UG/Itemsprite_item_battletop.png"},
        {"RC_Drone", "Assets/UI/Images/Item Sprites/UG/Itemsprite_item_drone.png"},
        {"Rubber Ball", "Assets/UI/Images/Item Sprites/UG/Itemsprite_rubberball.png"},
        {"Shield_Ram", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_batteringram.png"},
        {"Shield_Dark", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_dark.png"},
        {"Shield_Film", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_film.png"},
        {"Shield_Minecart", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_minecart.png"},
        {"Shield_Painting", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_painting.png"},
        {"Shield_Record", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_record.png"},
        {"Shield_Sign", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_sign.png"},
        {"Shield_Surfboard", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_surfboard.png"},
        {"Shield_Web", "Assets/UI/Images/Item Sprites/UG/Itemsprite_shield_web.png"},
        {"Slam_Club", "Assets/UI/Images/Item Sprites/UG/Itemsprite_slam_club.png"},
        {"Slam_Pickaxe", "Assets/UI/Images/Item Sprites/UG/Itemsprite_slam_pickaxe.png"},
        {"SpecialStamina_Name", "Assets/UI/Images/Item Sprites/UG/Itemsprite_special.png"},
        {"SpiderWeb", "Assets/UI/Images/Item Sprites/UG/Itemsprite_spiderweb.png"},
        {"Spin_Bubble", "Assets/UI/Images/Item Sprites/UG/Itemsprite_spin_bubble.png"},
        {"Spin_Ribbon", "Assets/UI/Images/Item Sprites/UG/Itemsprite_spin_ribbon.png"},
        {"Sword_Dark", "Assets/UI/Images/Item Sprites/UG/Itemsprite_sword_dark.png"},
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
        {"UG Map", new AddedTexture("GatorRando.Sprites.ug_map_3.png", 2048, 2048)},
        {"Red Square", new AddedTexture("GatorRando.Sprites.redsquare.png", 16, 16)},
        {"Green Square", new AddedTexture("GatorRando.Sprites.greensquare.png", 16, 16)},
        {"Wood Frame", new AddedTexture("GatorRando.Sprites.woodframe.png", 96, 96)},
        {"Poster", new AddedTexture("GatorRando.Sprites.poster.png", 32, 32)},
    };

    private static readonly Dictionary<string, string> spritesFromIsland = new()
    {
        {"ICE CREAM", "Itemsprite_quest_icecream"},
        {"CLIPPINGS", "Itemsprite_quest_clippings"},
        {"WATER", "Itemsprite_quest_bucketfull"},
        {"BEACH ROCK", "Itemsprite_quest_cavemanrock"},
        {"HALF A CHEESE SANDWICH", "Itemsprite_quest_halfacheesesandwich"},
    };

    private static readonly Dictionary<string, string> spritesFromUnderground = new()
    {
        {"First Queen Letter", "Itemsprite_note"},
        {"Second Queen Letter", "Itemsprite_notetwo"},
        {"Thorny","Cryptids_Sketch_CB_0"},
        {"Bubbly","Cryptids_Sketch_CB_1"},
        {"Cakey","Cryptids_Sketch_CB_2"},
        {"Holy","Cryptids_Sketch_CB_3"},
        {"Finny","Cryptids_Sketch_CB_4"},
        {"Drippy","Cryptids_Sketch_CB_5"},
        {"Floofy","Cryptids_Sketch_CB_6"},
        {"Treey","Cryptids_Sketch_CB_7"},
        {"Looky","Cryptids_Sketch_CB_8"},
        {"Clam","Itemsprite_clam"},
    };
    private static readonly List<string> cryptids = [
        "Thorny", "Bubbly", "Cakey", "Holy", "Finny", "Drippy", "Floofy", "Treey", "Looky"
    ];


    private static readonly Dictionary<string, string> spritesFromTitleScreen = new()
    {
        {"Friend", "Counter_friend_icon"},
    };

    public readonly struct AddedTexture(string path, int width, int height)
    {
        public readonly string path = path;
        public readonly int width = width;
        public readonly int height = height;
    }

    private static void DuplicateSpritesFromScene(SceneName sceneName)
    {
        Dictionary<string, string> spriteNameDict = sceneName switch
        {
            SceneName.Island => spritesFromIsland,
            SceneName.Underground => spritesFromUnderground,
            SceneName.MainMenu => spritesFromTitleScreen,
            _ => throw new NotImplementedException("Tried to duplicate sprites from unknown scene"),
        };
        Sprite[] builtinSprites = Resources.FindObjectsOfTypeAll<Sprite>();
        foreach (string itemName in spriteNameDict.Keys)
        {
            if (!duplicatedSprites.ContainsKey(itemName))
            {
                Sprite existingSprite;
                try
                {
                    existingSprite = builtinSprites.First(sprite => sprite.name == spriteNameDict[itemName]);
                }
                catch (InvalidOperationException)
                {
                    Plugin.LogWarn($"No sprite found for {itemName}, storing placeholder!");
                    existingSprite = Util.FindItemObjectByName("Placeholder").sprite;
                }
                // duplicatedSprites[itemName] = DuplicateSprite(existingSprite.texture);
                Plugin.Instance.StartCoroutine(SaveCachedSprite(itemName, existingSprite));
            }
        }
    }


    private static Texture2D LoadTextureFromModForName(string name)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        Texture2D texture2D = new(200, 200);
        using (MemoryStream memstream = new MemoryStream())
        {
            assembly.GetManifestResourceStream(newSpriteInformation[name].path).CopyTo(memstream);
            texture2D.LoadImage(memstream.ToArray());
        }
        texture2D.Apply(false,false);
        return texture2D;
    }

    public static void LoadSprites()
    {
        if (loadedAssetSprites.Count == 0)
        {
            LoadAssetSprites();
        }
        if (duplicatedSprites.Count == 0)
        {
            Directory.CreateDirectory(cachedSpritesPath); // in case cached folder doesn't exit yet
            LoadCachedSprites();

            DuplicateSpritesFromScene(SceneName.MainMenu);

            bool loadIsland = false;
            bool loadUnderground = false;

            foreach (string key in spritesFromIsland.Keys)
            {
                if (!duplicatedSprites.ContainsKey(key))
                {
                    loadIsland = true;
                    break;
                }
            }
            foreach (string key in spritesFromUnderground.Keys)
            {
                if (!duplicatedSprites.ContainsKey(key))
                {
                    loadUnderground = true;
                    break;
                }
            }
            if (loadIsland || loadUnderground)
            {
                onSceneLoaded = (scene, _) => LoadSpritesOnSceneChange(scene, loadUnderground);
                SceneManager.sceneLoaded += onSceneLoaded;
            }

            if (loadIsland)
            {
                // Start with loading island
                LoadSceneSequence.LoadScene(islandAsset);
            }
            else if (loadUnderground)
            {
                // If Island is already cached, load underground only
                LoadSceneSequence.LoadScene(undergroundAsset);
            }
            else
            {
                Plugin.Instance.firstLoad = false; // Everything is cached, move on
                StateManager.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
            }
        }
    }

    private static void LoadSpritesOnSceneChange(Scene scene, bool loadUnderground)
    {
        static IEnumerator waitToLoadIsland(bool loadUnderground)
        {
            // yield return null;
            yield return new WaitForSeconds(Plugin.LoadDelay);
            DuplicateSpritesFromScene(SceneName.Island);
            // yield return new WaitForSeconds(Plugin.LoadDelay);
            if (loadUnderground)
            {
                LoadSceneSequence.LoadScene(undergroundAsset);
            }
            else
            {

                LoadSceneSequence.LoadScene(mainMenuAsset);
            }
        }

        static IEnumerator waitToLoadUnderground()
        {
            // yield return null;
            yield return new WaitForSeconds(Plugin.LoadDelay);
            DuplicateSpritesFromScene(SceneName.Underground);
            // yield return new WaitForSeconds(Plugin.LoadDelay);
            LoadSceneSequence.LoadScene(mainMenuAsset);
        }
        if (Plugin.Instance.firstLoad)
        {
            if (scene.name == "Island")
            {
                UIMenus.u.SetGameplayState(false, true);
                Plugin.Instance.StartCoroutine(waitToLoadIsland(loadUnderground));
            }
            else if (scene.name == "Underground")
            {
                UIMenus.u.SetGameplayState(false, true);
                Plugin.Instance.StartCoroutine(waitToLoadUnderground());
            }
            else if (scene.name == "Prologue")
            {
                LoadCachedSprites();
                SceneManager.sceneLoaded -= onSceneLoaded;
                Plugin.Instance.firstLoad = false;
                StateManager.StartUp();
            }
        }
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

    private static void LoadCachedSprites()
    {
        foreach (string filePath in Directory.EnumerateFiles(cachedSpritesPath, "*.gatorcache"))
        {
            string constructedFilename = Path.GetFileNameWithoutExtension(filePath);
            string[] filenameComponents = constructedFilename.Split("+");
            int width = int.Parse(filenameComponents[0]);
            int height = int.Parse(filenameComponents[1]);
            string itemKey = filenameComponents[2];

			byte[] cachefileBytes = File.ReadAllBytes(filePath);

            //Not actually sure what mipchain does here, but it's mandatory, so...
            Texture2D texture2D = new(width, height, TextureFormat.ARGB32, mipChain: true);
            texture2D.LoadRawTextureData(cachefileBytes);
            // "you must call Apply after LoadRawTextureData to upload the changed pixels to the GPU."
            texture2D.Apply(false, false); 
            duplicatedSprites[itemKey] = Sprite.Create(texture2D, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f));
        }
    }
    private static IEnumerator SaveCachedSprite(string itemKey, Sprite sprite)
    {
        int spriteWidth = (int)sprite.rect.width;
        int spriteHeight = (int)sprite.rect.height;

        string constructedFilename = $"{spriteWidth}+{spriteHeight}+{itemKey}";

        RenderTexture renderTexture = RenderTexture.GetTemporary(spriteWidth, spriteHeight, 0, RenderTextureFormat.ARGB32);

        GameObject renderParentGO = new GameObject()
        {
            name = "Sprite Renderer"
        };

        GameObject canvasGO = new GameObject()
        {
            name = "Sprite Renderer Canvas"
        };
        canvasGO.transform.parent = renderParentGO.transform;

        GameObject cameraGO = new GameObject()
        {
            name = "Sprite Renderer Camera"
        };
        cameraGO.transform.parent = renderParentGO.transform;

        Camera camera = cameraGO.AddComponent<Camera>();
        camera.farClipPlane = 0.6f; // Don't render any distant geometry
        camera.backgroundColor = Color.clear;
        camera.clearFlags = CameraClearFlags.SolidColor; // Clear with Transparent
        camera.targetTexture = renderTexture;

        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.worldCamera = camera;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.planeDistance = 0.5f;

        // Canvas and Image on the same GO emperically determined to render the image "Fullscreen"
        //  uncertain how intended or consistent this behavior is
        Image uiImage = canvasGO.AddComponent<Image>();
        uiImage.sprite = sprite;
        uiImage.sprite.texture.filterMode = FilterMode.Point;
        uiImage.type = Image.Type.Simple;
        uiImage.useSpriteMesh = true;

        yield return new WaitForEndOfFrame(); // Must wait for frame to render before calling ReadPixels

        Texture2D copiedTexture = new(spriteWidth, spriteHeight);
        
        //ReadPixels reads *from RenderTexture.active*
        RenderTexture originalActiveRT = RenderTexture.active;
        RenderTexture.active = renderTexture;
        copiedTexture.ReadPixels(new Rect(0,0,spriteWidth, spriteHeight), 0, 0);
        copiedTexture.Apply(false, false);
        RenderTexture.active = originalActiveRT;

        using FileStream cacheFileStream = File.OpenWrite(Path.Combine(cachedSpritesPath, constructedFilename + ".gatorcache"));
        cacheFileStream.Write(copiedTexture.GetRawTextureData());

        RenderTexture.ReleaseTemporary(renderTexture);
        GameObject.Destroy(renderParentGO);
    }

    public static Sprite GetSpriteForItem(string name)
    {
        if (name.Contains("Friend")) //Adjust once we get the recolor
        {
            name = "Friend";
        }
        if (name.Contains("Trap"))
        {
            name = "Archipelago";
        }
        // Plugin.LogDebug($"Looking for sprite for {name}");
        if (newSpriteInformation.ContainsKey(name))
        {
            if (newSprites.Any(sprite => sprite.name == name))
            {
                return newSprites.First(sprite => sprite.name == name);
            }
            else
            {
                Texture2D texture2D = LoadTextureFromModForName(name);
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
            else if (name.Contains("Thrown_Pencil"))
            {
                name = "Sword_Pencil";
            }

            if (spritesFromIsland.ContainsKey(name) || spritesFromUnderground.ContainsKey(name) || spritesFromTitleScreen.ContainsKey(name))
            {
                return duplicatedSprites[name];
            }
            else if (existingSpritePaths.ContainsKey(name))
            {
                return loadedAssetSprites[name];
            }
            else
            {
                Plugin.LogWarn($"No sprite found, using placeholder for {name}!");
                return Util.FindItemObjectByName("Placeholder").sprite; // Should not appear
            }
        }
    }
}
