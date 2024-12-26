using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.UI;
using CosmicHorrorFishingBuddies.Util;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CosmicHorrorFishingBuddies.Debugging;

[JsonObject(MemberSerialization.OptIn)]
internal class DebugController : MonoBehaviour
{
	public static DebugController Instance { get; private set; }

	[JsonProperty]
	public bool DebugEnabled = false;

	[JsonProperty]
	public bool QuickLoad = false;
	private bool _hasQuickLoaded = false;

	public void Awake()
	{
		Instance = this;

		var path = Path.Combine(CFBCore.GetModFolder(), "debug_options.json");
		if (File.Exists(path))
		{
			var file = File.ReadAllText(path);
			JsonConvert.PopulateObject(file, this);
		}

		// Make sure it's up to date idk
		File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));

		if (DebugEnabled)
		{
			gameObject.AddComponent<DebugKeyPadCommands>();
			gameObject.AddComponent<TerminalCommands>();
		}

		if (QuickLoad)
		{
			CFBCore.LogInfo("QUICK LOAD ENABLED!");
			// Would load directly on startup but wasn't working
			// Wonder if it's intro skipper's fault
			Delay.RunWhen(() => SceneManager.GetActiveScene().name == Scenes.Title, DoQuickLoad);

			CFBSceneManager.GameSceneLoaded += OnGameSceneLoaded;
		}
	}

	private void DoQuickLoad()
	{
		if (!_hasQuickLoaded)
		{
			Delay.FireInNUpdates(10, () =>
			{
				try
				{
					var instances = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Count();
					CFBCore.LogInfo($"Open game instances: {instances}");

					if (instances == 1)
					{
						CFBCore.LogInfo("QuickStart enabled, hosting game");

						// Start hosting on first save slot
						CFBNetworkManager.Instance.SetConnection(true, "localhost", TransportType.KCP);
						GameManager.Instance.SettingsSaveData.lastSaveSlot = 0;
						GameManager.Instance.Loader.LoadGameFromTitle();
					}
					else
					{
						CFBCore.LogInfo("QuickStart enabled, joining existing game");

						// Join existing game
						CFBNetworkManager.Instance.SetConnection(false, "localhost", TransportType.KCP);
						GameManager.Instance.SettingsSaveData.lastSaveSlot = 0;
						GameManager.Instance.Loader.LoadGameFromTitle();
					}
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to quick load {e}");
				}
			});
		}
	}

	private void OnGameSceneLoaded()
	{
		var debugDisplay = new GameObject("DebugDisplay").AddComponent<DebugDisplay>();
		debugDisplay.transform.parent = GameObject.Find("GameCanvases/GameCanvas").transform;
		debugDisplay.transform.localPosition = new Vector3(-700, 400, 0);
	}
}
