using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.Unity.Patches
{
    [HarmonyPatch(typeof(Debug))]
    internal static class DebugPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Debug.Log), new Type[] { typeof(object) })]
        public static void Debug_Log(object message) => CFBCore.Log($"[UnityEngine.Debug.Log] {message}");

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Debug.LogWarning), new Type[] { typeof(object) })]
		public static void Debug_LogWarning(object message) => CFBCore.LogWarning($"[UnityEngine.Debug.Log] {message}");

		[HarmonyPostfix]
		[HarmonyPatch(nameof(Debug.LogError), new Type[] { typeof(object) })]
		public static void Debug_LogError(object message) => CFBCore.LogError($"[UnityEngine.Debug.Log] {message}");
	}
}
