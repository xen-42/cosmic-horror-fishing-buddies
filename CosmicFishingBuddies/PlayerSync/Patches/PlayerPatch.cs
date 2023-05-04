using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Player.Dock))]
		public static void Player_Dock() => NetworkPlayer.LocalPlayer?.CmdSetIsDocked(true);

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Player.Undock))]
		public static void Player_Undock() => NetworkPlayer.LocalPlayer?.CmdSetIsDocked(false);
	}
}
