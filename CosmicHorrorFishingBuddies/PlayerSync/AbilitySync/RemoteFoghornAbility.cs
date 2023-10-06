using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using CosmicHorrorFishingBuddies.Util;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
    internal class RemoteFoghornAbility : RemoteSyncVarAbility
	{
		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public override Type AbilityType => typeof(FoghornAbility);

		public override void Start()
		{
			base.Start();

			foghornEndSource.clip = AbilityHelper.GetAbility<FoghornAbility>().foghornEndSource.clip;
			foghornMidSource.clip = AbilityHelper.GetAbility<FoghornAbility>().foghornMidSource.clip;
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
