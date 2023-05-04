using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmicFishingBuddies.PlayerSync.Patches
{
	[HarmonyPatch(typeof(Player))]
	internal static class PlayerPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(Player.Start))]
		public static void Player_Start()
		{
			CFBCore.Instance.PlayerLoaded.Invoke();
		}
	}
}
