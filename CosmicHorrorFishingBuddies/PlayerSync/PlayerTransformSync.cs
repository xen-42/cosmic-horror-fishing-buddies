using CosmicHorrorFishingBuddies.BaseSync;
using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal class PlayerTransformSync : TransformSync
	{
		public static PlayerTransformSync LocalInstance { get; private set; }

		public static GameObject PlayerPrefab { get; private set; }

		private static Transform CopyDetail(GameObject parent, Transform detail)
		{
			var newDetail = detail.gameObject.InstantiateInactive();
			newDetail.name = detail.name;
			newDetail.transform.parent = parent.transform;
			newDetail.transform.localPosition = Vector3.zero;
			newDetail.transform.localRotation = Quaternion.identity;
			newDetail.gameObject.SetActive(true);
			return newDetail.transform;
		}

		private static void CreatePrefab()
		{
			try
			{
				PlayerPrefab = new GameObject("PlayerPrefab");

				var boat1 = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat1"));
				var boat2 = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat2"));
				var boat3 = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat3"));
				var boat4 = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat4"));

				// Attached rigidbodies are really weird. Have to set up some networkrigidbody stuff in the future
				foreach (var rigidBody in PlayerPrefab.gameObject.GetComponentsInChildren<Rigidbody>(true).Select(x => x.gameObject))
				{
					GameObject.DestroyImmediate(rigidBody.GetComponent<Joint>());
					GameObject.DestroyImmediate(rigidBody.GetComponent<RigidBodyVelocityResetter>());
					GameObject.DestroyImmediate(rigidBody.GetComponent<Rigidbody>());
				}

				var wake = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("BoatTrailParticles"));

				var teleportEffect = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Abilities/TeleportAbility/TeleportEffect"));
				teleportEffect.gameObject.SetActive(false);

				var banishAbility = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Abilities/BanishAbility"));
				GameObject.DestroyImmediate(banishAbility.GetComponent<BanishAbility>());
				var banishAudio = banishAbility.GetComponentInChildren<AudioSource>();
				banishAudio.spatialBlend = 1;
				banishAudio.maxDistance = 20;
				banishAudio.minDistance = 5;

				// SmokeColumn shares a material between all players and gets weird because of it
				foreach (var smokeColumn in PlayerPrefab.GetComponentsInChildren<SmokeColumn>(true))
				{
					var mat = new Material(smokeColumn.smokeMaterial);
					smokeColumn.line.material = mat;
					smokeColumn.smokeMaterial = mat;
				}

				PlayerPrefab.SetActive(false);
				PlayerPrefab.DontDestroyOnLoad();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Failed to make player prefab - multiplayer will not function {e}");
			}
		}

		protected override Transform InitLocalTransform()
		{
			CFBCore.LogInfo($"Creating local {nameof(PlayerTransformSync)}");

			LocalInstance = this;

			return transform;
		}

		protected override Transform InitRemoteTransform()
		{
			CFBCore.LogInfo($"Creating remote {nameof(PlayerTransformSync)}");

			if (PlayerPrefab == null) CreatePrefab();
			var remotePlayer = Instantiate(PlayerPrefab).transform;
			remotePlayer.gameObject.SetActive(true);
			remotePlayer.name = "RemotePlayer";
			remotePlayer.transform.parent = transform;
			remotePlayer.transform.localPosition = Vector3.zero;
			remotePlayer.transform.localRotation = Quaternion.identity;

			var networkPlayer = GetComponent<NetworkPlayer>();

			networkPlayer.remotePlayerBoatGraphics.boatSubModelTogglers = remotePlayer.GetComponentsInChildren<BoatSubModelToggler>();
			networkPlayer.remotePlayerBoatGraphics.boatModelProxies = remotePlayer.GetComponentsInChildren<BoatModelProxy>();
			networkPlayer.remotePlayerBoatGraphics.wake = remotePlayer.Find("BoatTrailParticles").gameObject;

			networkPlayer.remoteTeleportAbility.teleportEffect = remotePlayer.Find("TeleportEffect").gameObject;

			networkPlayer.remoteBanishAbility.banishEffect = remotePlayer.Find("BanishAbility/BanishEffect").gameObject;
			networkPlayer.remoteBanishAbility.banishAudioSource = remotePlayer.Find("BanishAbility/BanishLoopSFX").GetComponent<AudioSource>();

			var atrophy = GameManager.Instance.PlayerAbilities.abilityMap["atrophy"] as AtrophyAbility;
			networkPlayer.remoteAtrophyAbility.playerVfxPrefab = atrophy.playerVfxPrefab;
			networkPlayer.remoteAtrophyAbility.harvestVfxPrefab = atrophy.spotVfxPrefab;
			networkPlayer.remoteAtrophyAbility.loopAudio = remotePlayer.gameObject.AddComponent<AudioSource>();
			networkPlayer.remoteAtrophyAbility.loopAudio.spatialBlend = 1;
			networkPlayer.remoteAtrophyAbility.loopAudio.maxDistance = 20;
			networkPlayer.remoteAtrophyAbility.loopAudio.minDistance = 5;
			networkPlayer.remoteAtrophyAbility.loopAudio.clip = atrophy.loopAudioSource.clip;

			return transform;
		}

		private void Update()
		{
			if (isOwned)
			{
				transform.position = GameManager.Instance.Player.transform.position;
				transform.rotation = GameManager.Instance.Player.transform.rotation;
			}
		}
	}
}
