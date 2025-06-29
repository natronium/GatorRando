using HarmonyLib;
using UnityEngine;
namespace GatorRando.Patches;

[HarmonyPatch(typeof(MultilingualTextDocument))]
static class MultilingualTextDocumentPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("FetchChunk")]
    static void PreFetchChunk(MultilingualTextDocument __instance, string id)
    {
        int num = Animator.StringToHash(id);
		foreach (DialogueChunk dialogueChunk in __instance.chunks)
		{
            Plugin.LogDebug(dialogueChunk.lines[0].english[0]);
			if (dialogueChunk.id == num)
            {
                Plugin.LogDebug("Found it");
            }
		}
		Debug.LogError("No chunk of id " + id + " found");
    }
}