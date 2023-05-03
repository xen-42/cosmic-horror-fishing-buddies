using Mirror;

namespace CosmicFishingBuddies.TimeSync
{
	internal class TimeSyncManager : NetworkBehaviour
	{
		public static TimeSyncManager Instance { get; private set; }

		[SyncVar]
		public float timeAndDay;

		public void Awake()
		{
			Instance = this;
		}
	}
}
