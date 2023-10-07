﻿using CosmicHorrorFishingBuddies.Core;
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
		private bool _hasStarted;

		public bool IsActive => _active;

		public virtual void Start()
		{
			if (isOwned)
			{
				Toggle(AbilityHelper.GetAbility(AbilityType).IsActive);
			}
		}

		[Command]
		public override sealed void Toggle(bool active)
		{
			CFBCore.LogInfo($"Command - Player {NetworkPlayer.LocalPlayer.netId} just toggled {AbilityType.Name} to {active}");
			_active = active;
		}

		private void Hook(bool _, bool current)
		{
			CFBCore.LogInfo($"Hook - Player {NetworkPlayer.LocalPlayer.netId} just toggled {AbilityType.Name} to {current}");
			if (!isOwned && _hasStarted)
			{
				OnToggleRemote(current);
			}
		}

		protected abstract void OnToggleRemote(bool active);
	}
}
