using BepInEx;
using HarmonyLib;
using kcp2k;
using System.Reflection;
using UnityEngine;

namespace CosmicFishingBuddies
{
	[BepInPlugin("com.xen-42.dredge.cosmic-fishing-buddies", "Cosmic Fishing Buddies", "0.0.1")]
	[BepInProcess("DREDGE.exe")]
	[HarmonyPatch]
	public class CFBCore : BaseUnityPlugin
	{
		public static CFBCore Instance { get; private set; }

		public static GameObject terminal;

		private void Awake()
		{
			Instance = this;

			// Plugin startup logic
			Logger.LogInfo($"Cosmic Fishing Buddies is loaded!");

			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

			Application.runInBackground = true;
		
			var networkManagerObj = new GameObject("NetworkManager");
			networkManagerObj.AddComponent<CFBNetworkManager>();
			GameObject.DontDestroyOnLoad(networkManagerObj);
		}

		public static void Log(object msg) => Instance.Logger.LogInfo(msg);
		public static void LogError(object msg) => Instance.Logger.LogError(msg);
		public static void LogWarning(object msg) => Instance.Logger.LogWarning(msg);
	}
}
