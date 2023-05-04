﻿using CosmicFishingBuddies.PlayerSync;
using Mirror;
using System.Linq;

namespace CosmicFishingBuddies.TimeSync
{
	internal class TimeSyncManager : NetworkBehaviour
	{
		public static TimeSyncManager Instance { get; private set; }

		[SyncVar]
		public float TimeAndDay;

		[SyncVar]
		public float TimePassageModifier;

		public void Awake()
		{
			Instance = this;
		}

		[Command]
		public void RefreshTimePassageModifier()
		{
			if (isServer)
			{
				// Either all players are skipping time, or at least one player is but all players are docked
				var allPlayersSkippingFlag = true;
				var anyPlayerSkippingFlag = false;
				var allPlayersDockedFlag = true;
				foreach (var player in PlayerManager.Players)
				{
					if (player.TimeMode == TimePassageMode.NONE) allPlayersSkippingFlag = false;
					else anyPlayerSkippingFlag = true;
					if (!player.IsDocked) allPlayersDockedFlag = false;
				}
				var canSkipTime = allPlayersSkippingFlag || (anyPlayerSkippingFlag && allPlayersDockedFlag);

				TimePassageModifier = canSkipTime ? 10 : 0.5f;
			}
		}
	}
}
