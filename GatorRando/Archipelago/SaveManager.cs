using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GatorRando.Archipelago;

public static class SaveManager
{
    // Goals:
    // Create a save or set of saves per seed
    // Allow you to load an existing save for that seed or start a new one
    // Cache location scouts in save
    // Delete all AP saves function



    private static readonly string saveFolderPath = Path.Combine(Application.persistentDataPath, "AP_Saves");

    private static readonly string[] apServerDataPaths = [];

    private static string CurrentSavePath()
    {
        return Path.Combine(saveFolderPath, "AP_" + ConnectionManager.Seed());
    }

    public static void LoadAPSaveData()
    {
        FileUtil.saveFilePaths[0] = CurrentSavePath() + "_0";
        FileUtil.saveFilePaths[1] = CurrentSavePath() + "_1";
        FileUtil.saveFilePaths[2] = CurrentSavePath() + "_2";
        FileUtil.infoFilePaths[0] = CurrentSavePath() + "info_0";
        FileUtil.infoFilePaths[1] = CurrentSavePath() + "info_1";
        FileUtil.infoFilePaths[2] = CurrentSavePath() + "info_2";
        apServerDataPaths[0] = CurrentSavePath() + "_server0";
        apServerDataPaths[1] = CurrentSavePath() + "_server1";
        apServerDataPaths[2] = CurrentSavePath() + "_server2";
        FileUtil.ReadGameSaveDataInfo(0);
        FileUtil.ReadGameSaveDataInfo(1);
        FileUtil.ReadGameSaveDataInfo(2);
    }

    public static void SaveAPServerData()
    {
        using BinaryWriter binaryWriter = new(File.Open(apServerDataPaths[GameData.g.saveFileSlot], FileMode.Create));
        binaryWriter.Write(ConnectionManager.ServerData.ToString());
    }

    public static void LoadAPServerData()
    {
        string apServerData = "";
        using (BinaryReader binaryReader = new(File.Open(apServerDataPaths[GameData.g.saveFileSlot], FileMode.Open)))
        {
            apServerData = binaryReader.ReadString();
        }
        ConnectionManager.ServerData = JsonConvert.DeserializeObject<ArchipelagoData>(apServerData);
    }

    public static void EraseAllAPSaveData()
    {
        string[] apSaveFilePaths = Directory.GetFiles(saveFolderPath, "AP_*");
        foreach (string apSavePath in apSaveFilePaths)
        {
            File.Delete(apSavePath);
        }
    }
}