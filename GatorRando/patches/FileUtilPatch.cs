using System.IO;
using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(FileUtil))]
static class FileUtilPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Read))]
    static void PreRead(ref bool forceLocal)
    {
        forceLocal = true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(FileUtil.Write))]
    static void PreWrite(ref bool forceLocal)
    {
        forceLocal = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(FileUtil.WriteSaveData))]
    static void PostWriteSaveData()
    {
        SaveManager.SaveAPServerData();        
    }
}