using CosmicFishingBuddies.AudioSync;
using HarmonyLib;
using Sirenix.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Winch.Core;

namespace CosmicFishingBuddies
{
	public class CFBCore : MonoBehaviour
	{
		public static CFBCore Instance { get; private set; }

		public static GameObject terminal;

		public UnityEvent PlayerLoaded = new();

		public void Awake()
		{
			try
			{
				Instance = this;

				// Plugin startup logic
				LogInfo($"Cosmic Fishing Buddies is loaded!");

				// [RunTimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] Attribute methods just never get called since this is a mod not Unity
				// we have to manually initialize them
				InitAssemblies();

				// Winch loads the mod too late for this to work for the SteamAPI patch, have to rely on Winch's patching instead
				new Harmony(nameof(CFBCore)).PatchAll();

				Application.runInBackground = true;

				var managerObj = new GameObject("CFBManager");
				managerObj.AddComponent<CFBNetworkManager>();
				managerObj.AddComponent<AudioClipManager>();
				GameObject.DontDestroyOnLoad(managerObj);

				Application.logMessageReceived += Application_logMessageReceived;
			}
			catch (Exception e)
			{
				LogError($"Failed to load Cosmic Fishing Buddies: {e}");
			}
		}

		private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
		{
			LogInfo($"{condition} : {stackTrace}");
		}

		public static string GetModFolder() => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

		private static void InitAssemblies()
		{
			// JohnCorby is a hero
			// Stolen from QSB

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

		public static void LogInfo(object msg) => WinchCore.Log.Info(msg);
		public static void LogError(object msg) => WinchCore.Log.Error(msg);
		public static void LogWarning(object msg) => WinchCore.Log.Warn(msg);
	}
}
