using CosmicFishingBuddies.BaseSync;
using CosmicFishingBuddies.Extensions;
using FluffyUnderware.DevTools.Extensions;
using System;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync
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

				// TODO: Fix the tyres
				boat1.GetComponentsInChildren<Rigidbody>().ForEach(x => x.gameObject.SetActive(false));
				boat2.GetComponentsInChildren<Rigidbody>().ForEach(x => x.gameObject.SetActive(false));
				boat3.GetComponentsInChildren<Rigidbody>().ForEach(x => x.gameObject.SetActive(false));
				boat4.GetComponentsInChildren<Rigidbody>().ForEach(x => x.gameObject.SetActive(false));

				var wake = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("BoatTrailParticles"));

				var teleportEffect = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Abilities/TeleportAbility/TeleportEffect"));
				teleportEffect.gameObject.SetActive(false);

				var banishAbility = CopyDetail(PlayerPrefab, GameManager.Instance.Player.transform.Find("Abilities/BanishAbility"));
				GameObject.DestroyImmediate(banishAbility.GetComponent<BanishAbility>());
				var banishAudio = banishAbility.GetComponentInChildren<AudioSource>();
				banishAudio.spatialBlend = 1;
				banishAudio.maxDistance = 20;
				banishAudio.minDistance = 5;

				PlayerPrefab.SetActive(false);
				GameObject.DontDestroyOnLoad(PlayerPrefab);
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
			return GameManager.Instance.Player.transform;
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
			networkPlayer.boatModelProxies = remotePlayer.GetComponentsInChildren<BoatModelProxy>();
			networkPlayer.remoteTeleportAbility.teleportEffect = remotePlayer.Find("TeleportEffect").gameObject;
			networkPlayer.remoteBanishAbility.banishEffect = remotePlayer.Find("BanishAbility/BanishEffect").gameObject;
			networkPlayer.remoteBanishAbility.banishAudioSource = remotePlayer.Find("BanishAbility/BanishLoopSFX").GetComponent<AudioSource>();

			networkPlayer.wake = remotePlayer.Find("BoatTrailParticles").gameObject;

			return remotePlayer;
		}
	}
}
