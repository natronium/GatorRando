using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace GatorRando.Archipelago;

public static class SaveManager
{
    private static readonly string saveFolderPath = Path.Combine(Application.persistentDataPath, "AP_Saves");

    private static readonly string[] apServerDataPaths = new string[3];

    private static readonly string lastConnectionFile = Path.Combine(saveFolderPath, "lastConnection.txt");

    public static readonly string slotNameString = "Slot Name";
    public static readonly string serverString = "Server Address:Port";
    public static readonly string passwordString = "Password";
    public static readonly string apItemIndexKey = "LastAPItemIndex";

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

    public static void CopyAPServerData(int sourceIndex, int targetIndex)
    {
        File.Copy(apServerDataPaths[sourceIndex], apServerDataPaths[targetIndex], true);
    }

    public static void ReadAPServerData(string path, bool lastConnection = false)
    {
        if (File.Exists(path))
        {
            ConnectionManager.ServerData = JsonConvert.DeserializeObject<ArchipelagoData>(File.ReadAllText(path));
        }
        else if (lastConnection)
        {
            ConnectionManager.ServerData = new();
        }
    }

    public static void ReadLastConnectionData()
    {
        ReadAPServerData(lastConnectionFile, true);
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

    public static void LoadPostPrologueSaveData(int index)
    {
        string postPrologueSave = "";
        var assembly = Assembly.GetExecutingAssembly();
        using (var reader = new StreamReader(assembly.GetManifestResourceStream("GatorRando.Data.postPrologueSave.json")))
        {
            postPrologueSave = reader.ReadToEnd();
        }
        File.WriteAllText(FileUtil.saveFilePaths[index], postPrologueSave);
        GameSaveData gameSaveData = FileUtil.ReadSaveData(index);
        gameSaveData.playerName = ConnectionManager.SlotName();
        FileUtil.WriteSaveData(gameSaveData, index);
        FileUtil.UpdateSaveDataInfo(gameSaveData, index);
    }

    public static void ForceSave()
    {
        GameData.g.WriteToDisk(false);
    }

    public static bool CheckIfSaveAheadOfServer(int index)
    {
        if (File.Exists(apServerDataPaths[index]))
        {
            ArchipelagoData tempServerData = JsonConvert.DeserializeObject<ArchipelagoData>(File.ReadAllText(apServerDataPaths[index]));
            if (tempServerData.Index > ConnectionManager.ItemsReceived().Count)
            {
                return true; // Saved server data is ahead of the items received count
            }
        }
        if (File.Exists(FileUtil.saveFilePaths[index]))
        {
            GameSaveData tempGameSaveData = FileUtil.ReadSaveData(index);
            if (tempGameSaveData.ints.TryGetValue(apItemIndexKey, out int savedItemIndex))
            {
                if (savedItemIndex > ConnectionManager.ItemsReceived().Count)
                {
                    return true; // Save file data is ahead of the item received count
                }
            }
        }
        return false; // Saves are not ahead of server
    }
}