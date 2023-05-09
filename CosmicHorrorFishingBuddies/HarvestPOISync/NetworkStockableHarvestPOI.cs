using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.HarvestPOISync.Patches;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.HarvestPOISync
{
	internal class NetworkStockableHarvestPOI : NetworkHarvestPOI
	{
		public bool IsCurrentlySpecial => _isCurrentlySpecial;

		[SyncVar] private float _stockCount = Mathf.NegativeInfinity;

		[SyncVar] private bool _isCurrentlySpecial;

		[Command(requiresAuthority = false)]
		public void SetStockCount(float count)
		{
			_stockCount = count;
			RpcSetStockCount(count);
		}

		[ClientRpc]
		private void RpcSetStockCount(float count)
		{
			try
			{
				HarvestPOIPatches.disabled = true;

				var dStock = count - Target.Stock;
				Target.harvestable.AddStock(dStock, false);
				Target.OnStockUpdated();

				HarvestPOIPatches.disabled = false;
			}
			catch (Exception e)
			{
				CFBCore.LogError($"HOW?!?!?! Target: {Target == null} this: {name} target name: {Target.name} - {e}");
			}
		}

		[Command]
		public void SetIsCurrentlySpecial(bool isCurrentlySpecial)
		{
			_isCurrentlySpecial = isCurrentlySpecial;
			RpcSetSpecial(isCurrentlySpecial);
		}

		[ClientRpc]
		private void RpcSetSpecial(bool isSpecial)
		{
			try
			{
				HarvestPOIPatches.disabled = true;

				Target.SetIsCurrentlySpecial(isSpecial);

				HarvestPOIPatches.disabled = false;
			}
			catch (Exception e)
			{
				CFBCore.LogError(e);
			}
		}

		protected override void OnSetTarget()
		{
			if (!NetworkClient.activeHost)
			{
				RpcSetStockCount(_stockCount);
				RpcSetSpecial(_isCurrentlySpecial);
			}
			else
			{
				SetStockCount(Target.Stock);
				SetIsCurrentlySpecial(Target.IsCurrentlySpecial);
			}
		}
	}
}
