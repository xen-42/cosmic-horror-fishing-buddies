﻿using CosmicFishingBuddies.Extensions;
using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync
{
	internal class NetworkPlayer : NetworkBehaviour
	{
		[SyncVar(hook = nameof(FoghornHook))]
		public bool fogHornActive;

		public static NetworkPlayer LocalPlayer { get; private set; }

		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public void Start()
		{
			if (isOwned)
			{
				LocalPlayer = this;
			}
			else
			{
				var existingFoghorn = GameObject.FindObjectOfType<FoghornAbility>();
				foghornEndSource.clip = existingFoghorn.GetValue<AudioSource>("foghornEndSource").clip;
				foghornMidSource.clip = existingFoghorn.GetValue<AudioSource>("foghornMidSource").clip;
			}
		}

		public void FoghornHook(bool prev, bool current)
		{
			if (!isOwned)
			{
				CFBCore.LogInfo($"Remote player foghorn {current}");
				if (!prev && current)
				{
					foghornMidSource.volume = 3.0f;
					foghornMidSource.Play();
				}
				if (prev && !current)
				{
					foghornMidSource.Stop();
					foghornEndSource.volume = 3.0f;
					foghornEndSource.PlayOneShot(foghornEndSource.clip);
				}
			}

		}
	}
}