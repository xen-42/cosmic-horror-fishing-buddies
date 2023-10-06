using CosmicHorrorFishingBuddies.Util;
using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteSyncVarAbility : RemoteAbility
	{
		[SyncVar(hook = nameof(Hook))]
		private bool _active;

		public bool IsActive => _active;

		public virtual void Start()
		{
			if (isOwned)
			{
				Toggle(AbilityHelper.GetAbility(AbilityType).IsActive);
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
