using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Converters;
using Archipelago.MultiClient.Net.Packets;
using GatorRando.PrefabMods;
using GatorRando.UIMods;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

namespace GatorRando.Archipelago;

public static class TrapManager
{
    internal static IEnumerator trapHandler;
    internal static ConcurrentQueue<TrapType> trapQueue;
    private static readonly float trapDelay = 3f;
    private static readonly float floatTime = 5f;
    private static readonly float sneakTime = 5f;
    private static float sneakTimer;

    private static readonly float pixelTime = 5f;
    private static float pixelTimer;

    public static Dictionary<string, TrapType> traps = new() {
        {"Stumble Trap", TrapType.Ragdoll },
        {"Dialogue Trap", TrapType.Dialogue },
        {"Float Trap", TrapType.Float },
        {"Sneak Trap", TrapType.Sneak },
        {"Pixel Trap", TrapType.Pixel },
    };

    public static Dictionary<string, TrapType> TrapNameToType = new()
    {
        // Lil Gator
        {"Stumble Trap", TrapType.Ragdoll },
        {"Dialogue Trap", TrapType.Dialogue },
        {"Float Trap", TrapType.Float },
        {"Sneak Trap", TrapType.Sneak },
        {"Pixel Trap", TrapType.Pixel },

        // Death's Door
        {"Rotation Trap", TrapType.Pixel },
        {"Player Invisibility Trap", TrapType.Sneak },
        {"Enemy Invisibility Trap", TrapType.Pixel },
        {"Knockback Trap", TrapType.Ragdoll },

        // List below borrowed from Tunic, commented out ones we don't yet have good equivalents for
        {"Ice Trap", TrapType.Sneak },
        {"Freeze Trap", TrapType.Sneak },
        {"Frozen Trap", TrapType.Sneak },
        {"Stun Trap", TrapType.Sneak },
        {"Paralyze Trap", TrapType.Sneak },
        {"Chaos Control Trap", TrapType.Sneak },

        // {"Fire Trap", TrapType.Fire },
        // {"Damage Trap", TrapType.Fire },
        // {"Bomb", TrapType.Fire },  // Luigi's Mansion, yes it's just Bomb
        {"Posession Trap", TrapType.Dialogue },  // Luigi's Mansion, damage-based trap
        // {"Nut Trap", TrapType.Fire },  // DKC, damage-based trap

        // {"Bee Trap", TrapType.Bee },

        // {"Tiny Trap", TrapType.PlayerInvisibility },
        // {"Poison Mushroom", TrapType.PlayerInvisibility },  // Luigi's Mansion, makes player smaller

        {"Screen Flip Trap", TrapType.Pixel },
        {"Mirror Trap", TrapType.Pixel },
        {"Reverse Trap", TrapType.Pixel },
        {"Reversal Trap", TrapType.Pixel },

        {"Deisometric Trap", TrapType.Pixel },
        {"Confuse Trap", TrapType.Pixel },
        {"Confusion Trap", TrapType.Pixel },
        {"Fuzzy Trap", TrapType.Pixel },
        {"Confound Trap", TrapType.Pixel },

        {"Bonk Trap", TrapType.Ragdoll },
        {"Banana Trap", TrapType.Ragdoll },
        {"Spring Trap", TrapType.Float },

        // {"Zoom Trap", TrapType.Zoom },  // Celeste, zooms camera in

        // {"Bald Trap", TrapType.Bald },  // Celeste, bald
        {"Whoops! Trap", TrapType.Float }, // Here Comes Niko, drops the player from way high up
        // {"W I D E Trap", TrapType.Wide }, // Here Comes Niko, makes the fox W I D E
        {"Home Trap", TrapType.Float }, // Here Comes Niko, teleports the player "home", here, Up reference
    };

    internal static void Setup()
    {
        trapQueue = new();
        trapHandler = TrapHandler();
        sneakTimer = 0f;
        pixelTimer = 0f;

        if (RandoSettingsMenu.IsTrapLinkOn())
        {
            EnableTrapLink();
            ConnectionManager.Session.Socket.PacketReceived += ReceiveTrapLink;
        }
        else
        {
            DisableTrapLink();
            ConnectionManager.Session.Socket.PacketReceived -= ReceiveTrapLink;
        }
    }

    public static void QueueTrap(string trapName)
    {
        if (traps.ContainsKey(trapName))
        {
            trapQueue.Enqueue(traps[trapName]);
        }
    }

    private static IEnumerator TrapHandler()
    {
        static bool InPlay()
        {
            return Game.State == GameState.Play;
        }

        while (ConnectionManager.Authenticated)
        {
            if (!InPlay() || DialogueModifier.inTrapDialogue)
            {
                yield return true;
                continue;
            }
            if (!trapQueue.TryPeek(out TrapType trap))
            {
                yield return true;
                continue;
            }

            switch (trap)
            {
                case TrapType.Ragdoll:
                    ActivateRagdollTrap(); break;
                case TrapType.Dialogue:
                    ActivateDialogueTrap(); break;
                case TrapType.Float:
                    ActivateFloatTrap(); break;
                case TrapType.Sneak:
                    ActivateSneakTrap(); break;
                case TrapType.Pixel:
                    ActivatePixelTrap(); break;
            }
            trapQueue.TryDequeue(out _);
            // Add delay between each trap
            float timePreDelay = Time.time;
            while (Time.time - timePreDelay < trapDelay)
            {
                yield return null;
            }
        }
    }

