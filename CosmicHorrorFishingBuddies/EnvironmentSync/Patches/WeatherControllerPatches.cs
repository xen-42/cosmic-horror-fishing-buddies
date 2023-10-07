using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Messaging;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.EnvironmentSync.Patches;

[HarmonyPatch(typeof(WeatherController))]
internal class WeatherControllerPatches
{
	// Only host decides the weather
	[HarmonyPrefix]
	[HarmonyPatch(nameof(WeatherController.PickNewWeather))]
	public static bool WeatherController_PickNewWeather_Prefix() => CFBCore.IsHost;

	[HarmonyPostfix]
	[HarmonyPatch(nameof(WeatherController.PickNewWeather))]
	public static void WeatherController_PickNewWeather_Postfix(WeatherController __instance)
	{
		if (CFBCore.IsHost)
		{
			new WeatherMessage(__instance.allWeather.IndexOf(__instance.currentWeatherData)).Send();
		}
	}
}
