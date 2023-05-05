using CosmicFishingBuddies.AudioSync;
using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.TimeSync;
using Mirror;
using System;
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

		public void LightHook(bool prev, bool current)
		{
			if (!isOwned)
			{
				CFBCore.LogInfo($"Remote player light {_lightActive}");

				PlayOneShot(_lightActive ? AudioEnum.LIGHT_ON : AudioEnum.LIGHT_OFF, 0.3f, 1f);
				RefreshLights();
			}
		}

		private void RefreshLights()
		{
			CurrentBoatModelProxy.SetLightStrength(_lightActive ? 4f : 0f);

			foreach (var light in CurrentBoatModelProxy.Lights)
			{
				light.SetActive(_lightActive);
			}
			foreach (var lightBeam in CurrentBoatModelProxy.LightBeams)
			{
				lightBeam.SetActive(_lightActive);
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
		public void SetTimeMode(TimePassageMode mode)
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
		public void SetIsDocked(bool isDocked)
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

		#region	Upgrade tiers
		[SyncVar(hook = nameof(UpgradeTierHook))]
		private int _upgradeTier;

		[Command]
		public void SetUpgradeTier(int upgradeTier) => _upgradeTier = upgradeTier;

		public void UpgradeTierHook(int prev, int current) => RefreshUpgradeTier();

		public void RefreshUpgradeTier()
		{
			if (!isOwned)
			{
				try
				{
					CFBCore.LogInfo($"Player {netId} has ship hull upgrade tier {_upgradeTier}");

					foreach (var boatModel in boatModelProxies)
					{
						boatModel.gameObject.SetActive(false);
					}
					CurrentBoatModelProxy = boatModelProxies[_upgradeTier];
					CurrentBoatModelProxy.gameObject.SetActive(true);

					RefreshLights();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to refresh upgrade tier {e}");
				}
			}
		}
		#endregion

		public static NetworkPlayer LocalPlayer { get; private set; }

		public AudioSource foghornEndSource;
		public AudioSource foghornMidSource;

		public AudioSource oneShotSource;

		public RemotePlayerEngineAudio remotePlayerEngineAudio;
		public RemoteTeleportAbility remoteTeleportAbility;

		public BoatModelProxy[] boatModelProxies;
		public BoatModelProxy CurrentBoatModelProxy { get; private set; }
		public GameObject wake;

		public void Start()
		{
			if (isOwned)
			{
				LocalPlayer = this;

				GameEvents.Instance.OnUpgradesChanged += OnUpgradesChanged;

				// Initial state
				SetUpgradeTier(GetUpgradeTier());
				SetIsDocked(GameManager.Instance.Player.IsDocked);
				SetTimeMode(GameManager.Instance.Time.CurrentTimePassageMode);
			}
			else
			{
				try
				{
					// Fog horns
					var existingFoghorn = GameObject.FindObjectOfType<FoghornAbility>();
					foghornEndSource.clip = existingFoghorn.foghornEndSource.clip;
					foghornMidSource.clip = existingFoghorn.foghornMidSource.clip;

					RefreshUpgradeTier();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to make remote player {e}");
				}
			}

			PlayerManager.Players.Add(this);
		}

		private int GetUpgradeTier() => Math.Clamp(GameManager.Instance.Player._allBoatModelProxies.IndexOf(GameManager.Instance.Player.BoatModelProxy), 0, 3);

		public void OnDestroy()
		{
			PlayerManager.Players.Remove(this);

			if (isOwned)
			{
				GameEvents.Instance.OnUpgradesChanged -= OnUpgradesChanged;
			}
		}

		private void OnUpgradesChanged(UpgradeData upgradeData)
		{
			if (upgradeData is HullUpgradeData)
			{
				SetUpgradeTier(upgradeData.tier);
			}
		}
	}
}
