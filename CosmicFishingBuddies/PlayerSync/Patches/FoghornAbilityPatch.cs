using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(FoghornAbility))]
	internal class FoghornAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Activate))]
		public static void FoghornAbility_Activate() => NetworkPlayer.LocalPlayer.fogHornActive = true;

		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Deactivate))]
		public static void FoghornAbility_Deactivate() => NetworkPlayer.LocalPlayer.fogHornActive = false;
	}
}
