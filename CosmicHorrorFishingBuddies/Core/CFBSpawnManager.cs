using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.HarvestPOISync;
using CosmicHorrorFishingBuddies.PlayerSync;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Core
{
	internal class CFBSpawnManager : NetworkBehaviour
	{
		public static CFBSpawnManager Instance;

		private HarvestPOI _previouslyCreatedLocalHarvestPOI;

		public void Awake()
		{
			Instance = this;
		}

		public void TrySpawnNetworkBait(BaitHarvestPOI bait)
		{
			try
			{
				if (NetworkHarvestPOIManager.Instance.IsHarvestPOITracked(bait)) return;

				CFBCore.LogInfo($"Spawning network bait for {PlayerManager.LocalNetID}");

				var zone = GameManager.Instance.Player.PlayerZoneDetector.GetCurrentZone();
				var position = new Vector3(GameManager.Instance.Player.BoatModelProxy.DeployPosition.position.x, 0f, GameManager.Instance.Player.BoatModelProxy.DeployPosition.position.z);
				var yRot = GameManager.Instance.Player.BoatModelProxy.DeployPosition.eulerAngles.y;
				var numFish = Mathf.CeilToInt(bait.HarvestPOIData.startStock);

				_previouslyCreatedLocalHarvestPOI = bait;

				CmdSpawnNetworkBait(zone, position, yRot, numFish, NetworkPlayer.LocalPlayer.netIdentity);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't spawn network bait {e}");
			}
		}

		[Command(requiresAuthority = false)]
		private void CmdSpawnNetworkBait(ZoneEnum zone, Vector3 position, float yRot, int numFish, NetworkIdentity sender)
		{
			var networkBaitHarvestPOI = CFBNetworkManager.BaitHarvestPOIPrefab.SpawnWithServerAuthority().GetComponent<NetworkBaitHarvestPOI>();

			networkBaitHarvestPOI.SetBaitData(zone, position, yRot, numFish, sender.netId);

			// Target the original creator so it can register its bait
			SpawnNetworkBaitCallback(sender.connectionToClient, networkBaitHarvestPOI);
		}

		[TargetRpc]
		private void SpawnNetworkBaitCallback(NetworkConnectionToClient _, NetworkBaitHarvestPOI networkBait)
		{
			if (networkBait.Target != null)
			{
				CFBCore.LogInfo("It already spawned extra bait");
				GameObject.DestroyImmediate(networkBait.Target);
			}

			networkBait.Target = _previouslyCreatedLocalHarvestPOI;
			_previouslyCreatedLocalHarvestPOI = null;
			NetworkHarvestPOIManager.Instance.RegisterNetworkBaitHarvestPOI(networkBait);
		}
	}
}
