using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Core.Patches;

[HarmonyPatch]
public static class EOSHostManagerPatches
{
	private static IEnumerable<MethodBase> _targetMethods;

	private static bool _initialized;

	private static void GetMethods()
	{
		try
		{
			var baseTypes = Assembly.LoadFrom(Path.Combine(Application.dataPath, "Managed", "Assembly-CSharp.dll")).GetTypes();
			var hostManager = baseTypes.FirstOrDefault(x => x.Name == "EOSHostManager");
			CFBCore.LogInfo($"Found EOSHostManager class: [{hostManager}]");

			var playEveryWareTypes = Assembly.LoadFrom(Path.Combine(Application.dataPath, "Managed", "com.playeveryware.eos.core.dll")).GetTypes();
			var manager = playEveryWareTypes.FirstOrDefault(x => x.Name == "EOSManager");
			CFBCore.LogInfo($"Found EOSManager class: [{manager}]");

			var singleton = playEveryWareTypes.FirstOrDefault(x => x.Name == "EOSSingleton");
			CFBCore.LogInfo($"Found EOSSingleton class: [{manager}]");

			_targetMethods =
			[
				manager.GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic),
				manager.GetMethod("Update", BindingFlags.Instance | BindingFlags.NonPublic),
				manager.GetMethod("OnApplicationFocus", BindingFlags.Instance | BindingFlags.NonPublic),
				manager.GetMethod("OnApplicationPause", BindingFlags.Instance | BindingFlags.NonPublic),
				manager.GetMethod("OnEnable", BindingFlags.Instance | BindingFlags.NonPublic),
				manager.GetMethod("OnDisable", BindingFlags.Instance | BindingFlags.NonPublic),
				//manager.GetMethod("OnApplicationQuitting", BindingFlags.Instance | BindingFlags.NonPublic),
				//singleton.GetMethod("GetOrCreateManager", BindingFlags.Instance | BindingFlags.Public),
				//singleton.GetMethod("RemoveManager", BindingFlags.Instance | BindingFlags.Public),
				singleton.GetMethod("InitializePlatformInterface", BindingFlags.Instance | BindingFlags.NonPublic),
				singleton.GetMethod("CreatePlatformInterface", BindingFlags.Instance | BindingFlags.NonPublic),
				hostManager.GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic)
			];
			_targetMethods = _targetMethods.Where(x => x != null);

			CFBCore.LogInfo($"Patching {_targetMethods.Count()} EOS methods");
			foreach (var method in _targetMethods)
			{
				CFBCore.LogInfo($"Patching {method.DeclaringType.Name}.{method.Name}");
			}
		}
		catch (Exception e)
		{
			CFBCore.LogInfo($"Could not patch EOS classes - ignore if not on Epic Games: [{e}]");

			_targetMethods = null;
		}

		_initialized = true;
	}

	public static bool Prepare()
	{
		if (!_initialized)
		{
			GetMethods();
		}

		return _targetMethods != null && _targetMethods.Any();
	}

	public static IEnumerable<MethodBase> TargetMethods() => _targetMethods;

public static bool Prefix() => false;
}
