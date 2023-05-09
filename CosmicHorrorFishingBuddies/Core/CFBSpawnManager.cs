using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.HarvestPOISync;
using CosmicHorrorFishingBuddies.HarvestPOISync.Data;
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

		#region Bait
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
			SpawnNetworkPOICallback(sender.connectionToClient, networkBaitHarvestPOI);
		}
		#endregion

		#region Crab
		public void TrySpawnNetworkCrabPot(PlacedHarvestPOI crabPot)
		{
			try
			{
				if (NetworkHarvestPOIManager.Instance.IsHarvestPOITracked(crabPot)) return;

				CFBCore.LogInfo($"Spawning network crab pot for {PlayerManager.LocalNetID}");

				_previouslyCreatedLocalHarvestPOI = crabPot;

				CmdSpawnNetworkCrabPot(new SerializedCrabPotPOIDataWrapper(crabPot.Harvestable as SerializedCrabPotPOIData), NetworkPlayer.LocalPlayer.netIdentity);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't spawn network bait {e}");
			}
		}

		[Command(requiresAuthority = false)]
		private void CmdSpawnNetworkCrabPot(SerializedCrabPotPOIDataWrapper crabPotData, NetworkIdentity sender)
		{
			var networkPlacedHarvestPOI = CFBNetworkManager.PlacedHarvestPOIPrefab.SpawnWithServerAuthority().GetComponent<NetworkPlacedHarvestPOI>();

			networkPlacedHarvestPOI.SetCrabPotData(crabPotData, sender.netId);

			// Target the original creator so it can register its bait
			SpawnNetworkPOICallback(sender.connectionToClient, networkPlacedHarvestPOI);
		}
		#endregion

		[TargetRpc]
		private void SpawnNetworkPOICallback(NetworkConnectionToClient _, NetworkHarvestPOI networkPOI)
		{
			networkPOI.Target = _previouslyCreatedLocalHarvestPOI;
			_previouslyCreatedLocalHarvestPOI = null;

			if (networkPOI is NetworkBaitHarvestPOI networkBait)
			{
				NetworkHarvestPOIManager.Instance.RegisterNetworkBaitHarvestPOI(networkBait);
			}
			else if (networkPOI is NetworkPlacedHarvestPOI networkCrabPot)
			{
				NetworkHarvestPOIManager.Instance.RegisterPlacedHarvestPOI(networkCrabPot);
			}
		}
	}
}
