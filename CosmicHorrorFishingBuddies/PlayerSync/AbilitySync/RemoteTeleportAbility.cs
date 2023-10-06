using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
    internal class RemoteTeleportAbility : RemoteRPCAbility
	{
		public override Type AbilityType => typeof(TeleportAbility);

		public GameObject teleportEffect;

		[ClientRpc(includeOwner = false)]
		protected override void OnTriggerAbility(bool active)
		{
			teleportEffect.SetActive(active);
			_networkPlayer.remotePlayerBoatGraphics.CurrentBoatModelProxy.gameObject.SetActive(!active);
			_networkPlayer.remotePlayerBoatGraphics.wake.gameObject.SetActive(!active);
		}
	}
}
