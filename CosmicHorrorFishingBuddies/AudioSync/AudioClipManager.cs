using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Util;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CosmicHorrorFishingBuddies.AudioSync
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
		private TeleportAbility _teleportAbility;
		private AtrophyAbility _atrophyAbility;
		private BaitAbility _baitAbility;
		private DeployPotAbility _potAbility;
		private TrawlNetAbility _trawlAbility;
		private BoostAbility _hasteAbility;

		private static AudioClipManager _instance;

		private void OnPlayerLoaded()
		{
			_playerCollisionAudio = GameObject.FindObjectOfType<PlayerCollisionAudio>();
			_lightAbility = GameManager.Instance.PlayerAbilities.abilityMap["lights"] as LightAbility;
			_teleportAbility = GameManager.Instance.PlayerAbilities.abilityMap["manifest"] as TeleportAbility;
			_atrophyAbility = GameManager.Instance.PlayerAbilities.abilityMap["atrophy"] as AtrophyAbility;
			_baitAbility = GameManager.Instance.PlayerAbilities.abilityMap["bait"] as BaitAbility;
			_potAbility = GameManager.Instance.PlayerAbilities.abilityMap["pot"] as DeployPotAbility;
			_trawlAbility = GameManager.Instance.PlayerAbilities.abilityMap["trawl"] as TrawlNetAbility;
			_hasteAbility = GameManager.Instance.PlayerAbilities.abilityMap["haste"] as BoostAbility;
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
			// Ability -> Ability Data
			AudioEnum.MANIFEST => _instance?._teleportAbility?.abilityData?.castSFX,
			AudioEnum.ATROPHY => _instance?._atrophyAbility?.abilityData?.castSFX,
			AudioEnum.BAIT => _instance?._baitAbility?.abilityData?.castSFX,
			AudioEnum.DEPLOY_POT => _instance?._potAbility?.abilityData?.castSFX,
			AudioEnum.TRAWL => _instance?._trawlAbility?.abilityData?.castSFX,
			AudioEnum.HASTE => _instance?._hasteAbility?.abilityData?.castSFX,
			AudioEnum.LIGHT_FLICKER => EventHelper.GetWorldEvent<FlickerLightsWorldEvent>()?.flickerSFX,
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
