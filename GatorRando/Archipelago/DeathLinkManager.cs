using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using GatorRando.UIMods;

namespace GatorRando.Archipelago;

internal static class DeathLinkManager
{
    private static DeathLinkService deathLinkService;
    private static bool deathLinkEnabled = false;

    internal static void EnableDeathLink()
    {
        deathLinkService = ConnectionManager.Session.CreateDeathLinkService();
        deathLinkService.EnableDeathLink();
        deathLinkEnabled = true;
        deathLinkService.OnDeathLinkReceived += OnDeathReceived;
    }

    private static void OnDeathReceived(DeathLink deathLink)
	{
        if (deathLinkEnabled)
        {
            BubbleManager.QueueBubble($"I'm about to stumble because {deathLink.Cause}", BubbleManager.BubbleType.Trap);
            // Queue a stumble trap (instead of immediate ragdoll)
		    TrapManager.QueueTrap("Stumble Trap");
        }
	}

    internal static void DisableDeathLink()
    {
        deathLinkService.DisableDeathLink();
        deathLinkService.OnDeathLinkReceived -= OnDeathReceived;
        deathLinkEnabled = false;
    }
}