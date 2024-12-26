using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal class RemotePlayerEngineAudio : NetworkBehaviour
	{
		public float minPitch = 0.9f;
		public float maxPitch = 1.3f;

		public float minVolume = 0.1f;
		public float maxVolume = 1f;

		public AudioSource engineSource;

		[SyncVar]
		private float volume;

		[SyncVar]
		private float pitch;

		[Command]
		public void UpdateEngineSound(float v, float p)
		{
			volume = v;
			pitch = p;
		}

		public void Start()
		{
			if (!isOwned)
			{
				engineSource.clip = GameManager.Instance.Player.playerEngineAudio.audioSource.clip;
				engineSource.Play();
			}
		}

		public void Update()
		{
			if (!isOwned)
			{
				engineSource.volume = volume;
				engineSource.pitch = pitch;
			}
		}
	}
}
