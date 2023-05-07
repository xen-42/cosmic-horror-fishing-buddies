using System.Collections;
using System.Collections.Generic;
using CosmicFishingBuddies.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace CosmicFishingBuddies.AudioSync
{
    internal class AudioClipManager : MonoBehaviour
	{
		public void Awake()
		{
			_instance = this;
			CFBCore.Instance.PlayerLoaded.AddListener(OnPlayerLoaded);
		}

		private PlayerCollisionAudio _playerCollisionAudio;
		private LightAbility _lightAbility;

		private static AudioClipManager _instance;

		private void OnPlayerLoaded()
		{
			_playerCollisionAudio = GameObject.FindObjectOfType<PlayerCollisionAudio>();
			_lightAbility = GameManager.Instance.PlayerAbilities.abilityMap["lights"] as LightAbility;
		}

		public static void PlayClip(AudioEnum audio, AudioSource source, float volume, float pitch)
		{
			var loadedClip = GetClip(audio);
			if (loadedClip != null)
			{
				PlayClip(loadedClip, source, volume, pitch);
			}
			else
			{
				var assetReference = GetAssetReference(audio);
				if (assetReference != null)
				{
					Addressables.LoadAssetAsync<AudioClip>(assetReference).Completed += (AsyncOperationHandle<AudioClip> op) =>
					{
						if (op.Status == AsyncOperationStatus.Succeeded)
						{
							PlayClip(op.Result, source, volume, pitch);
						}
						else
						{
							CFBCore.LogError($"Could not load audio async {audio}");
						}
					};
				}
				else
				{
					CFBCore.LogError($"Could not load audio {audio}");
				}
			}
		}

		private static AudioClip GetClip(AudioEnum audio) => audio switch
		{
			AudioEnum.LIGHT_ON => _instance?._lightAbility?.onSFX,
			AudioEnum.LIGHT_OFF => _instance?._lightAbility?.offSFX,
			_ => null,
		};

		private static AssetReference GetAssetReference(AudioEnum audio) => audio switch
		{
			AudioEnum.PLAYER_COLLISION => _instance?._playerCollisionAudio?.clipRefs?.PickRandom(),
			_ => null,
		};

		private static void PlayClip(AudioClip clip, AudioSource source, float volume, float pitch)
		{
			if (clip != null)
			{
				source.pitch = pitch;
				source.PlayOneShot(clip, volume);
			}
		}
	}
}
