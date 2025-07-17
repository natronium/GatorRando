using System.IO;
using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(FileUtil))]
static class FileUtilPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Read))]
    static bool PreRead(string path, bool forceLocal, ref string __result)
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
    static bool PreWrite(string path, bool forceLocal, string contents)
    {
        if (forceLocal)
        {
            return true;
        }
        File.WriteAllText(path, contents);
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.WriteSaveData))]
    static void PostWriteSaveData()
    {
        SaveManager.WriteCurrentAPServerData();
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.ReadSaveData))]
    static void PostReadSaveData()
    {
        if (ConnectionManager.Authenticated)
        {
            SaveManager.ReadCurrentAPServerData();
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.EraseGameSaveData))]
    static void PostEraseSaveData()
    {
        SaveManager.EraseCurrentAPServerData();
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.ReadGameSaveDataInfo))]
    static bool PreReadGameSaveDataInfo()
    {
        return ConnectionManager.Authenticated;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.DoesFileExist))]
    static void PreDoesFileExist(ref bool forceLocal)
    {
        forceLocal = true;
    }
}