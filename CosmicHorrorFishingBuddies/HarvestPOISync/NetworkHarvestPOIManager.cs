using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.PlayerSync;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.HarvestPOISync
{
	internal class NetworkHarvestPOIManager : NetworkBehaviour
	{
		public static NetworkHarvestPOIManager Instance { get; private set; }

		private HarvestPOI _previouslyCreatedLocalHarvestPOI;

		public List<HarvestPOI> SortedHarvestPOIs { get; private set; }
		private Dictionary<HarvestPOI, NetworkHarvestPOI> _lookUp = new();

		private Dictionary<BaitHarvestPOI, NetworkBaitHarvestPOI> _baitLookUp = new();

		public void Start()
		{
			try
			{
				Instance = this;

				SortedHarvestPOIs = GameManager.Instance.HarvestPOIManager.allHarvestPOIs;
				SortedHarvestPOIs.OrderBy(x => x.name);

				if (NetworkClient.activeHost)
				{
					for (int i = 0; i < SortedHarvestPOIs.Count; i++)
					{
						var poi = SortedHarvestPOIs[i];
						var networkPOI = CFBNetworkManager.IndexedHarvestPOIPrefab.SpawnWithServerAuthority().GetComponent<IndexedNetworkHarvestPOI>();
						networkPOI.SetSyncIndex(i);
						_lookUp[poi] = networkPOI;
					}
				}
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't instance {nameof(NetworkHarvestPOIManager)} : {e}");
			}
		}

		public void RegisterIndexedNetworkHarvestPOI(IndexedNetworkHarvestPOI networkPOI)
		{
			var harvestPOI = SortedHarvestPOIs[networkPOI.SyncIndex];
			_lookUp[harvestPOI] = networkPOI;
			networkPOI.Target = harvestPOI;
		}

		public void RegisterNetworkBaitHarvestPOI(NetworkBaitHarvestPOI networkPOI)
		{
			_baitLookUp[networkPOI.Target as BaitHarvestPOI] = networkPOI;
		}

		public void TryDestroyNetworkBait(BaitHarvestPOI bait)
		{
			if (_baitLookUp.TryGetValue(bait, out var networkBait))
			{
				_baitLookUp.Remove(bait);
				if (NetworkClient.activeHost)
				{
					NetworkServer.Destroy(networkBait.gameObject);
				}
			}
		}

		public bool IsHarvestPOITracked(HarvestPOI harvestPOI)
		{
			return (harvestPOI is BaitHarvestPOI baitHarvestPOI && _baitLookUp.ContainsKey(baitHarvestPOI)) || _lookUp.ContainsKey(harvestPOI);
		}

		public NetworkHarvestPOI GetNetworkObject(HarvestPOI harvestPOI)
		{
			if (harvestPOI is BaitHarvestPOI baitHarvestPOI && _baitLookUp.TryGetValue(baitHarvestPOI, out var networkBait))
			{
				return networkBait;
			}
			else if (_lookUp.TryGetValue(harvestPOI, out var networkPOI))
			{
				return networkPOI;
			}
			else
			{
				CFBCore.LogError($"Untracked HarvestPOI {harvestPOI.name}");
				GameObject.DestroyImmediate(networkPOI.gameObject);
				return null;
			}
		}

		public void TrySpawnNetworkBait(BaitHarvestPOI bait)
		{
			try
			{
				if (IsHarvestPOITracked(bait)) return;

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
			RegisterNetworkBaitHarvestPOI(networkBait);
		}
	}
}
