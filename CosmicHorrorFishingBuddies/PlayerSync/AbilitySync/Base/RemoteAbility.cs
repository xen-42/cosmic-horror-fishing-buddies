using CosmicHorrorFishingBuddies.Util;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteAbility : NetworkBehaviour
	{
		protected NetworkPlayer _networkPlayer;

		public virtual Type AbilityType { get; }

		public virtual void Awake()
		{
			_networkPlayer = GetComponent<NetworkPlayer>();
		}

		[Command]
		public virtual void Toggle(bool active) { }
	}
}