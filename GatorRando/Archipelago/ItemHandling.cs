using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Archipelago.MultiClient.Net.Models;
using GatorRando.Data;
using GatorRando.UIMods;

namespace GatorRando.Archipelago;


public static class ItemHandling
{
    private readonly struct QueuedItem(Items.Item item, string sendingPlayerName)
    {
        public readonly Items.Item item = item;
        public readonly string sendingPlayerName = sendingPlayerName;
    }
    private static ConcurrentQueue<QueuedItem> ItemQueue = new();
    private static readonly Dictionary<string, Action> SpecialItemFunctions = [];
    public static void RegisterItemListener(string itemName, Action listener) => SpecialItemFunctions[itemName] = listener;
    public static void TriggerItemListeners()
    {
        foreach (ItemInfo itemInfo in ConnectionManager.ItemsReceived())
        {
            Items.Item item = GetItemEntryByApId(itemInfo.ItemId);
            if (item.clientNameId is not null && SpecialItemFunctions.ContainsKey(item.clientNameId))
            {
                SpecialItemFunctions[item.clientNameId]();
            }
        }
    }


    private static Items.Item GetItemEntryByApId(long id) => Items.itemData.First(entry => entry.apItemId == id);
    public static string GetClientIDByAPId(long id) => GetItemEntryByApId(id).clientNameId;
    private static long GetItemApIdFromGatorName(string gatorName) =>
        Items.itemData.First(entry => entry.clientNameId == gatorName).apItemId;
    private static long GetItemApIdFromAPName(string apName) =>
        Items.itemData.First(entry => entry.name == apName).apItemId;

    public static bool IsItemUnlocked(string itemName, bool isAPName = false)
    {
        if (isAPName)
        {
            return ConnectionManager.ItemsReceived().Select(info => info.ItemId).Contains(GetItemApIdFromAPName(itemName));
        }
        else
        {
            if (itemName == "Broken Scooter Board")
            {
                itemName = "Shield_ScooterBoardGreen";
            }
            return ConnectionManager.ItemsReceived().Select(info => info.ItemId).Contains(GetItemApIdFromGatorName(itemName));
        }
    }

    public static int GetItemUnlockCount(string itemName, bool isAPName = false)
    {
        if (isAPName)
        {
            return ConnectionManager.ItemsReceived().Where(itemInfo => itemInfo.ItemId == GetItemApIdFromAPName(itemName)).Count();
        }
        else
        {
            return ConnectionManager.ItemsReceived().Where(itemInfo => itemInfo.ItemId == GetItemApIdFromGatorName(itemName)).Count();
        }
    }

    public static void ProcessItemQueue()
    {
        while (ItemQueue.TryDequeue(out QueuedItem queuedItem))
        {
            ReceiveItem(queuedItem.item, queuedItem.sendingPlayerName);
            LocationAccessibilty.UpdateAccessibleLocations();
            var lastIndex = GameData.g.ReadInt("LastAPItemIndex", 0);
            GameData.g.Write("LastAPItemIndex", lastIndex + 1);
        }
    }

    public static void ClearItemQueue()
    {
        while (ItemQueue.TryDequeue(out QueuedItem queuedItem))
        {
            
        }
    }

    public static void OnDisconnect()
    {
        ItemQueue = new();
    }

    public static void EnqueueItem(long id, string playerName)
    {
        ItemQueue.Enqueue(new QueuedItem(GetItemEntryByApId(id), playerName));
    }

    private static void ReceiveItem(Items.Item item, string playerName)
    {
        Plugin.LogDebug($"ReceiveItem for {item.name}. ClientId:{item.clientNameId}, Type:{item.clientItemType}, AP:{item.apItemId}");
        switch (item.clientItemType)
        {
            case Items.ClientItemType.Item: ItemUtil.GiveItem(item.clientNameId); break;
            case Items.ClientItemType.Craft: ItemUtil.GiveCraft(item.clientNameId); break;
            case Items.ClientItemType.Friend: ItemUtil.GiveFriends((int)item.clientResourceAmount); break;
            case Items.ClientItemType.CraftStuff: ItemUtil.GiveCraftStuff((int)item.clientResourceAmount); break;
            default:
                throw new Exception($"Item {item.clientNameId} has an unknown client_item type of {item.clientItemType}");
        }
        ;

        if (item.clientNameId is not null && SpecialItemFunctions.ContainsKey(item.clientNameId))
        {
            SpecialItemFunctions[item.clientNameId]();
        }
        if (playerName != ConnectionManager.SlotName())
        {
            BubbleManager.QueueBubble($"{playerName} sent me {item.name}!", BubbleManager.BubbleType.ItemReceived);
        }
    }
}