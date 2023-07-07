using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.HarvestPOISync.Patches
{
	[HarmonyPatch(typeof(PlacedHarvestPOI))]
	internal class PlacedHarvestPOIPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(PlacedHarvestPOI.Awake))]
		public static void PlacedHarvestPOI_Awake(PlacedHarvestPOI __instance)
		{
			// PlacedHarvestPOI has no Start method else I'd use that
			Delay.FireOnNextUpdate(() => Start(__instance));
		}

		private static void Start(PlacedHarvestPOI instance)
		{
			CFBSpawnManager.Instance?.TrySpawnNetworkCrabPot(instance);
		}
	}
}
