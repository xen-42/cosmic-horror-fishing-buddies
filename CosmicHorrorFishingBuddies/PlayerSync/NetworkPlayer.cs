using CosmicHorrorFishingBuddies.AudioSync;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync;
using CosmicHorrorFishingBuddies.TimeSync;
using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal class NetworkPlayer : NetworkBehaviour
	{
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

		public static NetworkPlayer LocalPlayer { get; private set; }

		public AudioSource oneShotSource;

		public RemotePlayerEngineAudio remotePlayerEngineAudio;
		public RemoteBoatGraphics remotePlayerBoatGraphics;
		public RemoteSteeringAnimator remoteSteeringAnimator;

		// Abilities
		public RemoteLightAbility remoteLightAbility;
		public RemoteTeleportAbility remoteTeleportAbility;
		public RemoteBanishAbility remoteBanishAbility;
		public RemoteAtrophyAbility remoteAtrophyAbility;
		public RemoteFoghornAbility remoteFoghornAbility;
		public RemoteTrawlNetAbility remoteTrawlNetAbility;

		public void Start()
		{
			if (isOwned)
			{
				LocalPlayer = this;

				SetIsDocked(GameManager.Instance.Player.IsDocked);
				SetTimeMode(GameManager.Instance.Time.CurrentTimePassageMode);
			}

			PlayerManager.Players.Add(netIdentity.netId, this);

			// Don't invoke event if the first player isn't set up yet
			if (LocalPlayer != null)
			{
				PlayerManager.PlayerJoined?.Invoke(isOwned, netIdentity.netId);
			}
		}

		public void OnDestroy()
		{
			PlayerManager.Players.Remove(netIdentity.netId);
			PlayerManager.PlayerLeft?.Invoke(isOwned, netIdentity.netId);
		}
	}
}
