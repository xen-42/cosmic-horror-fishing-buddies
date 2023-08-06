using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Messaging;
using CosmicHorrorFishingBuddies.PlayerSync;
using CosmicHorrorFishingBuddies.Util;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.EnvironmentSync
{
	[AddToGameScene]
	internal class EnvironmentSyncManager : MonoBehaviour
	{
		public void Awake()
		{
			PlayerManager.PlayerJoined += OnPlayerJoined;
		}

		public void OnDestroy()
		{
			PlayerManager.PlayerJoined -= OnPlayerJoined;
		}

		private void OnPlayerJoined(bool isLocalPlayer, uint id)
		{
			// Initial sync
			if (!isLocalPlayer && CFBCore.IsHost)
			{
				UpdateFromLocalWaveController(id);
			}
		}

		public static void UpdateFromLocalWaveController(uint to = uint.MaxValue)
		{
			CFBCore.LogInfo("TRYING TO SEND MESSAGE");
			var controller = GameManager.Instance.WaveController;
			new WaveMessage((controller.steepness, controller.wavelength, controller.speed, controller.waveDirections)) { To = to }.Send();
		}
	}
}
