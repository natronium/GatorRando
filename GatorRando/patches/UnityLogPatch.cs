using HarmonyLib;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(Logger))]
internal static class UnityLogPatch
{
    private static readonly List<string> suppressKeywords = [
        "Nature/Soft",
        "Tree prefab",
        "Look rotation viewing vector",
        "DontDestroyOnLoad only works for root GameObjects",
        "Kinematic body only supports",
    ];
    private static bool MessageContainsAnyKeyword(string message, List<string> keywords)
    {
        foreach (string keyword in keywords)
        {
            if (message.Contains(keyword))
            {
                return true;
            }
        }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Logger.Log), [typeof(LogType),typeof(object)])]
    private static bool PreLog(object message)
    {
        Plugin.LogInfo("logging" + message.ToString());
        if (MessageContainsAnyKeyword(message.ToString(), suppressKeywords))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Logger.Log), [typeof(object)])]
    private static bool PreLog2(object message)
    {
        Plugin.LogInfo("logging2" + message.ToString());
        if (MessageContainsAnyKeyword(message.ToString(), suppressKeywords))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Logger.LogWarning), [typeof(string),typeof(object)])]
    private static bool PreLogWarning(object message)
    {
        Plugin.LogInfo("logging warning 2 argument"  + message.ToString());
        if (MessageContainsAnyKeyword(message.ToString(), suppressKeywords))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Logger.LogWarning), [typeof(string),typeof(object),typeof(Object)])]
    private static bool PreLogWarning2(object message)
    {
        Plugin.LogInfo("logging warning 3 argument"  + message.ToString());
        if (MessageContainsAnyKeyword(message.ToString(), suppressKeywords))
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    [HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(LogType), typeof(object)])]
private static bool PreLogOverride1(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride1");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(LogType), typeof(object), typeof(Object)])]
private static bool PreLogOverride2(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride2");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(LogType), typeof(string), typeof(object)])]
private static bool PreLogOverride3(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride3");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(LogType), typeof(string), typeof(object), typeof(Object)])]
private static bool PreLogOverride4(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride4");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(object)])]
private static bool PreLogOverride5(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride5");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(string), typeof(object)])]
private static bool PreLogOverride6(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride6");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.Log), [typeof(string), typeof(object), typeof(Object)])]
private static bool PreLogOverride7(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride7");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.LogWarning), [typeof(string), typeof(object)])]
private static bool PreLogOverride8(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride8");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.LogWarning), [typeof(string), typeof(object), typeof(Object)])]
private static bool PreLogOverride9(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride9");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.LogError), [typeof(string), typeof(object)])]
private static bool PreLogOverride10(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride10");
    return true;
}
[HarmonyPrefix]
[HarmonyPatch(nameof(Logger.LogError), [typeof(string), typeof(object), typeof(Object)])]
private static bool PreLogOverride11(object message)
{
    Plugin.LogInfo("Logging" + message.ToString() + "from LogOverride11");
    return true;
}
// [HarmonyPrefix]
// [HarmonyPatch(nameof(Logger.LogException), [typeof(Exception)])]
// private static bool PreLogOverride12(Exception exception)
// {
//     Plugin.LogInfo("Logging" + exception.ToString() + "from LogOverride12");
//     return true;
// }
// [HarmonyPrefix]
// [HarmonyPatch(nameof(Logger.LogException), [typeof(Exception), typeof(Object)])]
// private static bool PreLogOverride13(Exception exception)
// {
//     Plugin.LogInfo("Logging" + exception.ToString() + "from LogOverride13");
//     return true;
// }

    // [HarmonyPrefix]
    // [HarmonyPatch(nameof(Logger.LogWarning), [typeof(object),typeof(Object)])]
    // private static bool PreLogWarning2(object message)
    // {
    //     Plugin.LogInfo("logging warning 2 argument" + message.ToString());
    //     if (MessageContainsAnyKeyword(message.ToString(), suppressKeywords))
    //     {
    //         return false;
    //     }
    //     else
    //     {
    //         return true;
    //     }
    // }
}