using CosmicHorrorFishingBuddies.PlayerSync;
using HarmonyLib;
using Mirror;

namespace CosmicHorrorFishingBuddies.HarvestPOISync.Patches
{
	[HarmonyPatch]
	internal static class HarvestPOIPatches
	{
		public static bool disabled;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.OnStockUpdated))]
		public static void HarvestPOI_OnStockUpdated(HarvestPOI __instance)
		{
			if (__instance is PlacedHarvestPOI) return;

			if (!disabled)
			{
				var localID = NetworkClient.connection.identity.netId;
				NetworkHarvestPOIManager.Instance?.GetNetworkObject(__instance)?.SetStockCount(__instance.harvestable.GetStockCount(false), localID);
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.Update))]
		public static void HarvestPOI_Update(HarvestPOI __instance)
		{
			if (__instance is PlacedHarvestPOI) return;

			if (!disabled && NetworkClient.activeHost)
			{
				var networkObj = NetworkHarvestPOIManager.Instance?.GetNetworkObject(__instance);

				if (networkObj != null && networkObj.IsCurrentlySpecial != __instance.IsCurrentlySpecial)
				{
					networkObj.SetIsCurrentlySpecial(__instance.isCurrentlySpecial);
				}
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.Update))]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.OnDayNightChanged))]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.SetIsCurrentlySpecial))]
		public static bool DisableOnClient() => disabled || NetworkClient.activeHost;
	}
}
