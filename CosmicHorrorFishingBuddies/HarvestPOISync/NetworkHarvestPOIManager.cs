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
	internal class NetworkHarvestPOIManager : MonoBehaviour
	{
		public static NetworkHarvestPOIManager Instance { get; private set; }

		public List<HarvestPOI> SortedHarvestPOIs { get; private set; }
		private Dictionary<HarvestPOI, NetworkHarvestPOI> _lookUp = new();

		private Dictionary<BaitHarvestPOI, NetworkBaitHarvestPOI> _baitLookUp = new();
		private Dictionary<PlacedHarvestPOI, NetworkPlacedHarvestPOI> _crabLookUp = new();

		public void Start()
		{
			try
			{
				Instance = this;

				SortedHarvestPOIs = GameManager.Instance.HarvestPOIManager.allHarvestPOIs;
				// Since DREDGE 1.4.0 the POIs can be null
				// Cant use null coalesecescnencene because its unity objects
				SortedHarvestPOIs.Sort((a, b) => (a == null ? string.Empty : a.name).CompareTo(b == null ? string.Empty : b.name));

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

		public void RegisterPlacedHarvestPOI(NetworkPlacedHarvestPOI networkPOI)
		{
			_crabLookUp[networkPOI.Target as PlacedHarvestPOI] = networkPOI;
		}

		public void DestroyNetworkBait(BaitHarvestPOI bait)
		{
			if (_baitLookUp.TryGetValue(bait, out var networkBait))
			{
				_baitLookUp.Remove(bait);
				networkBait.DestroyHarvestPOI();
			}
			else
			{
				CFBCore.LogError($"Failed to find network object for {bait.name}");
			}
		}

		public void DestroyCrabPot(PlacedHarvestPOI crabPot)
		{
			if (_crabLookUp.TryGetValue(crabPot, out var networkCrabPot))
			{
				_crabLookUp.Remove(crabPot);
				networkCrabPot.DestroyHarvestPOI();
			}
			else
			{
				CFBCore.LogError($"Failed to find network object for {crabPot.name}");
			}
		}

		public bool IsHarvestPOITracked(HarvestPOI harvestPOI)
		{
			return (harvestPOI is BaitHarvestPOI baitHarvestPOI && _baitLookUp.ContainsKey(baitHarvestPOI)) 
				|| (harvestPOI is PlacedHarvestPOI placedHarvestPOI && _crabLookUp.ContainsKey(placedHarvestPOI))
				|| _lookUp.ContainsKey(harvestPOI);
		}

		public NetworkHarvestPOI GetNetworkObject(HarvestPOI harvestPOI)
		{
			if (harvestPOI is BaitHarvestPOI baitHarvestPOI && _baitLookUp.TryGetValue(baitHarvestPOI, out var networkBait))
			{
				return networkBait;
			}
			else if (harvestPOI is PlacedHarvestPOI placedHarvestPOI && _crabLookUp.TryGetValue(placedHarvestPOI, out var networkCrabPot))
			{
				return networkCrabPot;
			}
			else if (_lookUp.TryGetValue(harvestPOI, out var networkPOI))
			{
				return networkPOI;
			}
			else
			{
				// CFBCore.LogError($"Untracked HarvestPOI {harvestPOI?.name} on {NetworkPlayer.LocalPlayer?.netId}");
				return null;
			}
		}
	}
}
