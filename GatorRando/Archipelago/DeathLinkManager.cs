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
            // Plugin.LogDebug(deathLink.Cause);
            string deathLinkMessage = $"I'm about to stumble because {deathLink.Source} died";
            if (deathLink.Cause != null && deathLink.Cause != "")
            {
                deathLinkMessage += $"due to {deathLink.Cause}";
            }
            BubbleManager.QueueBubble(deathLinkMessage, BubbleManager.BubbleType.Trap);
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