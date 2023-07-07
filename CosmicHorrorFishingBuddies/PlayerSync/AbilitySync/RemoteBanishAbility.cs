using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteBanishAbility : RemoteRPCAbility
	{
		public GameObject banishEffect;
		public AudioSource banishAudioSource;

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
