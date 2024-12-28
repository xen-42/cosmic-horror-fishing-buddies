using CosmicHorrorFishingBuddies.Debugging;
using CosmicHorrorFishingBuddies.Save;
using CosmicHorrorFishingBuddies.UI;
using CosmicHorrorFishingBuddies.Util;
using HarmonyLib;
using Mirror;
using Sirenix.Utilities;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Winch.Core;

namespace CosmicHorrorFishingBuddies.Core
{
	public class CFBCore : MonoBehaviour
	{
		public static CFBCore Instance { get; private set; }

		public static GameObject terminal;

		public const string INTRO_SKIP_ARG = "skipintro";

		public UnityEvent PlayerLoaded = new();
		public UnityEvent<string> SwitchSceneRequested = new();

		public static bool IsHost => NetworkServer.activeHost;

		private bool _restarting;

		public void Awake()
		{
			try
			{
				Instance = this;

				LogInfo($"Cosmic Fishing Buddies is loaded!");

				InitAssemblies();

				new Harmony(nameof(CFBCore)).PatchAll();

				gameObject.AddComponent<DebugController>();
				gameObject.AddComponent<UIHelper>();
				gameObject.AddComponent<CFBNetworkManager>();
				gameObject.AddComponent<CFBSceneManager>();

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
			// Have to manually load the EOSDK shipping library since this is a mod
			[DllImport("Kernel32.dll", SetLastError = true)]
			static extern IntPtr LoadLibrary(string dllToLoad);

			var libPath = Path.Combine(GetModFolder(), "lib\\EOSSDK-Win32-Shipping.dll");
			var libraryPointer = LoadLibrary(libPath);

			if (libraryPointer == IntPtr.Zero)
			{
				LogError($"Failed to load EOS SDK library {libPath} - error code: {Marshal.GetLastWin32Error()}");
			}
			else
			{
				LogInfo($"Loaded EOS SDK library");
			}

			// JohnCorby is a hero (stolen from QSB)
			// [RunTimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)] Attribute methods just never get called since this is a mod not Unity
			// We have to manually initialize them
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

		public static void LogInfo(object msg)
		{
			try
			{
				WinchCore.Log.Info(msg);
			}
			catch { }
		}

		public static void LogError(object msg)
		{
			try
			{
				WinchCore.Log.Error(msg);
			}
			catch { }
		}

		public static void LogWarning(object msg)
		{
			try
			{
				WinchCore.Log.Warn(msg);
			}
			catch { }
		}

		public static bool CanRestart()
		{
			return !SaveManagerPatches.IsSaving && !SaveManagerPatches.IsLoading && !SaveManagerPatches.IsSavingSettings;
		}

		public static void RestartGame()
		{
			if (Instance._restarting)
			{
				LogError("Game is already restarting");
				return;
			}
			else
			{
				Instance._restarting = true;

				// Save their game even though they aren't at a dock its fine
				if (SceneManager.GetActiveScene().name == Scenes.Game)
				{
					GameManager.Instance.Save();
				}

				// Make sure we aren't saving/loading any data before restarting to prevent corruption
				Delay.RunWhen(CanRestart, RestartInternal);
			}
		}

		private static void RestartInternal()
		{
			// Don't restart if we have QuickLoad on else we get trapped forever
			if (!DebugController.Instance.QuickLoad)
			{
				LogInfo("Restarting the game");
				var args = Environment.GetCommandLineArgs().ToList();
				if (!args.Contains(INTRO_SKIP_ARG)) args.Add(INTRO_SKIP_ARG);
				Process.Start(Application.dataPath.Replace("_Data", ".exe"), string.Join(" ", args));
			}

			Application.Quit();
		}
	}
}