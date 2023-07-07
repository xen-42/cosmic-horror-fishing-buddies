using CosmicHorrorFishingBuddies.Core;
using HarmonyLib;
using System;

namespace CosmicHorrorFishingBuddies.PlayerSync.Events.Patches
{
	[HarmonyPatch(typeof(FlickerLightsWorldEvent))]
	internal class FlickerLightsWorldEventPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(FlickerLightsWorldEvent.Activate))]
		public static void FlickerLightsWorldEvent_Activate()
		{
			try
			{
				NetworkPlayer.LocalPlayer?.remoteLightAbility?.FlickerLights();
			}
			catch (Exception e)
			{
				CFBCore.LogError(e);
			}
		}
	}
}
