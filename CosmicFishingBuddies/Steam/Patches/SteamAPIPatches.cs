using HarmonyLib;
using Steamworks;

namespace CosmicFishingBuddies.Steam.Patches
{
    [HarmonyPatch(typeof(SteamAPI))]
    internal static class SteamAPIPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(SteamAPI.RestartAppIfNecessary))]
        public static bool SteamAPI_RestartAppIfNecessary(ref bool __result)
        {
            // For testing purposes (i.e., opening two steam instances) we want to ensure it doesn't restart
            __result = false;
            return false;
        }
    }
}
