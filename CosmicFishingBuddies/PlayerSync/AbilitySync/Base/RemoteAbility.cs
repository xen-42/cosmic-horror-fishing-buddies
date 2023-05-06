using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync.Base
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteAbility : NetworkBehaviour
	{
		protected NetworkPlayer _networkPlayer;

		public void Awake()
		{
			_networkPlayer = GetComponent<NetworkPlayer>();
		}

		[Command]
		public virtual void Toggle(bool active) { }
	}
}
