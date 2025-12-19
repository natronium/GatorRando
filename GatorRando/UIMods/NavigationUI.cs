using GatorRando.Archipelago;
using GatorRando.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GatorRando.UIMods;

internal static class NavigationUI
{
    private static GameObject mapDisplay;
    private static Text gridCoordText;

    private readonly struct MapCoords(float x, float y)
    {
        public float X { get; } = x;
        public float Y { get; } = y;
    }

    private static MapCoords lastMapCoords;
    private static float compassRotation;
    private static GameObject arrow;
    private static GameObject map;
    private static Dictionary<long, List<List<int>>> locationCoords;
    private static List<LocationSquare> locationSquares = [];
    private static float mapBottomY = 0;
    private static float mapLeftX = 0;
    private static readonly float MAP_PIXEL_SIZE = 2048;

    private readonly struct LocationSquare(List<int> coord, GameObject location, List<long> apIds)
    {
        internal readonly List<int> Coord = coord;
        internal readonly GameObject Location = location;
        internal readonly List<long> ApIds = apIds;
    }

    internal static void Setup()
    {
        GameObject canvas = new()
        {
            name = "Navigation Panel Canvas"
        };
        Canvas canvasCanvas = canvas.AddComponent<Canvas>();
        CanvasScaler canvasScaler = canvas.AddComponent<CanvasScaler>();
        canvasCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        mapDisplay = new()
        {
            name = "Navigation Panel"
        };
        mapDisplay.transform.parent = canvas.transform;
        Image panelBackground = mapDisplay.AddComponent<Image>();
        panelBackground.sprite = SpriteHandler.GetSpriteForItem("Wood Frame");
        panelBackground.sprite.texture.filterMode = FilterMode.Point;
        panelBackground.type = Image.Type.Sliced;
        RectTransform mapDisplayRect = mapDisplay.GetComponent<RectTransform>();
        mapDisplayRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 10, 110);
        mapDisplayRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10, 110);

        GameObject mapContainer = new()
        {
            name = "Map Container"
        };
        mapContainer.transform.parent = mapDisplay.transform;
        mapContainer.AddComponent<RectMask2D>();
        RectTransform mapMaskRect = mapContainer.GetComponent<RectTransform>();
        mapMaskRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 5, 100);
        mapMaskRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 5, 100);

        map = new()
        {
            name = "Minimap"
        };
        map.transform.parent = mapContainer.transform;
        RawImage mapImage = map.AddComponent<RawImage>();
        mapImage.maskable = true;
        mapImage.texture = SpriteHandler.GetSpriteForItem("Map").texture;
        mapImage.texture.filterMode = FilterMode.Point;
        map.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 2048);
        map.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 2048);

        mapDisplay.SetActive(RandoSettingsMenu.IsNavigationOn());

        GameObject poster = new()
        {
            name = "Grid Poster"
        };
        poster.transform.parent = mapDisplay.transform;
        Image posterImage = poster.AddComponent<Image>();
        posterImage.sprite = SpriteHandler.GetSpriteForItem("Poster");
        posterImage.sprite.texture.filterMode = FilterMode.Point;
        poster.transform.localPosition = new(-40, -55, 0);
        posterImage.rectTransform.sizeDelta = new(32, 32);

        GameObject tutorialTextObj = Util.GetByPath("Canvas/Tutorials/Climb Tutorial/Tutorial Image/Contents/Description");

        GameObject gridCoord = Object.Instantiate(tutorialTextObj, poster.transform);
        gridCoord.name = "Grid Coordinate Text";
        gridCoord.transform.localPosition = new(1, 0, 0);
        gridCoordText = gridCoord.GetComponent<Text>();
        gridCoordText.text = "Test";
        gridCoordText.rectTransform.sizeDelta = new(22, 22);
        Outline gridTextOutline = gridCoord.AddComponent<Outline>();
        gridTextOutline.effectColor = Color.white;

        InitializeLocationPositions();

        // Create the arrow after the location squares so that it is on top
        arrow = new()
        {
            name = "Compass Arrow"
        };
        arrow.transform.parent = map.transform;
        Image arrowImage = arrow.AddComponent<Image>();
        arrowImage.sprite = SpriteHandler.GetSpriteForItem("Sword_Wood");
        arrowImage.rectTransform.sizeDelta = new(15, 15);
    }

    internal static void CleanUp()
    {
        mapDisplay = null;
        gridCoordText = null;
        arrow = null;
        map = null;
        locationSquares = [];
    }

    internal static void UpdateNavigation()
    {
        if (RandoSettingsMenu.IsNavigationOn() != mapDisplay.activeSelf)
        {
            mapDisplay.SetActive(RandoSettingsMenu.IsNavigationOn());
        }
        UpdateMapCoordinates();
        UpdateMapPosition();
        UpdateCompassDirection();
        arrow.transform.eulerAngles = new(0, 0, compassRotation - 45);
        UpdateGridCoordinates();
    }

    private static void UpdateMapCoordinates()
    {
        float XPosToMapY(float xpos)
        {
            return (xpos + 165) / 480;
        }
        float YPosToMapX(float ypos)
        {
            return (ypos + 165) / 480;
        }

        MapManager.PlayerCoords playerCoords = MapManager.CurrentCoords();
        lastMapCoords = new MapCoords(XPosToMapY(playerCoords.X), YPosToMapX(playerCoords.Y));
    }

    private static void UpdateMapPosition()
    {
        float offset = -0.5f;

        // 1/2 - 1/20 = half of the actual map image size - half of the minimap display frame size
        float maxExtent = MAP_PIXEL_SIZE * (0.5f - 0.05f);

        mapLeftX = -1 * Mathf.Clamp((lastMapCoords.X + offset) * MAP_PIXEL_SIZE, -maxExtent, maxExtent);
        mapBottomY = -1 * Mathf.Clamp((lastMapCoords.Y + offset) * MAP_PIXEL_SIZE, -maxExtent, maxExtent);
        map.transform.localPosition = new(mapLeftX, mapBottomY, 0);
        arrow.transform.localPosition = new((lastMapCoords.X + offset) * MAP_PIXEL_SIZE, (lastMapCoords.Y + offset) * MAP_PIXEL_SIZE, 0);
    }

    private static void UpdateGridCoordinates()
    {
        string[] rowLetters = ["A", "B", "C", "D", "E", "F", "G", "H", "I", "J"];
        int column = (int)Mathf.Clamp(lastMapCoords.X * 10, 0, 9);
        int row = (int)Mathf.Clamp((1 - lastMapCoords.Y) * 10, 0, 9);
        string gridCoordinate = rowLetters[row] + column.ToString();
        gridCoordText.text = gridCoordinate;
    }

    private static void UpdateCompassDirection()
    {
        GameObject lilGator = Util.GetByPath("Players/Player/Heroboy");
        Vector3 direction = lilGator.transform.forward;
        Vector3 north = Vector3.forward;
        compassRotation = 360 - Vector3.SignedAngle(north, direction, Vector3.up);
    }

    private static void InitializeLocationPositions()
    {
        locationCoords = Rules.GatorRules.LocationCoords;
        IEnumerable<List<int>> newKeys = locationCoords.Values.SelectMany(v => v).Distinct(new CoordComparator());
        Dictionary<List<int>, List<long>> tempDict = new(new CoordComparator());

        foreach (List<int> nk in newKeys)
        {
            if (!tempDict.ContainsKey(nk))
            {
                tempDict[nk] = [];
            }
            IEnumerable<long> vals = locationCoords.Keys.Where(k => locationCoords[k].Contains(nk, new CoordComparator()));
            tempDict[nk].InsertRange(0, vals);
        }
        foreach (KeyValuePair<List<int>, List<long>> keyValuePair in tempDict)
        {
            List<int> coord = keyValuePair.Key;
            List<long> apIds = keyValuePair.Value;
            GameObject locSquare = MakeLocationSquare(coord);
            locationSquares.Add(new(coord, locSquare, apIds));
        }
        LocationAccessibilty.UpdateAccessibleLocations();
    }

    private static GameObject MakeLocationSquare(List<int> coord)
    {
        GameObject location = new()
        {
            name = $"X: {coord[0]}, Y: {coord[1]}"
        };
        location.transform.parent = map.transform;
        // RectTransform locRect = location.AddComponent<RectTransform>();

        GameObject redSquare = new()
        {
            name = "Out of Logic"
        };
        redSquare.transform.parent = location.transform;
        // redSquare.transform.localPosition = new(0,0,0);
        RawImage red = redSquare.AddComponent<RawImage>();
        red.texture = SpriteHandler.GetSpriteForItem("Red Square").texture;
        red.texture.filterMode = FilterMode.Point;
        red.maskable = true;
        red.rectTransform.sizeDelta = new(16, 16);

        GameObject greenSquare = new()
        {
            name = "In Logic"
        };
        greenSquare.transform.parent = location.transform;
        greenSquare.SetActive(false);
        // greenSquare.transform.localPosition = new(0,0,0);
        RawImage green = greenSquare.AddComponent<RawImage>();
        green.texture = SpriteHandler.GetSpriteForItem("Green Square").texture;
        green.texture.filterMode = FilterMode.Point;
        green.maskable = true;
        green.rectTransform.sizeDelta = new(16, 16);

        location.transform.localPosition = new(coord[0] - MAP_PIXEL_SIZE / 2, MAP_PIXEL_SIZE / 2 - coord[1], 0);

        return location;
    }

    internal static void UpdateLocationSquareState()
    {
        foreach (LocationSquare locationSquare in locationSquares)
        {
            if (locationSquare.Location == null)
            {
                continue;
            }
            bool accessible = false;
            bool collected = true;
            foreach (long apId in locationSquare.ApIds)
            {
                if (!LocationHandling.IsApLocationCollected(apId))
                {
                    collected = false;
                    if (LocationAccessibilty.IsApLocationIdAccessible(apId))
                    {
                        accessible = true;
                    }
                }
            }
            if (collected)
            {
                // Turn off both squares
                locationSquare.Location.transform.GetChild(1).gameObject.SetActive(false);
                locationSquare.Location.transform.GetChild(0).gameObject.SetActive(false);
            }
            else if (accessible)
            {
                // Turn on Green
                locationSquare.Location.transform.GetChild(1).gameObject.SetActive(true);
                // Turn off Red
                locationSquare.Location.transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                // Remaining checks are not accessible
                // Turn off Green
                locationSquare.Location.transform.GetChild(1).gameObject.SetActive(false);
                // Turn on Red
                locationSquare.Location.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}

internal class CoordComparator : EqualityComparer<List<int>>
{
    public override bool Equals(List<int> coord1, List<int> coord2)
    {
        if (coord1 == null && coord2 == null)
        {
            return false;
        }
        else if (coord1 == null || coord2 == null)
        {
            return false;
        }
        else if (coord1.Count != 2 || coord2.Count != 2)
        {
            return false;
        }
        return coord1[0] == coord2[0] && coord1[1] == coord2[1];
    }

    public override int GetHashCode(List<int> coord)
    {
        return $"X: {coord[0]}, Y: {coord[1]}".GetHashCode();
    }
}