using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;

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
		    Player.movement.Ragdoll();
        }
	}

    internal static void DisableDeathLink()
    {
        deathLinkService.DisableDeathLink();
        deathLinkService.OnDeathLinkReceived -= OnDeathReceived;
        deathLinkEnabled = false;
    }
}