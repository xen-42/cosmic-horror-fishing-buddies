using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteAbility<T> : NetworkBehaviour where T : Ability
	{
		protected NetworkPlayer _networkPlayer;

		public virtual void Awake()
		{
			_networkPlayer = GetComponent<NetworkPlayer>();
		}

		[Command]
		public virtual void Toggle(bool active) { }
	}
}
