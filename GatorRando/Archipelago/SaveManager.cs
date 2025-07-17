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

    //TODO: Load last connection info on startup


    private static readonly string saveFolderPath = Path.Combine(Application.persistentDataPath, "AP_Saves");

    private static readonly string[] apServerDataPaths = new string[3];

    private static readonly string lastConnectionFile = Path.Combine(saveFolderPath, "lastConnection.txt");

    public static readonly string slotNameString = "Slot Name";
    public static readonly string serverString = "Server Address:Port";
    public static readonly string passwordString = "Password";

    private static string CurrentSavePath()
    {
        return Path.Combine(saveFolderPath, "AP_" + ConnectionManager.SlotName() + "_" + ConnectionManager.Seed());
    }

    public static void CreateSaveDirectory()
    {
        Directory.CreateDirectory(saveFolderPath);
    }

    public static void LoadAPSaveData()
    {
        FileUtil.saveFilePaths[0] = CurrentSavePath() + "_0";
        FileUtil.saveFilePaths[1] = CurrentSavePath() + "_1";
        FileUtil.saveFilePaths[2] = CurrentSavePath() + "_2";
        FileUtil.infoFilePaths[0] = CurrentSavePath() + "_info_0";
        FileUtil.infoFilePaths[1] = CurrentSavePath() + "_info_1";
        FileUtil.infoFilePaths[2] = CurrentSavePath() + "_info_2";
        apServerDataPaths[0] = CurrentSavePath() + "_server_0";
        apServerDataPaths[1] = CurrentSavePath() + "_server_1";
        apServerDataPaths[2] = CurrentSavePath() + "_server_2";
        FileUtil.ReadGameSaveDataInfo(0);
        FileUtil.ReadGameSaveDataInfo(1);
        FileUtil.ReadGameSaveDataInfo(2);
        SaveFileScreen saveFileScreen = Util.GetByPath("Main Menu/Main Menu Canvas/Load File Screen").GetComponent<SaveFileScreen>();
        saveFileScreen.UpdateState();
    }

    private static void WriteAPServerData(string path)
    {
        File.WriteAllText(path, ConnectionManager.ServerData.ToString());
    }
    
    private static void WriteLastConnectionData()
    {
        WriteAPServerData(lastConnectionFile);
    }
    

    public static void WriteCurrentAPServerData()
    {
        WriteAPServerData(apServerDataPaths[GameData.g.saveFileSlot]);
    }

    public static void ReadAPServerData(string path)
    {
        if (File.Exists(path))
        {
            ConnectionManager.ServerData = JsonConvert.DeserializeObject<ArchipelagoData>(File.ReadAllText(path));
        }
        else
        {
            ConnectionManager.ServerData = new();
        }
    }

    public static void ReadLastConnectionData()
    {
        ReadAPServerData(lastConnectionFile);
    }


    public static void ReadCurrentAPServerData()
    {
        ReadAPServerData(apServerDataPaths[GameData.g.saveFileSlot]);
    }

    public static void EraseCurrentAPServerData()
    {
        File.Delete(apServerDataPaths[GameData.g.saveFileSlot]);
    }

    public static void EraseAllAPSaveData()
    {
        string[] apSaveFilePaths = Directory.GetFiles(saveFolderPath, "AP_*");
        foreach (string apSavePath in apSaveFilePaths)
        {
            File.Delete(apSavePath);
        }
    }

    public static void UpdateLastConnectionData(string connectionField, string connectionString)
    {
        if (connectionField == slotNameString.ToLower())
        {
            ConnectionManager.ServerData.SlotName = connectionString;
        }
        else if (connectionField == serverString.ToLower())
        {
            ConnectionManager.ServerData.Uri = connectionString;
        }
        else if (connectionField == passwordString.ToLower())
        {
            ConnectionManager.ServerData.Password = connectionString;
        }
        else
        {
            Plugin.LogWarn("Unknown connection field written");
        }
        WriteLastConnectionData();

    }

    public static string DisplayLastConnectionData(string connectionField)
    {
        ReadLastConnectionData();
        if (connectionField == slotNameString.ToLower())
        {
            return ConnectionManager.ServerData.SlotName;
        }
        else if (connectionField == serverString.ToLower())
        {
            return ConnectionManager.ServerData.Uri;
        }
        else if (connectionField == passwordString.ToLower())
        {
            return ConnectionManager.ServerData.Password;
        }
        Plugin.LogWarn("Unknown connection field read");
        return "";
    }

    

    
}