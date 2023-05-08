using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch]
	internal static class TrawlNetAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TrawlNetAbility), nameof(TrawlNetAbility.Activate))]
		public static void TrawlNetAbility_Activate(TrawlNetAbility __instance)
		{
			// Activate acts as a toggle for the trawl net so only toggle on if it actually is on
			if (__instance.isActive)
			{
				NetworkPlayer.LocalPlayer?.remoteTrawlNetAbility?.Toggle(true);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(TrawlNetAbility), nameof(TrawlNetAbility.Deactivate))]
		public static void TrawlNetAbility_Deactivate()
		{
			NetworkPlayer.LocalPlayer?.remoteTrawlNetAbility?.Toggle(false);
		}
	}
}
