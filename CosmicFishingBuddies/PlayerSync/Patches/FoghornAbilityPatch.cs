using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(FoghornAbility))]
	internal class FoghornAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Activate))]
		public static void FoghornAbility_Activate()
		{
			if (NetworkPlayer.LocalPlayer != null)
			{
				NetworkPlayer.LocalPlayer.fogHornActive = true;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(FoghornAbility.Deactivate))]
		public static void FoghornAbility_Deactivate()
		{
			if (NetworkPlayer.LocalPlayer != null)
			{
				NetworkPlayer.LocalPlayer.fogHornActive = false;
			}
		}
	}
}
