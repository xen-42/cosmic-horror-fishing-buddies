using HarmonyLib;

namespace CosmicHorrorFishingBuddies.Core.Patches;

[HarmonyPatch(typeof(SKUSpecificDisabler))]
public static class SKUSpecificDisablerPatches
{
	[HarmonyPrefix, HarmonyPatch(nameof(SKUSpecificDisabler.Awake))]
	public static bool SKUSpecificDisable_Awake(SKUSpecificDisabler __instance)
	{
		// If something only works on Epic we don't want it
		if (!__instance.supportedPlatforms.HasFlag(Platform.PC_GOG) && !__instance.supportedPlatforms.HasFlag(Platform.PC_STEAM))
		{
			if (__instance.destroyIfUnavailable)
			{
				UnityEngine.Object.Destroy(__instance.gameObject);
			}
			else
			{
				__instance.gameObject.SetActive(false);
			}
			return false;
		}
		else
		{
			return true;
		}
	}
}
