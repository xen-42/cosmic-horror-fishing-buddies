using BepInEx;
using BepInEx.Logging;
using FluffyUnderware.DevTools.Extensions;
using HarmonyLib;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CosmicFishingBuddies
{
	[BepInPlugin("com.xen-42.dredge.cosmic-fishing-buddies", "Cosmic Fishing Buddies", "0.0.1")]
	[BepInProcess("DREDGE.exe")]
	[HarmonyPatch]
	public class CFBCore : BaseUnityPlugin
	{
		private static string _guid = "com.xen-42.dredge.cosmic-fishing-buddies";

		public static CFBCore Instance { get; private set; }

		public static GameObject terminal;

		private void Awake()
		{
			Instance = this;

			// Plugin startup logic
			Logger.LogInfo($"Cosmic Fishing Buddies is loaded!");

			// [RunTimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] Attribute methods just never get called since this is a mod not Unity
			// we have to manually initialize them
			InitAssemblies();

			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

			Application.runInBackground = true;
		
			var networkManagerObj = new GameObject("NetworkManager");
			networkManagerObj.AddComponent<CFBNetworkManager>();
			GameObject.DontDestroyOnLoad(networkManagerObj);

			Application.logMessageReceived += Application_logMessageReceived;
		}

		private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
		{
			Log(type, $"{condition} : {stackTrace}");
		}

		public static string GetModFolder() => Path.Combine(Paths.PluginPath, _guid);

		private static void InitAssemblies()
		{
			// JohnCorby is a hero

			static void Init(Assembly assembly)
			{
				LogInfo(assembly.ToString());
				assembly
					.GetTypes()
					.SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
					.Where(x => x.IsDefined(typeof(RuntimeInitializeOnLoadMethodAttribute)))
					.ForEach(x => x.Invoke(null, null));
			}

			LogInfo("Running RuntimeInitializeOnLoad methods for our assemblies");
			foreach (var path in Directory.EnumerateFiles(GetModFolder(), "*.dll"))
			{
				var assembly = Assembly.LoadFile(path);
				Init(assembly);
			}

			LogInfo("Assemblies initialized");
		}

		public static void Log(LogType type, object msg) => Instance.Logger.Log(ConvertLogType(type), msg);
		public static void LogInfo(object msg) => Instance.Logger.LogInfo(msg);
		public static void LogError(object msg) => Instance.Logger.LogError(msg);
		public static void LogWarning(object msg) => Instance.Logger.LogWarning(msg);

		private static LogLevel ConvertLogType(LogType type) => type switch
		{
			LogType.Exception or LogType.Assert or LogType.Error => LogLevel.Error,
			LogType.Warning => LogLevel.Warning,
			_ => LogLevel.Info,
		};
	}
}
