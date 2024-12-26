using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.HarvestPOISync.Patches;
using Mirror;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.HarvestPOISync
{
	internal class NetworkHarvestPOI : NetworkBehaviour
	{
		public HarvestPOI Target
		{
			get
			{
				return _target;
			}
			set
			{
				_target = value;
				try
				{
					OnSetTarget();
				}
				catch (Exception e)
				{
					CFBCore.LogError(e);
				}
			}
		}

		protected virtual void OnSetTarget()
		{

		}

		private HarvestPOI _target;

		// When the network behaviour for a HarvestPOI is destroyed, destroy it too
		public override void OnStopClient()
		{
			base.OnStopClient();

			if (Target?.gameObject != null)
			{
				GameObject.Destroy(Target.gameObject);
			}
		}

		/// <summary>
		/// Allow any player to remove a HarvestPOI (harvesting a crab pot or bait)
		/// </summary>
		[Command(requiresAuthority = false)]
		public void DestroyHarvestPOI() => NetworkServer.Destroy(gameObject);
	}
}
