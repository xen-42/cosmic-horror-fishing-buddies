using HarmonyLib;
using System;
using System.Threading.Tasks;
using UnityAsyncAwaitUtil;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Core.Patches;

[HarmonyPatch]
public static class GameManagerPatches
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(GameManager), nameof(GameManager.WaitForAllAsyncManagers))]
	public static bool GameManager_WaitForAllAsyncManagers(GameManager __instance)
	{
		Task.Run(new Action(() => WaitForAllAsyncManagers(__instance)));
		return false;
	}

	private static async void WaitForAllAsyncManagers(GameManager manager)
	{
		// Do not wait for things relating to Epic since we want to use our own EOS setup for multiplayer
		while (!manager.ConsoleManager.Initialized)
		{
			await Awaiters.NextFrame;
		}
		await Awaiters.MainThread;
		manager.SaveManager.Init();
		manager.VibrationManager.Init();
		manager.HasLoadedAsyncManagers = true;
	}
}
