using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch]
	internal static class TeleportAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TeleportAbility), nameof(TeleportAbility.Activate))]
		public static void TeleportAbility_Activate()
		{
			NetworkPlayer.LocalPlayer?.remoteTeleportAbility?.Toggle(true);
			NetworkPlayer.LocalPlayer?.CmdPlayOneShot(AudioSync.AudioEnum.MANIFEST, 1f, 1f);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameEvents), nameof(GameEvents.TriggerTeleportComplete))]
		public static void GameEvents_TriggerTeleportComplete() => NetworkPlayer.LocalPlayer?.remoteTeleportAbility?.Toggle(false);
	}
}
