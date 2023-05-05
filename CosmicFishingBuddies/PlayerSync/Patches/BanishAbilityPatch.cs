using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(BanishAbility))]
	internal static class BanishAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(BanishAbility.Activate))]
		public static void BanishAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteBanishAbility?.ToggleAbility(true);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(BanishAbility.Deactivate))]
		public static void BanishAbility_Deactivate() => NetworkPlayer.LocalPlayer?.remoteBanishAbility?.ToggleAbility(false);
	}
}
