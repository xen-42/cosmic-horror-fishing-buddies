using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.Abilities
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal class RemoteBanishAbility : NetworkBehaviour
	{
		public GameObject banishEffect;
		public AudioSource banishAudioSource;

		private NetworkPlayer _networkPlayer;

		public void Awake()
		{
			_networkPlayer = GetComponent<NetworkPlayer>();
		}

		[Command]
		public void ToggleAbility(bool active) => RpcTriggerAbility(active);

		[ClientRpc(includeOwner = false)]
		private void RpcTriggerAbility(bool active)
		{
			banishEffect.SetActive(active);
			if (active)
			{
				banishAudioSource.Play();
			}
			else
			{
				banishAudioSource.Stop();
			}
		}
	}
}
