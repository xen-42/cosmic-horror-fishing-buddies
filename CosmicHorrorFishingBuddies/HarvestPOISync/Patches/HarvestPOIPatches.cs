﻿using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;
using Mirror;
using System;

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
			if (__instance.IsCrabPotPOI) return;

			if (!disabled)
			{
				try
				{
					var networkObj = NetworkHarvestPOIManager.Instance?.GetNetworkObject(__instance);
					if (networkObj is NetworkStockableHarvestPOI stockableObj)
					{
						stockableObj.SetStockCount(__instance.harvestable.GetStockCount(false));
					}
				}
				catch (Exception e)
				{
					CFBCore.LogError(e);
				}
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(HarvestPOI), nameof(HarvestPOI.Update))]
		public static void HarvestPOI_Update(HarvestPOI __instance)
		{
			if (__instance.IsCrabPotPOI || __instance.IsBaitPOI) return;

			if (!disabled && NetworkClient.activeHost)
			{
				var networkObj = NetworkHarvestPOIManager.Instance?.GetNetworkObject(__instance);

				if (networkObj is NetworkStockableHarvestPOI stockableObj && stockableObj.IsCurrentlySpecial != __instance.IsCurrentlySpecial)
				{
					stockableObj.SetIsCurrentlySpecial(__instance.isCurrentlySpecial);
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
