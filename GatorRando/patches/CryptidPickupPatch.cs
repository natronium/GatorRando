using GatorRando.Archipelago;
using HarmonyLib;

namespace GatorRando.Patches;

[HarmonyPatch(typeof(CryptidPickup))]
internal static class CryptidPickupPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(CryptidPickup.Interact))]
	private static bool PreInteract(CryptidPickup __instance)
    {
        __instance.gameObject.SetActive(false);
        __instance.dsItem.itemName = __instance.name;
       __instance.dsItem.Run();
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CryptidPickup.PersistentState), MethodType.Getter)]
    private static bool PreGetPersistentState(CryptidPickup __instance, ref bool __result)
    {
        __result = LocationHandling.IsLocationCollected(__instance.name);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(CryptidPickup.Start))]
    private static bool PreStart(CryptidPickup __instance)
    {
       __instance.gameObject.SetActive(!LocationHandling.IsLocationCollected(__instance.name));
        return false;
    }

    // TODO: Test these once locations are enabled
}