using UnityEngine;

namespace GatorRando.UIMods;

public static class SpeedrunTimerDisplay
{
    private static readonly string SpeedrunTimerPrefix = "Speedrun Time: ";

    public static void AddTimerToSave()
    {
        Util.RemoveIntKeysByPrefix(SpeedrunTimerPrefix);
        GameData.g.Write(SpeedrunTimerPrefix + SpeedrunData.inGameTime.ToString(), 1);
    }

    private static double ReadTimerFromSave()
    {
        return double.Parse(Util.FindIntKeyByPrefix(SpeedrunTimerPrefix));
    }

    public static void OverwriteSpeedrunTimerWithSavedTime()
    {
        SpeedrunTimer.DestroySpeedrunTimer();
        SpeedrunData.inGameTime = ReadTimerFromSave();
        SpeedrunData.state = RunState.Started;
        SpeedrunTimer.CreateSpeedrunTimer();
        SpeedrunTimer.instance.UpdateIconColor(RunState.Started);
    }

    public static void ShowOrHideTimer()
    {
        GameObject speedrunTimer = SpeedrunTimer.instance.timerDisplay.gameObject;
        GameObject speedrunIcon = SpeedrunTimer.instance.icon.gameObject;
        speedrunTimer.SetActive(!RandoSettingsMenu.HideSpeedrunTimer());
        speedrunIcon.SetActive(!RandoSettingsMenu.HideSpeedrunTimer());
    }
}