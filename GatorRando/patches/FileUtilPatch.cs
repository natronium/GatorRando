using System;
using GatorRando.Archipelago;
using GatorRando.UIMods;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(FileUtil))]
internal static class FileUtilPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Read))]
    private static void PreRead(ref bool forceLocal)
    {
        forceLocal = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.ReadSaveData))]
    private static void PreReadSaveData(ref Action<GameSaveData> onComplete)
    {
		static void readServer(GameSaveData _) {
            if (ConnectionManager.Authenticated)
            {
                SaveManager.ReadCurrentAPServerData();
            }
        }

        onComplete += readServer;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Write))]
    private static void PreWrite(ref bool forceLocal)
    {
        forceLocal = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.WriteSaveData))]
    private static void PreWriteSaveData(ref Action onComplete)
    {
        SpeedrunTimerDisplay.AddTimerToSave();
        onComplete += SaveManager.WriteCurrentAPServerData;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.CopyGameSaveData))]
	private static void PreCopySaveData(int sourceIndex, int targetIndex, ref Action onComplete)
    {
        onComplete += () => SaveManager.CopyAPServerData(sourceIndex, targetIndex);
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.EraseGameSaveData))]
	private static void PreEraseSaveData(ref Action onComplete)
    {
        onComplete += () => SaveManager.EraseCurrentAPServerData();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.ReadGameSaveDataInfo))]
	private static bool PreReadGameSaveDataInfo()
    {
        return ConnectionManager.Authenticated;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.DoesFileExistCoroutine))]
    private static void PreDoesFileExistCoroutine(ref bool forceLocal)
    {
        forceLocal = true;
    }
}