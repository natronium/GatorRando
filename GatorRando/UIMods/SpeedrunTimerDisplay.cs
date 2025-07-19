using System;
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
        try
        {
            return double.Parse(Util.FindIntKeyByPrefix(SpeedrunTimerPrefix));
        }
        catch (FormatException)
        {
            return 0;
        }
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
        if (SpeedrunData.isSpeedrunMode)
        {
            SpeedrunTimer.instance.gameObject.SetActive(!RandoSettingsMenu.HideSpeedrunTimer());
        }
    }
}