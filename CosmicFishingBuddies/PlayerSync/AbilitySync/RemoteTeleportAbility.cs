using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteTeleportAbility : RemoteRPCAbility
	{
		public GameObject teleportEffect;

		[ClientRpc(includeOwner = false)]
		protected override void OnTriggerAbility(bool active)
		{
			teleportEffect.SetActive(active);
			_networkPlayer.CurrentBoatModelProxy.gameObject.SetActive(!active);
			_networkPlayer.wake.gameObject.SetActive(!active);
		}
	}
}
