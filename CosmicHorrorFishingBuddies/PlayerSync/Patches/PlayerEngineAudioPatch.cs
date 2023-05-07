using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(PlayerEngineAudio))]
	internal static class PlayerEngineAudioPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(nameof(PlayerEngineAudio.UpdateEngineSound))]
		public static void PlayerEngineAudio_UpdateEngineSound(PlayerEngineAudio __instance)
		{
			NetworkPlayer.LocalPlayer?.remotePlayerEngineAudio?.UpdateEngineSound(__instance.audioSource.volume, __instance.audioSource.pitch);
		}
	}
}
