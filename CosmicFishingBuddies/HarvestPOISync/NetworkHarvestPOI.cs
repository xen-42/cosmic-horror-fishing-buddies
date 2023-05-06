using CosmicFishingBuddies.HarvestPOISync.Patches;
using CosmicFishingBuddies.PlayerSync;
using Mirror;

namespace CosmicFishingBuddies.HarvestPOISync
{
	internal class NetworkHarvestPOI : NetworkBehaviour
	{
		[SyncVar]
		private uint _lastInteractionID = uint.MaxValue;

		public int SyncIndex => _syncIndex;

		[SyncVar(hook = nameof(OnIndexHook))]
		private int _syncIndex = -1;

		[SyncVar(hook = nameof(OnStockCountHook))]
		private float _stockCount;

		public HarvestPOI Target { get; set; }

		public void SetSyncIndex(int ind)
		{
			if (NetworkClient.activeHost)
			{
				_syncIndex = ind;
			}
			else
			{
				CFBCore.LogError($"Client tried to create {nameof(NetworkHarvestPOI)}");
			}
		}

		[Command(requiresAuthority = false)]
		public void SetStockCount(float count, uint from)
		{
			CFBCore.LogInfo($"Player {from} has requested {Target.name} stock count be set to {count}");

			_lastInteractionID = from;
			_stockCount = count;
		}

		private void OnIndexHook(int _, int ind)
		{
			NetworkHarvestPOIManager.Instance.RegisterNetworkHarvestPOI(this);
		}

		private void OnStockCountHook(float _, float count)
		{
			CFBCore.LogInfo($"Player {_lastInteractionID} has set {Target.name} stock count for Player {NetworkPlayer.LocalPlayer.netId} to {count}");

			if (NetworkPlayer.LocalPlayer.netId != _lastInteractionID)
			{
				HarvestPOIPatches.disabled = true;

				var dStock = count - Target.Stock;
				Target.harvestable.AddStock(dStock, false);
				Target.OnStockUpdated();

				HarvestPOIPatches.disabled = false;
			}
		}
	}
}
