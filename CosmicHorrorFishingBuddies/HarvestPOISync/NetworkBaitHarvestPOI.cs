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
	internal class NetworkBaitHarvestPOI : NetworkStockableHarvestPOI
	{
		[SyncVar]
		private ZoneEnum _zone;

		[SyncVar]
		private Vector3 _position;

		[SyncVar]
		private int _numFish;

		[SyncVar]
		private float _yRot;

		[Command]
		public void SetBaitData(ZoneEnum zone, Vector3 position, float yRot, int numFish, uint senderID)
		{
			_zone = zone;
			_position = position;
			_yRot = yRot;
			_numFish = numFish;
			RpcSetBaitData(zone, position, yRot, numFish, senderID);
		}

		[ClientRpc]
		private void RpcSetBaitData(ZoneEnum zone, Vector3 position, float yRot, int numFish, uint senderID)
		{
			CFBCore.LogInfo($"Player {senderID} requested {NetworkPlayer.LocalPlayer.netId} to make bait");
			if (NetworkPlayer.LocalPlayer.netIdentity.netId != senderID)
			{
				InitializeBaitData(zone, position, yRot, numFish);
			}
		}

		private void InitializeBaitData(ZoneEnum zone, Vector3 position, float yRot, int numFish)
		{
			try
			{
				// Largely copy pasted from BaitAbility
				var baitPOIDataModel = new BaitPOIDataModel();
				baitPOIDataModel.doesRestock = false;
				var fishList = (from i in GameManager.Instance.ItemManager.GetSpatialItemDataBySubtype(ItemSubtype.FISH).OfType<FishItemData>()
					where !i.IsAberration && i.CanAppearInBaitBalls && i.canBeCaughtByRod && i.zonesFoundIn.HasFlag(zone)
					&& GameManager.Instance.PlayerStats.HarvestableTypes.Contains(i.harvestableType)
					select i).Shuffle().ToList();

				var range = fishList.GetRange(0, Mathf.Min(fishList.Count, GameManager.Instance.GameConfigData.NumFishSpeciesInBaitBall));
				Stack<HarvestableItemData> harvestableItemDataStack = new();
				for (int index = 0; index < numFish; ++index)
				{
					harvestableItemDataStack.Push(range[index % range.Count]);
				}
				baitPOIDataModel.itemStock = harvestableItemDataStack;
				baitPOIDataModel.startStock = harvestableItemDataStack.Count;
				baitPOIDataModel.maxStock = baitPOIDataModel.startStock;
				baitPOIDataModel.usesTimeSpecificStock = false;

				var baitAbility = GameManager.Instance.PlayerAbilities.abilityMap["bait"] as BaitAbility;
				var gameObject = baitAbility.baitPOIPrefab.InstantiateInactive();
				gameObject.transform.parent = GameSceneInitializer.Instance.HarvestPoiContainer.transform;
				gameObject.transform.position = position;
				gameObject.transform.eulerAngles = new Vector3(0f, yRot, 0f);
				var harvestPOI = gameObject.GetComponent<HarvestPOI>();
				if (harvestPOI)
				{
					harvestPOI.Harvestable = baitPOIDataModel;
					harvestPOI.HarvestPOIData = baitPOIDataModel;
					var cullable = harvestPOI.GetComponent<Cullable>();
					if (cullable)
					{
						GameManager.Instance.CullingBrain.AddCullable(cullable);
					}
				}

				Target = harvestPOI;

				NetworkHarvestPOIManager.Instance.RegisterNetworkBaitHarvestPOI(this);

				gameObject.SetActive(true);

				CFBCore.LogInfo($"Created bait on {NetworkPlayer.LocalPlayer.netId}");
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't initialize bait data {e}");
			}
		}
	}
}
