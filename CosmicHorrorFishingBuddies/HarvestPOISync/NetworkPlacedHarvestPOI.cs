using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.HarvestPOISync.Data;
using CosmicHorrorFishingBuddies.PlayerSync;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.HarvestPOISync
{
	internal class NetworkPlacedHarvestPOI : NetworkHarvestPOI
	{
		[Command]
		public void SetCrabPotData(SerializedCrabPotPOIDataWrapper crabPotData, uint senderID)
		{
			RpcSetCrabPotData(crabPotData, senderID);
		}

		[ClientRpc]
		private void RpcSetCrabPotData(SerializedCrabPotPOIDataWrapper crabPotData, uint senderID)
		{
			CFBCore.LogInfo($"Player {senderID} requested {NetworkPlayer.LocalPlayer.netId} to make bait");
			if (NetworkPlayer.LocalPlayer.netIdentity.netId != senderID)
			{
				InitializeCrabPotData(crabPotData);
			}
		}

		private void InitializeCrabPotData(SerializedCrabPotPOIDataWrapper crabPotData)
		{
			try
			{
				var yRotation = crabPotData.yRotation;
				var position = new Vector3(crabPotData.x, 0f, crabPotData.z);
				var gameObject = GameSceneInitializer.Instance.placedPOIPrefab.InstantiateInactive();
				gameObject.transform.parent = GameSceneInitializer.Instance.harvestPoiContainer.transform;
				gameObject.transform.position = position;
				gameObject.transform.eulerAngles = new Vector3(0f, yRotation, 0f);
				gameObject.name = "PlacedHarvestPOI";
				var harvestPOI = gameObject.GetComponent<HarvestPOI>();
				if (harvestPOI)
				{
					var harvestable = crabPotData.ToData();

					harvestPOI.Harvestable = harvestable;
					var cullable = harvestPOI.GetComponent<Cullable>();
					if (cullable)
					{
						GameManager.Instance.CullingBrain.AddCullable(cullable);
					}
				}

				Target = harvestPOI;

				NetworkHarvestPOIManager.Instance.RegisterPlacedHarvestPOI(this);

				gameObject.SetActive(true);

				CFBCore.LogInfo($"Created crab pot on {NetworkPlayer.LocalPlayer.netId}");
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't initialize crab pot data {e}");
			}
		}
	}
}
