﻿using CosmicFishingBuddies.PlayerSync.AbilitySync.Base;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync.AbilitySync
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
