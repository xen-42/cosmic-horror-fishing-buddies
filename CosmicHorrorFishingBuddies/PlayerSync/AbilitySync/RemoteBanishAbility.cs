using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
    internal class RemoteBanishAbility : RemoteRPCAbility
	{
		public GameObject banishEffect;
		public AudioSource banishAudioSource;

		public override Type AbilityType => typeof(AtrophyAbility);

		[ClientRpc(includeOwner = false)]
		protected override void OnTriggerAbility(bool active)
		{
			banishEffect.SetActive(active);
			if (active)
			{
				banishAudioSource.Play();
			}
			else
			{
				banishAudioSource.Stop();
			}
		}
	}
}
