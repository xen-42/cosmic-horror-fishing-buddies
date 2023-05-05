using CosmicFishingBuddies.AudioSync;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal class RemoteTeleportAbility : NetworkBehaviour
	{
		public GameObject teleportEffect;
		private NetworkPlayer _networkPlayer;

		public void Awake()
		{
			_networkPlayer = GetComponent<NetworkPlayer>();
		}

		[Command]
		public void SetAbility(bool active) => RpcTriggerAbility(active);

		[ClientRpc(includeOwner = false)]
		private void RpcTriggerAbility(bool active)
		{
			teleportEffect.SetActive(active);
			_networkPlayer.CurrentBoatModelProxy.gameObject.SetActive(!active);
			_networkPlayer.wake.gameObject.SetActive(!active);
		}
	}
}
