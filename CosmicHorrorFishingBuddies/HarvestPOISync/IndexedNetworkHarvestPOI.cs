using CosmicHorrorFishingBuddies.Core;
using Mirror;

namespace CosmicHorrorFishingBuddies.HarvestPOISync
{
	internal class IndexedNetworkHarvestPOI : NetworkStockableHarvestPOI
	{
		[SyncVar(hook = nameof(OnIndexHook))]
		private int _syncIndex = -1;

		public int SyncIndex => _syncIndex;

		private void OnIndexHook(int prev, int current) => NetworkHarvestPOIManager.Instance.RegisterIndexedNetworkHarvestPOI(this);

		public void SetSyncIndex(int ind)
		{
			if (NetworkClient.activeHost)
			{
				_syncIndex = ind;
			}
			else
			{
				CFBCore.LogError($"Client tried to create {GetType().Name}");
			}
		}
	}
}
