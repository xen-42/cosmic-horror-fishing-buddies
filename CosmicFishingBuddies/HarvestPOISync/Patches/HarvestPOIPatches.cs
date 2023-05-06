using CosmicFishingBuddies.PlayerSync;
using HarmonyLib;
using Mirror;

namespace CosmicFishingBuddies.HarvestPOISync.Patches
{
	[HarmonyPatch]
	internal static class HarvestPOIPatches
	{
		public static bool disabled;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.OnStockUpdated))]
		public static void HarvestPOI_OnStockUpdated(HarvestPOI __instance)
		{
			if (!disabled)
			{
				NetworkHarvestPOIManager.Instance?.GetNetworkObject(__instance)?.SetStockCount(__instance.harvestable.GetStockCount(false), NetworkPlayer.LocalPlayer.netId);
			}
		}
	}
}
