using CosmicHorrorFishingBuddies.Util;
using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteSyncVarAbility<T> : RemoteAbility<T> where T : Ability
	{
		[SyncVar(hook = nameof(Hook))]
		private bool _active;

		public bool IsActive => _active;

		public virtual void Start()
		{
			if (isOwned)
			{
				Toggle(AbilityHelper.GetAbility<T>().IsActive);
			}
		}

		[Command]
		public override sealed void Toggle(bool active) => _active = active;

		private void Hook(bool _, bool current)
		{
			if (!isOwned)
			{
				OnToggleRemote(current);
			}
		}

		protected abstract void OnToggleRemote(bool active);
	}
}
