using CosmicHorrorFishingBuddies.Core;
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
			// Local - set our initial state
			if (_networkPlayer.isLocalPlayer)
			{
				Toggle(AbilityHelper.GetAbility(AbilityType).IsActive);
			}
			// Remote - send the initial state
			else
			{
				OnToggleRemote(_active);
			}
		}

		[Command]
		public override sealed void Toggle(bool active)
		{
			_active = active;

			// Hook has to be manually called on the server
			Hook(default, active);
		}

		private void Hook(bool _, bool current)
		{
			if (!_networkPlayer.isLocalPlayer)
			{
				OnToggleRemote(current);
			}
		}

		protected abstract void OnToggleRemote(bool active);
	}
}