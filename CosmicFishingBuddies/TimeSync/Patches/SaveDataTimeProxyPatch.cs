﻿using HarmonyLib;

namespace CosmicFishingBuddies.TimeSync.Patches
{
	[HarmonyPatch(typeof(SaveDataTimeProxy))]
	internal static class SaveDataTimeProxyPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(SaveDataTimeProxy.GetTimeAndDay))]
		public static void SaveDataTimeProxy_GetTimeAndDay(ref float __result)
		{
			if (TimeSyncManager.Instance != null)
			{
				if (TimeSyncManager.Instance.isServer)
				{
					TimeSyncManager.Instance.TimeAndDay = __result;
				}
				else
				{
					__result = TimeSyncManager.Instance.TimeAndDay;
				}
			}
		}
	}
}
