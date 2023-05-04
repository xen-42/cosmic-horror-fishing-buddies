using CosmicFishingBuddies.AudioSync;
using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.TimeSync;
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

				boatModelProxy.SetLightStrength(current ? 4f : 0f);
				PlayOneShot(current ? AudioEnum.LIGHT_ON : AudioEnum.LIGHT_OFF, 0.3f, 1f);
				foreach (var light in boatModelProxy.Lights)
				{
					light.SetActive(current);
				}
				foreach (var lightBeam in boatModelProxy.LightBeams)
				{
					lightBeam.SetActive(current);
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
				AudioClipManager.PlayClip(audio, oneShotSource, volume, pitch);
			}
		}
		#endregion

		#region Time mode
		[Command]
		public void CmdSetTimeMode(TimePassageMode mode)
		{
			var prevMode = _timeMode;
			_timeMode = mode;

			if (prevMode != mode)
			{
				TimeSyncManager.Instance.RefreshTimePassageModifier();
			}
		}

		[SyncVar]
		private TimePassageMode _timeMode;

		public TimePassageMode TimeMode => _timeMode;

		[Command]
		public void CmdSetIsDocked(bool isDocked)
		{
			var wasDocked = _isDocked;
			_isDocked = isDocked;

			if (wasDocked != isDocked)
			{
				TimeSyncManager.Instance.RefreshTimePassageModifier();
			}
		}

		[SyncVar]
		private bool _isDocked;

		public bool IsDocked => _isDocked;
		#endregion

		public static NetworkPlayer LocalPlayer { get; private set; }

		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public AudioSource oneShotSource;

		public RemotePlayerEngineAudio remotePlayerEngineAudio;

		public BoatModelProxy boatModelProxy;

		public void Start()
		{
			if (isOwned)
			{
				LocalPlayer = this;

				// Initial state
				CmdSetIsDocked(GameManager.Instance.Player.IsDocked);
				CmdSetTimeMode(GameManager.Instance.Time.CurrentTimePassageMode);
				TimeSyncManager.Instance.RefreshTimePassageModifier();
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

			PlayerManager.Players.Add(this);
		}

		public void OnDestroy()
		{
			PlayerManager.Players.Remove(this);
		}
	}
}
