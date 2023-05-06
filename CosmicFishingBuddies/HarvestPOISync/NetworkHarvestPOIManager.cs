using CosmicFishingBuddies.Extensions;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CosmicFishingBuddies.HarvestPOISync
{
	internal class NetworkHarvestPOIManager : MonoBehaviour
	{
		public static NetworkHarvestPOIManager Instance { get; private set; }

		public List<HarvestPOI> SortedHarvestPOIs { get; private set; }
		private Dictionary<HarvestPOI, NetworkHarvestPOI> _lookUp = new();

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
						var networkPOI = CFBNetworkManager.HarvestPOIPrefab.SpawnWithServerAuthority().GetComponent<NetworkHarvestPOI>();
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

		public void RegisterNetworkHarvestPOI(NetworkHarvestPOI networkPOI)
		{
			var harvestPOI = SortedHarvestPOIs[networkPOI.SyncIndex];
			_lookUp[harvestPOI] = networkPOI;
			networkPOI.Target = harvestPOI;
		}

		public NetworkHarvestPOI GetNetworkObject(HarvestPOI harvestPOI)
		{
			if (_lookUp.TryGetValue(harvestPOI, out var networkPOI))
			{
				return networkPOI;
			}
			else
			{
				CFBCore.LogError($"Untracked HarvestPOI {harvestPOI.name}");
				return null;
			}
		}
	}
}
