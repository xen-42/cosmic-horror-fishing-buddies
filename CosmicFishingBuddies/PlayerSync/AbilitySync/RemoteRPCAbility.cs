using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteRPCAbility : RemoteAbility
	{
		[Command]
		public override sealed void Toggle(bool active) => OnTriggerAbility(active);

		[ClientRpc(includeOwner = false)]
		protected virtual void OnTriggerAbility(bool active) { }
	}
}
