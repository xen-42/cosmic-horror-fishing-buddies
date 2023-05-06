using CosmicFishingBuddies.HarvestPOISync.Patches;
using CosmicFishingBuddies.PlayerSync;
using Mirror;
using System;
using UnityEngine;

namespace CosmicFishingBuddies.HarvestPOISync
{
	internal class NetworkHarvestPOI : NetworkBehaviour
	{
		[SyncVar]
		private uint _lastInteractionID = uint.MaxValue;

		public int SyncIndex => _syncIndex;

		public bool IsCurrentlySpecial => _isCurrentlySpecial;

		[SyncVar(hook = nameof(OnIndexHook))]
		private int _syncIndex = -1;

		[SyncVar(hook = nameof(OnStockCountHook))]
		private float _stockCount = Mathf.NegativeInfinity;

		[SyncVar(hook = nameof(OnSetIsCurrentlySpecialHook))]
		private bool _isCurrentlySpecial;

		public HarvestPOI Target { get; set; }

		private bool _isReady;

		public void Start()
		{
			if (!NetworkClient.activeHost)
			{
				_isReady = true;

				UpdateStockFromSyncVar();
				UpdateSpecialFromSyncVar();
			}
			else
			{
				SetIsCurrentlySpecial(Target.IsCurrentlySpecial);
				SetStockCount(Target.Stock, NetworkClient.connection.identity.netId);
			}
		}

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
			_lastInteractionID = from;
			_stockCount = count;

			//CFBCore.LogInfo($"Player {from} has requested {Target.name} stock count be set to {count}");
		}

		/// <summary>
		/// Only host can set this
		/// </summary>
		/// <param name="isCurrentlySpecial"></param>
		public void SetIsCurrentlySpecial(bool isCurrentlySpecial)
		{
			if (NetworkClient.activeHost)
			{
				_isCurrentlySpecial = isCurrentlySpecial;

				//CFBCore.LogInfo($"{Target.name} has been set to special : {isCurrentlySpecial} {Target.IsCurrentlySpecial}");
			}
			else
			{
				CFBCore.LogError($"Client tried to set IsCurrentySpecial on a {nameof(NetworkHarvestPOI)}");
			}
		}

		private void OnSetIsCurrentlySpecialHook(bool _, bool current)
		{
			try
			{
				if (!_isReady) return;

				if (!NetworkClient.activeHost)
				{
					UpdateSpecialFromSyncVar();
				}

				//CFBCore.LogInfo($"{Target.name} for Player {NetworkPlayer.LocalPlayer.netId} has been set to special : {_isCurrentlySpecial}");
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Couldn't set harvest POI special {e}");
			}
		}

		private void OnIndexHook(int _, int ind)
		{
			NetworkHarvestPOIManager.Instance.RegisterNetworkHarvestPOI(this);
		}

		private void OnStockCountHook(float _, float count)
		{
			if (!_isReady) return;

			var localID = NetworkClient.connection.identity.netId;

			if (localID != _lastInteractionID)
			{
				UpdateStockFromSyncVar();
			}

			//CFBCore.LogInfo($"Player {_lastInteractionID} has set {Target.name} stock count for Player {localID} to {count}");
		}

		private void UpdateSpecialFromSyncVar()
		{
			HarvestPOIPatches.disabled = true;

			Target.SetIsCurrentlySpecial(_isCurrentlySpecial);

			HarvestPOIPatches.disabled = false;
		}

		private void UpdateStockFromSyncVar()
		{
			HarvestPOIPatches.disabled = true;

			var dStock = _stockCount - Target.Stock;
			Target.harvestable.AddStock(dStock, false);
			Target.OnStockUpdated();

			HarvestPOIPatches.disabled = false;
		}
	}
}
