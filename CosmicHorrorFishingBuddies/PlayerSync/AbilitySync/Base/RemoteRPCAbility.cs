using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteRPCAbility<T> : RemoteAbility<T> where T : Ability
	{
		[Command]
		public override sealed void Toggle(bool active) => OnTriggerAbility(active);

		[ClientRpc(includeOwner = false)]
		protected virtual void OnTriggerAbility(bool active) { }
	}
}
