using HarmonyLib;
using Steamworks;

namespace CosmicFishingBuddies.Steam
{
	[HarmonyPatch]
	internal static class SteamPatches
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(SteamAPI), nameof(SteamAPI.RestartAppIfNecessary))]
		public static bool SteamAPI_RestartAppIfNecessary(ref bool __result)
		{
			// For testing purposes (i.e., opening two steam instances) we want to ensure it doesn't restart
			__result = false;
			return false;
		}
	}
}
