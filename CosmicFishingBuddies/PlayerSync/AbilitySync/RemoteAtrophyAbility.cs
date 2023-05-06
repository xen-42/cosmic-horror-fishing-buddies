using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteAtrophyAbility : NetworkBehaviour
	{
		public GameObject playerVfxPrefab, harvestVfxPrefab;

		public AudioSource loopAudio;

		private GameObject _playerVfx, _harvestVfx;

		[Command]
		public void Activate(NetworkIdentity harvestID) => OnActivate( harvestID);

		[ClientRpc(includeOwner = false)]
		protected void OnActivate(NetworkIdentity harvestID)
		{
			_playerVfx = GameObject.Instantiate(playerVfxPrefab, transform.position, Quaternion.identity);
			_harvestVfx = GameObject.Instantiate(playerVfxPrefab, harvestID.transform.position, Quaternion.identity);
			loopAudio.Play();
		}

		[Command]
		public void Deactivate() => OnDeactivate();

		[ClientRpc(includeOwner = false)]
		public void OnDeactivate()
		{
			_playerVfx?.GetComponent<SafeParticleDestroyer>()?.Destroy();
			_harvestVfx?.GetComponent<SafeParticleDestroyer>()?.Destroy();
			loopAudio.Stop();
		}
	}
}
