using HarmonyLib;
using System;
using System.Linq;

namespace CosmicHorrorFishingBuddies.Core.Patches;

[HarmonyPatch(typeof(SplashController))]
internal class SplashControllerPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(nameof(SplashController.OnEnable))]
	public static bool SplashController_OnEnable()
	{
		if (Environment.GetCommandLineArgs().Contains(CFBCore.INTRO_SKIP_ARG))
		{
			CFBCore.LogInfo("Skipping splash screen");
			GameManager.Instance.Loader.LoadStartupFromSplash();
			return false;
		}
		else
		{
			return true;
		}
	}
}
