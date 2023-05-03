using HarmonyLib;
using Steamworks;

namespace SteamAPIPatcher
{
	[HarmonyPatch(typeof(SteamAPI))]
	[HarmonyPatch(nameof(SteamAPI.RestartAppIfNecessary))]
	class Patch
	{
		static bool Prefix(ref bool __result)
		{
			// For testing purposes (i.e., opening two steam instances) we want to ensure it doesn't restart
			__result = false;
			return false;
		}
	}
}
