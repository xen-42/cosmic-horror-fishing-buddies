using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch]
	internal static class TeleportAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TeleportAbility), nameof(FoghornAbility.Activate))]
		public static void FoghornAbility_Activate() => NetworkPlayer.LocalPlayer?.remoteTeleportAbility?.ToggleAbility(true);

		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameEvents), nameof(GameEvents.TriggerTeleportComplete))]
		public static void GameEvents_TriggerTeleportComplete() => NetworkPlayer.LocalPlayer?.remoteTeleportAbility?.ToggleAbility(false);
	}
}
