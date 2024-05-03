using System;
using BepInEx;
using HarmonyLib;

namespace GatorRando
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        static Plugin Instance;

        private void Awake()
        {
            Instance = this;
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll(); // automatically patch based on harmony attributes
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            Logger.LogDebug("THIS IS A DEBUG LOG! LOOK AT ME! @@@@@@@@@@@");
        }

        static void LogCall(String typeName, String methodName)
        {
            Instance.Logger.LogDebug($"{typeName}.{methodName}!");
            Instance.Logger.LogDebug(new System.Diagnostics.StackTrace().ToString());
        }

        [HarmonyPatch(typeof(ItemManager))]
        private static class ItemManagerPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("UnlockItem")]
            static void PreUnlockItem(string itemID)
            {
                LogCall("ItemManager", "UnlockItem");
            }

            // [HarmonyPrefix]
            // [HarmonyPatch("EquipItem")]
            // static void PreEquipItem()
            // {
            //     LogCall("ItemManager", "EquipItem");
            // }

            [HarmonyPrefix]
            [HarmonyPatch("GiveItem")]
            static void PreGiveItem(ItemObject item, bool equip)
            {
                LogCall("ItemManager", "GiveItem");
            }

            [HarmonyPrefix]
            [HarmonyPatch("SetUnlocked")]
            static void PreSetUnlocked(string itemName)
            {
                LogCall("ItemManager", "SetUnlocked");
            }
            //EquipItem (there are several??)
            //GiveItem
            //UnlockItem
            //SetUnlocked?

        }

        [HarmonyPatch(typeof(ItemObject))]
        private static class ItemObejctPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch("IsUnlocked", MethodType.Setter)]
            static void PreSetIsUnlocked(bool value)
            {
                LogCall("ItemObject", "set_IsUnlocked");
            }
    }

}
}
