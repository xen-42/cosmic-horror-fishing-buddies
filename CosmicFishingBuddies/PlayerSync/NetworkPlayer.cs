using CosmicFishingBuddies.AudioSync;
using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.PlayerSync.AbilitySync;
using CosmicFishingBuddies.TimeSync;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace CosmicFishingBuddies.PlayerSync
{
	internal class NetworkPlayer : NetworkBehaviour
	{
		#region OneShot
		[Command]
		public void CmdPlayOneShot(AudioEnum audio, float volume, float pitch) => RpcPlayOneShot(audio, volume, pitch);

		[ClientRpc(includeOwner = false)]
		private void RpcPlayOneShot(AudioEnum audio, float volume, float pitch) => RemotePlayOneShot(audio, volume, pitch);

		public void RemotePlayOneShot(AudioEnum audio, float volume, float pitch)
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

		public UnityEvent RefreshBoatModel = new();

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

					RefreshBoatModel?.Invoke();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to refresh upgrade tier {e}");
				}
			}
		}
		#endregion

		public static NetworkPlayer LocalPlayer { get; private set; }

		public AudioSource oneShotSource;

		public RemotePlayerEngineAudio remotePlayerEngineAudio;

		// Abilities
		public RemoteLightAbility remoteLightAbility;
		public RemoteTeleportAbility remoteTeleportAbility;
		public RemoteBanishAbility remoteBanishAbility;
		public RemoteFoghornAbility remoteFoghornAbility;

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
					RefreshUpgradeTier();
				}
				catch (Exception e)
				{
					CFBCore.LogError($"Failed to make remote player {e}");
				}
			}

			PlayerManager.Players.Add(this);
			if (!isOwned) PlayerManager.RemotePlayerJoined?.Invoke();
		}

		private int GetUpgradeTier() => Math.Clamp(GameManager.Instance.Player._allBoatModelProxies.IndexOf(GameManager.Instance.Player.BoatModelProxy), 0, 3);

		public void OnDestroy()
		{
			PlayerManager.Players.Remove(this);
			if (!isOwned) PlayerManager.RemotePlayerLeft?.Invoke();

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
