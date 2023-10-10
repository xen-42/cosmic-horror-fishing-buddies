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

		// Want to do initial value after it is fully set up and OnStartClient is called (can happen before Start)
		private bool _started;
		private bool _startedOnClient;

		public override void OnStartClient()
		{
			// Remote - send the initial state
			_startedOnClient = true;
			if (_started)
			{
				OnToggleRemote(_active);
			}
		}

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
				_started = true;
				if (_startedOnClient)
				{
					OnToggleRemote(_active);
				}
			}
		}

		[Command]
		public override sealed void Toggle(bool active)
		{
			CFBCore.LogInfo($"Command - Player {NetworkPlayer.LocalPlayer?.netId} just toggled {AbilityType?.Name} to {active}");
			_active = active;

			// Hook has to be manually called on the server
			Hook(default, active);
		}

		private void Hook(bool _, bool current)
		{
			CFBCore.LogInfo($"Hook - Player {NetworkPlayer.LocalPlayer?.netId} just toggled {AbilityType?.Name} to {current}");
			if (!_networkPlayer.isLocalPlayer)
			{
				OnToggleRemote(current);
			}
		}

		protected abstract void OnToggleRemote(bool active);
	}
}