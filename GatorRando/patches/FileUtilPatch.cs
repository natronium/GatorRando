using System.IO;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(FileUtil))]
internal static class FileUtilPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Read))]
	private static bool PreRead(string path, bool forceLocal, ref string __result)
    {
        if (forceLocal)
        {
            return true;
        }
        __result = File.ReadAllText(path);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Write))]
	private static bool PreWrite(string path, bool forceLocal, string contents)
    {
        if (forceLocal)
        {
            return true;
        }
        File.WriteAllText(path, contents);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.WriteSaveData))]
	private static void PreWriteSaveData()
    {
        SpeedrunTimerDisplay.AddTimerToSave();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.WriteSaveData))]
	private static void PostWriteSaveData()
    {
        if (StateManager.GetCurrentState() != StateManager.State.NewGameSkipPrologue)
        {
            SaveManager.WriteCurrentAPServerData();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.ReadSaveData))]
	private static void PostReadSaveData()
    {
        if (ConnectionManager.Authenticated && StateManager.GetCurrentState() != StateManager.State.NewGameSkipPrologue)
        {
            SaveManager.ReadCurrentAPServerData();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.CopyGameSaveData))]
	private static void PostEraseSaveData(int sourceIndex, int targetIndex)
    {
        SaveManager.CopyAPServerData(sourceIndex, targetIndex);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.EraseGameSaveData))]
	private static void PostEraseSaveData()
    {
        SaveManager.EraseCurrentAPServerData();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.ReadGameSaveDataInfo))]
	private static bool PreReadGameSaveDataInfo()
    {
        return ConnectionManager.Authenticated;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.DoesFileExist))]
	private static void PreDoesFileExist(ref bool forceLocal)
    {
        forceLocal = true;
    }
}