    private static void ActivateRagdollTrap()
    {
        BubbleManager.QueueBubble("Oops, I stumbled!", BubbleManager.BubbleType.Trap);
        if (!Player.movement.isRagdolling)
        {
            Player.movement.Ragdoll();
        }
    }

    private static void ActivateDialogueTrap()
    {
        BubbleManager.QueueBubble("Oh, no! I have to talk to people", BubbleManager.BubbleType.Trap);
        DialogueModifier.DialogueTrap();
    }

    private static void ActivateFloatTrap()
    {
        BubbleManager.QueueBubble("Oh dear, not again...", BubbleManager.BubbleType.Trap);
        bool startRoutine = BalloonMods.floatTimer <= 0f;
        BalloonMods.floatTimer += floatTime;
        if (startRoutine)
        {
            Player.actor.StartCoroutine(BalloonMods.Floating());
        }
    }

    private static void ActivateSneakTrap()
    {
        static IEnumerator Sneaking()
        {
            float startingMaxSpeed = Player.movement.maxSpeed;

            Player.movement.maxSpeed = 1f;
            // Lower timer
            while (sneakTimer > 0)
            {
                sneakTimer -= Time.deltaTime;
                yield return null;
            }

            // At this point, timer has ran out

            sneakTimer = 0f;

            Player.movement.maxSpeed = startingMaxSpeed;
        }

        BubbleManager.QueueBubble("The monsters are watching me, I better sneak so they don't notice!", BubbleManager.BubbleType.Trap);
        bool startRoutine = sneakTimer <= 0f;
        sneakTimer += sneakTime;
        if (startRoutine)
        {
            Player.actor.StartCoroutine(Sneaking());
        }
    }

    private static void ActivatePixelTrap()
    {
        static IEnumerator Pixelizing()
        {
            PixelPerfectCamera p = MainCamera.p;
            bool startingEnabled = p.enabled; // In case someone is playing on crunchy normally
            int refResolutionX = p.refResolutionX;
            int refResolutionY = p.refResolutionY;

            p.enabled = true;
            p.refResolutionX = refResolutionX / 6;
            p.refResolutionY = refResolutionY / 6;

            // Lower timer
            while (pixelTimer > 0)
            {
                pixelTimer -= Time.deltaTime;
                yield return null;
            }

            // At this point, timer has ran out

            pixelTimer = 0f;

            p.enabled = startingEnabled;
            p.refResolutionX = refResolutionX;
            p.refResolutionY = refResolutionY;
        }

        BubbleManager.QueueBubble("It's Crunch Time!", BubbleManager.BubbleType.Trap);
        bool startRoutine = pixelTimer <= 0f;
        pixelTimer += pixelTime;
        if (startRoutine)
        {
            Player.actor.StartCoroutine(Pixelizing());
        }
    }



    // TrapLink Implementation borrowed from Tunic (SilentDestroyer and ScipioWright)
    public static void SendTrapLink(string trapName)
    {
        BouncePacket bouncePacket = new BouncePacket
        {
            Tags = ["TrapLink"],
            Data = new Dictionary<string, JToken>
                {
                    { "time", (float)DateTime.Now.ToUnixTimeStamp() },
                    { "source", ConnectionManager.SlotName()},
                    { "trap_name", trapName}
                }
        };
        ConnectionManager.Session.Socket.SendPacketAsync(bouncePacket);
    }

    public static void ReceiveTrapLink(ArchipelagoPacketBase packet)
    {
        if (RandoSettingsMenu.IsTrapLinkOn() && packet is BouncedPacket bouncedPacket && bouncedPacket.Tags.Contains("TrapLink"))
        {
            // we don't want to receive own trap links, since the other slot will have already received a trap on its own
            // note: if two people are connected to the same slot, both players will likely send their own trap links
            // idk if we can actually fix this? (Note from Silent from Tunic's implementation)
            if (bouncedPacket.Data["source"].ToString() == ConnectionManager.SlotName())
            {
                return;
            }
            string trapName = bouncedPacket.Data["trap_name"].ToString();
            string source = bouncedPacket.Data["source"].ToString();
            if (TrapNameToType.ContainsKey(trapName))
            {
                Plugin.LogInfo($"Received TrapLink {trapName} from {source}");
                trapQueue.Enqueue(TrapNameToType[trapName]);
            }
        }
    }

    public static void EnableTrapLink()
    {
        if (!ConnectionManager.Session.ConnectionInfo.Tags.Contains("TrapLink"))
        {
            ConnectionManager.Session.ConnectionInfo.UpdateConnectionOptions([.. ConnectionManager.Session.ConnectionInfo.Tags, .. new string[1] { "TrapLink" }]);
        }
    }

    public static void DisableTrapLink()
    {
        if (ConnectionManager.Session.ConnectionInfo.Tags.Contains("TrapLink"))
        {
            ConnectionManager.Session.ConnectionInfo.UpdateConnectionOptions([.. ConnectionManager.Session.ConnectionInfo.Tags.Where(tag => tag != "TrapLink")]);
        }
    }



    public enum TrapType
    {
        Ragdoll,
        Dialogue,
        Float,
        Sneak,
        Pixel
    }
}