using CosmicHorrorFishingBuddies.BaseSync;
using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.Util;
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

			return newDetail.transform;
		}

		private static Transform CopyBoat(GameObject parent, Transform boat)
		{
			var boatDetail = GameObject.Instantiate(boat.gameObject);

			// Doesn't work because we have no working rigidbodies also messes with the local player's net
			// This component resets your net OnDisable, so we can't InstantiateInactive off the player without breaking it
			foreach (var trawlResetter in boatDetail.GetComponentsInChildren<TrawlResetter>(true))
			{
				GameObject.DestroyImmediate(trawlResetter);
			}

			// Now it can be inactive
			boatDetail.SetActive(false);

			boatDetail.name = boat.name;
			boatDetail.transform.parent = parent.transform;
			boatDetail.transform.localPosition = Vector3.zero;
			boatDetail.transform.localRotation = Quaternion.identity;

			return boatDetail.transform;
		}

		private static void CreatePrefab()
		{
			try
			{
				PlayerPrefab = new GameObject("PlayerPrefab");

				PlayerPrefab.SetActive(false);

				CopyBoat(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat1"));
				CopyBoat(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat2"));
				CopyBoat(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat3"));
				CopyBoat(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat4"));
				CopyBoat(PlayerPrefab, GameManager.Instance.Player.transform.Find("Boat5"));

				// Attached rigidbodies are really weird. Have to set up some networkrigidbody stuff in the future
				foreach (var rigidBody in PlayerPrefab.gameObject.GetComponentsInChildren<Rigidbody>(true).Select(x => x.gameObject))
				{
					GameObject.DestroyImmediate(rigidBody.GetComponent<Joint>());
					GameObject.DestroyImmediate(rigidBody.GetComponent<RigidBodyVelocityResetter>());
					GameObject.DestroyImmediate(rigidBody.GetComponent<Rigidbody>());
				}

				CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("BoatTrailParticles"));

				var teleportEffect = CopyDetail(PlayerPrefab, GameManager.Instance.Player.GetComponentInChildren<TeleportAbility>(true).effect.transform);
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

				// Shares animator for TrawlNet, maybe other things
				foreach (var animator in PlayerPrefab.GetComponentsInChildren<Animator>(true))
				{
					animator.runtimeAnimatorController = Instantiate(animator.runtimeAnimatorController);
				}

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

			name = $"LocalPlayerRoot ({netId})";

			// Might as well make the prefab right away
			if (PlayerPrefab == null) CreatePrefab();

			LocalInstance = this;

			return transform;
		}

		protected override Transform InitRemoteTransform()
		{
			CFBCore.LogInfo($"Creating remote {nameof(PlayerTransformSync)}");

			name = $"RemotePlayerRoot ({netId})";

			// Just in case a remote player is made before the local player
			if (PlayerPrefab == null) CreatePrefab();

			var remotePlayer = Instantiate(PlayerPrefab).transform;
			remotePlayer.name = "RemotePlayer";
			remotePlayer.transform.parent = transform;
			remotePlayer.transform.localPosition = Vector3.zero;
			remotePlayer.transform.localRotation = Quaternion.identity;

			var networkPlayer = GetComponent<NetworkPlayer>();

			networkPlayer.remotePlayerBoatGraphics.boatSubModelTogglers = remotePlayer.GetComponentsInChildren<BoatSubModelToggler>(true);
			networkPlayer.remotePlayerBoatGraphics.boatModelProxies = remotePlayer.GetComponentsInChildren<BoatModelProxy>(true);
			networkPlayer.remotePlayerBoatGraphics.wake = remotePlayer.Find("BoatTrailParticles").gameObject;

			networkPlayer.remoteTeleportAbility.teleportEffect = remotePlayer.Find("TeleportEffect").gameObject;

			networkPlayer.remoteBanishAbility.banishEffect = remotePlayer.Find("BanishAbility/BanishEffect").gameObject;
			networkPlayer.remoteBanishAbility.banishAudioSource = remotePlayer.Find("BanishAbility/BanishLoopSFX").GetComponent<AudioSource>();

			var atrophy = AbilityHelper.GetAbility<AtrophyAbility>();
			networkPlayer.remoteAtrophyAbility.playerVfxPrefab = atrophy.playerVfxPrefab;
			networkPlayer.remoteAtrophyAbility.harvestVfxPrefab = atrophy.spotVfxPrefab;
			networkPlayer.remoteAtrophyAbility.loopAudio = AudioSourceUtil.MakeSpatialAudio(remotePlayer.gameObject);
			networkPlayer.remoteAtrophyAbility.loopAudio.clip = atrophy.loopAudioSource.clip;

			remotePlayer.gameObject.SetActive(true);

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
