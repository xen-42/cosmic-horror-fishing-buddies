using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Base;
using CosmicHorrorFishingBuddies.Util;
using System;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync
{
	internal class RemoteFoghornAbility : RemoteSyncVarAbility
	{
		public override Type AbilityType => typeof(FoghornAbility);

		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public override void Start()
		{
			foghornEndSource.clip = AbilityHelper.GetAbility<FoghornAbility>().foghornEndSource.clip;
			foghornMidSource.clip = AbilityHelper.GetAbility<FoghornAbility>().foghornMidSource.clip;
			base.Start();
		}

		protected override void OnToggleRemote(bool active)
		{
			CFBCore.LogInfo($"Remote player foghorn {active}");
			if (active)
			{
				foghornMidSource.volume = 1f;
				foghornMidSource.Play();
			}
			if (!active)
			{
				foghornMidSource.Stop();
				foghornEndSource.volume = 1f;
				foghornEndSource.PlayOneShot(foghornEndSource.clip);
			}
		}
	}
}