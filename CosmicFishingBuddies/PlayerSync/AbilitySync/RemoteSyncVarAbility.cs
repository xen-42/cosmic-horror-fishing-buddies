using Mirror;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal abstract class RemoteSyncVarAbility : RemoteAbility
	{
		[SyncVar(hook = nameof(Hook))]
		private bool _active;

		public bool IsActive => _active;

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
