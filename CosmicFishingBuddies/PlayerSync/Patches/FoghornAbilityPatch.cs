using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(FoghornAbility))]
	internal static class FoghornAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Activate))]
		public static void FoghornAbility_Activate() => NetworkPlayer.LocalPlayer?.SetFogHornActive(true);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Deactivate))]
		public static void FoghornAbility_Deactivate() => NetworkPlayer.LocalPlayer?.SetFogHornActive(false);
	}
}
