using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.EnvironmentSync.Patches
{
	[HarmonyPatch(typeof(WaveController))]
	public static class WaveControllerPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(WaveController.SetWaveSteepness))]
		[HarmonyPatch(nameof(WaveController.SetWaveSpeed))]
		[HarmonyPatch(nameof(WaveController.SetWaveLength))]
		[HarmonyPatch(nameof(WaveController.SetWaveDirections))]
		public static void UpdateWaveParameters(WaveController __instance)
		{
			// If the host uses console commands to change the waves, send this info to all players
			if (CFBCore.IsHost)
			{
				EnvironmentSyncManager.UpdateFromLocalWaveController();
			}
		}
	}
}
