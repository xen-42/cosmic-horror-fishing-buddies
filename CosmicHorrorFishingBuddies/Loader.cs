using CosmicHorrorFishingBuddies.Core;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CosmicHorrorFishingBuddies
{
	public class Loader
	{
		public static void Initialize()
		{
			var gameObject = new GameObject("CosmicHorrorFishingBuddies");
			gameObject.AddComponent<CFBCore>();
			gameObject.DontDestroyOnLoad();
		}

		public static void Preload()
		{
			var dredgeFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).Parent.Parent.FullName;

			void TryDeleteDir(string path)
			{
				try
				{
					Directory.Delete(Path.Combine(dredgeFolder, path), true);
				}
				catch (Exception e)
				{
					CFBCore.LogWarning($"Failed to remove EOS dir [{path}] (ignore if not on Epic Games OR this isn't the initial setup) - {e.Message}");
				}
			}

			void TryDeleteFile(params string[] path)
			{
				var combinedPath = Path.Combine(path);
				try
				{
					File.Delete(Path.Combine(dredgeFolder, combinedPath));
				}
				catch (Exception e)
				{
					CFBCore.LogWarning($"Failed to remove EOS file [{combinedPath}] (ignore if not on Epic Games OR this isn't the initial setup) - {e.Message}");
				}
			}

			TryDeleteDir(".egstore");
			TryDeleteFile("DREDGE_Data", "Plugins", "x86", "EOSSDK-Win32-Shipping.dll");
			TryDeleteFile("DREDGE_Data", "Managed", "com.Epic.OnlineServices.dll");
			TryDeleteFile("DREDGE_Data", "Managed", "com.playeveryware.eos.core.dll");
			TryDeleteFile("DREDGE_Data", "Managed", "com.playeveryware.eos.dll");
			TryDeleteFile("DREDGE_Data", "Managed", "com.playeveryware.eos-Editor.steam.utility.dll");
			TryDeleteFile("EOSBootstrapper.exe");
			TryDeleteFile("EOSBootstrapper.ini");
		}
	}
}
