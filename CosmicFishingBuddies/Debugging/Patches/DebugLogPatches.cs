using CosmicFishingBuddies.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.Debugging.Patches
{
    [HarmonyPatch(typeof(Debug))]
    internal static class DebugLogPatches
    {
		/*
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Debug.Log), new Type[] { typeof(object) })]
        public static void Debug_Log(object message) => CFBCore.LogInfo($"[UnityEngine.Debug.Log] {message}");

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Debug.LogWarning), new Type[] { typeof(object) })]
		public static void Debug_LogWarning(object message) => CFBCore.LogWarning($"[UnityEngine.Debug.Log] {message}");

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Debug.LogError), new Type[] { typeof(object) })]
		public static void Debug_LogError(object message) => CFBCore.LogError($"[UnityEngine.Debug.Log] {message}");
		*/

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Debug.LogException), new Type[] { typeof(Exception) })]
		public static void Debug_LogException(Exception exception) => CFBCore.LogError($"[UnityEngine.Debug.Log] {exception}");
	}
}
