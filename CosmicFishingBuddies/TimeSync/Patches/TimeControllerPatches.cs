using CosmicFishingBuddies.PlayerSync;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicFishingBuddies.TimeSync.Patches
{
	[HarmonyPatch(typeof(TimeController))]
	internal static class TimeControllerPatches
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(TimeController.IsTimePassing))]
		public static void TimeController_IsTimePassing(ref bool __result)
		{
			if (TimeSyncManager.Instance != null)
			{
				// Always keep time passing
				__result = true;
			}
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(TimeController.GetTimePassageModifier))]
		public static void TimeController_GetTimePassageModifier(ref float __result)
		{
			if (TimeSyncManager.Instance != null)
			{
				// While connected, have time pass constantly at the movement rate
				__result = TimeSyncManager.Instance.TimePassageModifier;
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(TimeController.Update))]
		public static void TimeController_Update(TimeController __instance)
		{
			if (TimeSyncManager.Instance != null)
			{
				// Never let them freeze time
				__instance._freezeTime = false;
				NetworkPlayer.LocalPlayer?.CmdSetTimeMode(__instance.currentTimePassageMode);
			}
		}
	}
}
