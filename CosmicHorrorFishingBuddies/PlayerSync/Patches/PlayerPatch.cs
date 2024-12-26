using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(Player))]
	internal static class PlayerPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Player.Start))]
		public static void Player_Start()
		{
			CFBCore.Instance.PlayerLoaded.Invoke();
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Player.Dock))]
		public static void Player_Dock() => NetworkPlayer.LocalPlayer?.SetIsDocked(true);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Player.Undock))]
		public static void Player_Undock() => NetworkPlayer.LocalPlayer?.SetIsDocked(false);
	}
}
