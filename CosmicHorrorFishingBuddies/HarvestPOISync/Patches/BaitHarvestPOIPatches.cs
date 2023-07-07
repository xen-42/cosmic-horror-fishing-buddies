using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.HarvestPOISync.Patches
{
	[HarmonyPatch(typeof(BaitHarvestPOI))]
	internal class BaitHarvestPOIPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(BaitHarvestPOI.Awake))]
		public static void BaitHarvestPOI_Awake(BaitHarvestPOI __instance)
		{
			// BaitHarvestPOI has no Start method else I'd use that
			Delay.FireOnNextUpdate(() => Start(__instance));
		}

		private static void Start(BaitHarvestPOI instance)
		{
			CFBSpawnManager.Instance?.TrySpawnNetworkBait(instance);
		}

		/*
		[HarmonyPrefix]
		[HarmonyPatch(nameof(BaitHarvestPOI.OnDestroy))]
		public static void BaitHarvestPOI_OnDestroy(BaitHarvestPOI __instance) => NetworkHarvestPOIManager.Instance?.TryDestroyNetworkBait(__instance);
		*/
	}
}
