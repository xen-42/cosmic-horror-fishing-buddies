using CosmicFishingBuddies.AudioSync;
using CosmicFishingBuddies.Extensions;
using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync
{
	internal class NetworkPlayer : NetworkBehaviour
	{
		#region Foghorn
		[SyncVar(hook = nameof(FoghornHook))]
		private bool _fogHornActive;

		[Command]
		public void SetFogHornActive(bool active) => _fogHornActive = active;

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
		#endregion

		#region Light
		[SyncVar(hook = nameof(LightHook))]
		private bool _lightActive;

		[Command]
		public void SetLightActive(bool active) => _lightActive = active;

		public void LightHook(bool _, bool current)
		{
			if (!isOwned)
			{
				CFBCore.LogInfo($"Remote player foghorn {current}");
				if (current)
				{
					boatModelProxy.SetLightStrength(5f);
					PlayOneShot(AudioEnum.LIGHT_ON, 0.3f, 1f);
				}
				else
				{
					boatModelProxy.SetLightStrength(0f);
					PlayOneShot(AudioEnum.LIGHT_OFF, 0.3f, 1f);
				}
			}
		}
		#endregion

		#region OneShot
		[Command]
		public void CmdPlayOneShot(AudioEnum audio, float volume, float pitch) => RpcPlayOneShot(audio, volume, pitch);

		[ClientRpc(includeOwner = false)]
		private void RpcPlayOneShot(AudioEnum audio, float volume, float pitch) => PlayOneShot(audio, volume, pitch);

		private void PlayOneShot(AudioEnum audio, float volume, float pitch)
		{
			if (!isOwned)
			{
				var clip = AudioClipManager.GetClip(audio);
				if (clip != null)
				{
					oneShotSource.pitch = pitch;
					oneShotSource.PlayOneShot(clip, volume);
				}
			}
		}
		#endregion

		public static NetworkPlayer LocalPlayer { get; private set; }

		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public AudioSource oneShotSource;

		public BoatModelProxy boatModelProxy;

		public void Start()
		{
			if (isOwned)
			{
				LocalPlayer = this;
			}
			else
			{
				// Fog horns
				var existingFoghorn = GameObject.FindObjectOfType<FoghornAbility>();
				foghornEndSource.clip = existingFoghorn.foghornEndSource.clip;
				foghornMidSource.clip = existingFoghorn.foghornMidSource.clip;

				// Lights
				boatModelProxy.SetLightStrength(0f);
			}
		}


	}
}
