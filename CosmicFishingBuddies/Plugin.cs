using BepInEx;
using CommandTerminal;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CosmicFishingBuddies
{
	[BepInPlugin("com.xen-42.dredge.cosmic-fishing-buddies", "Cosmic Fishing Buddies", "0.0.1")]
	[BepInProcess("DREDGE.exe")]
	[HarmonyPatch]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance { get; private set; }

		public static GameObject terminal;

		private void Awake()
		{
			Instance = this;

			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
