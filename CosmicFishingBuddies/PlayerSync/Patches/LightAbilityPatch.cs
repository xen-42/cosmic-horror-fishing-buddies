using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(LightAbility))]
	internal static class LightAbilityPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(nameof(LightAbility.Activate))]
		public static void LightAbility_Activate() => NetworkPlayer.LocalPlayer?.SetLightActive(true);

		[HarmonyPrefix]
		[HarmonyPatch(nameof(LightAbility.Deactivate))]
		public static void LightAbility_Deactivate() => NetworkPlayer.LocalPlayer?.SetLightActive(false);
	}
}
