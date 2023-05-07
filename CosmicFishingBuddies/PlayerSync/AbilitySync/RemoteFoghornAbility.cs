using CosmicFishingBuddies.Core;
using CosmicFishingBuddies.PlayerSync.AbilitySync.Base;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync
{
    internal class RemoteFoghornAbility : RemoteSyncVarAbility
	{
		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public void Start()
		{
			var existingFoghorn = FindObjectOfType<FoghornAbility>();
			foghornEndSource.clip = existingFoghorn.foghornEndSource.clip;
			foghornMidSource.clip = existingFoghorn.foghornMidSource.clip;
		}

		protected override void OnToggleRemote(bool active)
		{
			CFBCore.LogInfo($"Remote player foghorn {active}");
			if (active)
			{
				foghornMidSource.volume = 3.0f;
				foghornMidSource.Play();
			}
			if (!active)
			{
				foghornMidSource.Stop();
				foghornEndSource.volume = 3.0f;
				foghornEndSource.PlayOneShot(foghornEndSource.clip);
			}
		}
	}
}
