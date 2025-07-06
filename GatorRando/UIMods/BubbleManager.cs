using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace GatorRando.UIMods;

public static class BubbleManager
{
    private static readonly Queue<string> itemReceivedBubbleQueue = new();
    private static readonly Queue<string> locationCheckedBubbleQueue = new();
    private static readonly Queue<string> alertBubbleQueue = new();
    private static readonly Queue<string> unimportantBubbleQueue = new();
    private static DateTime lastBubbleTime = DateTime.UtcNow;
    private static TimeSpan timeBetweenBubbles = TimeSpan.FromSeconds(3);
    private static readonly int maxItemMessages = 5;

    public enum BubbleType
    {
        ItemReceived,
        LocationChecked,
        Alert,
        Unimportant
    }

    private static Queue<string> GetBubbleQueue(BubbleType bubbleType) => bubbleType switch
    {
        BubbleType.ItemReceived => itemReceivedBubbleQueue,
        BubbleType.LocationChecked => locationCheckedBubbleQueue,
        BubbleType.Alert => alertBubbleQueue,
        BubbleType.Unimportant => unimportantBubbleQueue,
        _ => throw new Exception("Somehow we have an invalid BubbleType"),
    };

    public static void QueueBubble(string dialogueString, BubbleType bubbleType) => GetBubbleQueue(bubbleType).Enqueue(dialogueString);

    private static void DequeueBubble(BubbleType bubbleType)
    {
        Queue<string> queue = GetBubbleQueue(bubbleType);

        string currentBubble = queue.Dequeue();
        while (queue.Count > 0 && queue.Peek() == currentBubble)
        {
            queue.Dequeue();
        }
        DialogueModifier.GatorBubble(currentBubble);
    }

    private static bool DequeuedImportantBubble()
    {
        if (alertBubbleQueue.Count > 0)
        {
            // Plugin.LogDebug("Sent alert");
            DequeueBubble(BubbleType.Alert);
            return true;
        }
        else if (itemReceivedBubbleQueue.Count > 0)
        {
            if (itemReceivedBubbleQueue.Count > maxItemMessages)
            {
                // If there's too many items received messages in the queue, dump them and display a special message
                itemReceivedBubbleQueue.Clear();
                DialogueModifier.GatorBubble("My friends sent me so much cool stuff!");
                return true;
            }
            else
            {
                DequeueBubble(BubbleType.ItemReceived);
            }
        }
        else if (locationCheckedBubbleQueue.Count > 0)
        {
            DequeueBubble(BubbleType.LocationChecked);
            return true;
        }
        return false;
    }

    public static void Update()
    {
        if (DateTime.UtcNow - lastBubbleTime > timeBetweenBubbles)
        {
            if (DequeuedImportantBubble())
            {
                lastBubbleTime = DateTime.UtcNow;
                // If there are important bubbles, clear out the unimportant ones
                unimportantBubbleQueue.Clear();
            }
            else
            {
                if (unimportantBubbleQueue.Count > 0)
                {
                    DequeueBubble(BubbleType.Unimportant);
                    lastBubbleTime = DateTime.UtcNow;
                }
            }
        }
    }
}