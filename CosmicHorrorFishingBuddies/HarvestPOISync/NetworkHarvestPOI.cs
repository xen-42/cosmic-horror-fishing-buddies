using CosmicHorrorFishingBuddies.Core;
using Mirror;
using System;

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


	}
}
