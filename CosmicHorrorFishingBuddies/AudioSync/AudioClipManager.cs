using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Util;
using CosmicHorrorFishingBuddies.Util.Attributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CosmicHorrorFishingBuddies.AudioSync
{
	[AddToGameScene]
	internal class AudioClipManager : MonoBehaviour
	{
		private static AudioClipManager _instance;
		private PlayerCollisionAudio _playerCollisionAudio;

		public void Awake()
		{
			_instance = this;
			_playerCollisionAudio = GameObject.FindObjectOfType<PlayerCollisionAudio>();
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
			AudioEnum.LIGHT_ON => AbilityHelper.GetAbility<LightAbility>().onSFX,
			AudioEnum.LIGHT_OFF => AbilityHelper.GetAbility<LightAbility>().offSFX,
			_ => null,
		};

		private static AssetReference GetAssetReference(AudioEnum audio) => audio switch
		{
			AudioEnum.PLAYER_COLLISION => _instance?._playerCollisionAudio?.clipRefs?.PickRandom(),
			// Ability -> Ability Data
			AudioEnum.MANIFEST => AbilityHelper.GetAbility<TeleportAbility>()?.abilityData?.castSFX,
			AudioEnum.ATROPHY => AbilityHelper.GetAbility<AtrophyAbility>()?.abilityData?.castSFX,
			AudioEnum.BAIT => AbilityHelper.GetAbility<BaitAbility>()?.abilityData?.castSFX,
			AudioEnum.DEPLOY_POT => AbilityHelper.GetAbility<DeployPotAbility>()?.abilityData?.castSFX,

			AudioEnum.TRAWL_ACTIVATE => AbilityHelper.GetAbility<TrawlNetAbility>()?.abilityData?.castSFX,
			AudioEnum.TRAWL_END => AbilityHelper.GetAbility<TrawlNetAbility>()?.endSFX,
			AudioEnum.TRAWL_BREAK => AbilityHelper.GetAbility<TrawlNetAbility>()?.breakSFX,

			AudioEnum.HASTE => AbilityHelper.GetAbility<BoostAbility>()?.abilityData?.castSFX,
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