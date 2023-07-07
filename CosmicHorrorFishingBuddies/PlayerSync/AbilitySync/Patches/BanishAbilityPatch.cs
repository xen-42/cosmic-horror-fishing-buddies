using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch(typeof(BanishAbility))]
	internal static class BanishAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(BanishAbility.Activate))]
		public static void BanishAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteBanishAbility?.Toggle(true);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(BanishAbility.Deactivate))]
		public static void BanishAbility_Deactivate() => NetworkPlayer.LocalPlayer?.remoteBanishAbility?.Toggle(false);
	}
}
