using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch(typeof(FoghornAbility))]
	internal static class FoghornAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Activate))]
		public static void FoghornAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteFoghornAbility?.Toggle(true);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Deactivate))]
		public static void FoghornAbility_Deactivate() => NetworkPlayer.LocalPlayer?.remoteFoghornAbility?.Toggle(false);
	}
}
