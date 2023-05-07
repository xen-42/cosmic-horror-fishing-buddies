using CosmicHorrorFishingBuddies.PlayerSync;
using Mirror;
using System.Linq;

namespace CosmicHorrorFishingBuddies.TimeSync
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

				// Think this is getting called before there are even players in the list, making it fast-forward time
				var canSkipTime = PlayerManager.Players.Count > 0 && (allPlayersSkippingFlag || (anyPlayerSkippingFlag && allPlayersDockedFlag));

				TimePassageModifier = canSkipTime ? 20 : 0.5f;
			}
		}
	}
}
