using CosmicFishingBuddies.AudioSync;
using HarmonyLib;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(PlayerCollisionAudio))]
	internal static class PlayerCollisionAudioPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(PlayerCollisionAudio.PlayRandom))]
		[HarmonyPatch(nameof(PlayerCollisionAudio.PlayRandomSafe))]
		public static void PlayerCollisionAudio_PlayRandom() => NetworkPlayer.LocalPlayer.CmdPlayOneShot(AudioEnum.PLAYER_COLLISION, 1f, 1f);
	}
}
