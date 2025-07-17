using GatorRando.Archipelago;
using UnityEngine;

namespace GatorRando;

public static class MapManager
{
    public readonly struct PlayerCoords(float x, float y)
    {
        public float X { get; } = x;
        public float Y { get; } = y;
    }

    private static PlayerCoords lastSentCoords;
    private static float lastSentTime;

    private const float timeDelta = 5.0f;
    private const float distanceDelta = 10.0f;

    public static void UpdateCoordsIfNeeded()
    {
        var currentTime = Time.time;
        var currentCoords = CurrentCoords();
        if (currentTime >= lastSentTime + timeDelta) {
            UpdateCoordinates();
        } else if (Vector2.Distance(new(lastSentCoords.X, lastSentCoords.Y), new(currentCoords.X, currentCoords.Y)) > distanceDelta){
            UpdateCoordinates();
        }
    }

    private static PlayerCoords CurrentCoords() {
        GameObject lilGator = Util.GetByPath("Players/Player/Heroboy");
        float westEast = lilGator.transform.position.x;
        float northSouth = lilGator.transform.position.z; // y is height
        return new(westEast, northSouth);
    }

    private static void UpdateCoordinates() {
        var currentTime = Time.time;
        var currentCoords = CurrentCoords();
        ConnectionManager.StorePosition(currentCoords);
        lastSentCoords = currentCoords;
        lastSentTime = currentTime;
    }
}