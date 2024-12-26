using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.HarvestPOISync;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteAtrophyAbility : NetworkBehaviour
	{
		public GameObject playerVfxPrefab, harvestVfxPrefab;

		public AudioSource loopAudio;

		private GameObject _playerVfx, _harvestVfx;

		[Command]
		public void Activate(NetworkIdentity harvestID) => OnActivate(harvestID);

		[ClientRpc(includeOwner = false)]
		protected void OnActivate(NetworkIdentity harvestID)
		{
			var harvestPOI = harvestID.GetComponent<NetworkHarvestPOI>().Target;

			_playerVfx = GameObject.Instantiate(playerVfxPrefab, transform.position, Quaternion.identity);
			_harvestVfx = GameObject.Instantiate(harvestVfxPrefab, harvestPOI.transform.position, Quaternion.identity);
			loopAudio.Play();

			harvestPOI.HarvestPOIData.AtrophyStock();
			harvestPOI.OnStockUpdated();
		}

		[Command]
		public void Deactivate() => OnDeactivate();

		[ClientRpc(includeOwner = false)]
		public void OnDeactivate()
		{
			try
			{
				_playerVfx?.GetComponent<SafeParticleDestroyer>()?.Destroy();
				_harvestVfx?.GetComponent<SafeParticleDestroyer>()?.Destroy();
				loopAudio.Stop();
			}
			catch (Exception e)
			{
				CFBCore.LogError(e);
			}
		}
	}
}