using CosmicFishingBuddies.Extensions;
using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync
{
	internal class NetworkPlayer : NetworkBehaviour
	{
		[SyncVar(hook = nameof(FoghornHook))]
		private bool _fogHornActive;

		[Command]
		public void SetFogHornActive(bool active)
		{
			_fogHornActive = active;
		}

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
				foghornEndSource.clip = existingFoghorn.foghornEndSource.clip;
				foghornMidSource.clip = existingFoghorn.foghornMidSource.clip;
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
