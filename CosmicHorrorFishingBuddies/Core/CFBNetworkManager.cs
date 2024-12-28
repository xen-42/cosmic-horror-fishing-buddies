using CosmicHorrorFishingBuddies.Debugging;
using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.HarvestPOISync;
using CosmicHorrorFishingBuddies.PlayerSync;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync;
using CosmicHorrorFishingBuddies.TimeSync;
using CosmicHorrorFishingBuddies.Util;
using CosmicHorrorFishingBuddies.WorldEventSync;
using EpicTransport;
using kcp2k;
using Mirror;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CosmicHorrorFishingBuddies.Core
{
	internal class CFBNetworkManager : NetworkManager
	{
		public static CFBNetworkManager Instance { get; private set; }

		private bool _flagSkipGoingToTitle;
		private bool _isConnected;
		private bool _isHost;

		public TransportType TransportType { get; private set; }

		private KcpTransport _kcpTransport;
		private EosTransport _epicTransport;
		private EosApiKey _epicApiKey;

		public static GameObject GlobalSyncPrefab { get; private set; }
		public static GameObject IndexedHarvestPOIPrefab { get; private set; }
		public static GameObject BaitHarvestPOIPrefab { get; private set; }
		public static GameObject PlacedHarvestPOIPrefab { get; private set; }

		public override void Awake()
		{
			try
			{
				Instance = this;

				gameObject.SetActive(false);

				_kcpTransport = gameObject.AddComponent<KcpTransport>();
				// KCP uses milliseconds
				_kcpTransport.Timeout = DebugController.Instance.Timeout * 1000;

				// EPIC
				_epicApiKey = ScriptableObject.CreateInstance<EosApiKey>();
				_epicApiKey.epicProductName = "chfb";
				_epicApiKey.epicProductVersion = "1.0";
				_epicApiKey.epicProductId = "353d318b07cc42a6b571e853b56d5d29";
				_epicApiKey.epicSandboxId = "e6b038c4475241b39ad45efd711dfe92";
				_epicApiKey.epicDeploymentId = "532fdccd86a84252adc328dc2404cda4";
				_epicApiKey.epicClientId = "xyza7891iS3Ss77kZa8Ps6fwTrTQcX8l";
				_epicApiKey.epicClientSecret = "4rTbIfqZlTzvQmpkJuU/HqMfYECkETCeBEbfo13DC+g";

				var eosSdkComponent = gameObject.AddComponent<EOSSDKComponent>();
				eosSdkComponent.SetValue("apiKeys", _epicApiKey);
				eosSdkComponent.epicLoggerLevel = Epic.OnlineServices.Logging.LogLevel.VeryVerbose;

				var eosTransport = gameObject.AddComponent<EosTransport>();
				eosTransport.timeout = DebugController.Instance.Timeout;
				_epicTransport = eosTransport;

				// PREFABS

				playerPrefab = MakeNewNetworkObject(1, "PlayerPrefab");
				playerPrefab.AddComponent<PlayerTransformSync>();
				playerPrefab.AddComponent<NetworkTransform>().syncDirection = SyncDirection.ClientToServer;

				var networkPlayer = playerPrefab.AddComponent<NetworkPlayer>();
				networkPlayer.syncDirection = SyncDirection.ClientToServer;

				// REMOTE PLAYER ABILITIES
				// Foghorn
				networkPlayer.remoteFoghornAbility = playerPrefab.AddComponent<RemoteFoghornAbility>();
				var remoteFoghornObj = new GameObject(nameof(RemoteFoghornAbility));
				remoteFoghornObj.transform.parent = playerPrefab.transform;
				remoteFoghornObj.transform.localPosition = Vector3.zero;
				networkPlayer.remoteFoghornAbility.foghornEndSource = AudioSourceUtil.MakeSpatialAudio(remoteFoghornObj, maxDistance: 100);
				networkPlayer.remoteFoghornAbility.foghornMidSource = AudioSourceUtil.MakeSpatialAudio(remoteFoghornObj, maxDistance: 100);

				// PlaySFX
				networkPlayer.oneShotSource = AudioSourceUtil.MakeSpatialAudio(playerPrefab);

				// Engine audio
				// All NetworkBehaviours have to be on root object to share its netid
				networkPlayer.remotePlayerEngineAudio = playerPrefab.AddComponent<RemotePlayerEngineAudio>();

				var engineAudioObj = new GameObject(nameof(RemotePlayerEngineAudio));
				engineAudioObj.transform.parent = playerPrefab.transform;
				engineAudioObj.transform.localPosition = Vector3.zero;
				networkPlayer.remotePlayerEngineAudio.engineSource = AudioSourceUtil.MakeSpatialAudio(engineAudioObj, loop: true);

				// Boat graphics
				networkPlayer.remotePlayerBoatGraphics = playerPrefab.AddComponent<RemoteBoatGraphics>();
				networkPlayer.remoteSteeringAnimator = playerPrefab.AddComponent<RemoteSteeringAnimator>();

				// Teleport ability
				networkPlayer.remoteTeleportAbility = playerPrefab.AddComponent<RemoteTeleportAbility>();
				networkPlayer.remoteBanishAbility = playerPrefab.AddComponent<RemoteBanishAbility>();
				networkPlayer.remoteAtrophyAbility = playerPrefab.AddComponent<RemoteAtrophyAbility>();
				networkPlayer.remoteLightAbility = playerPrefab.AddComponent<RemoteLightAbility>();
				networkPlayer.remoteTrawlNetAbility = playerPrefab.AddComponent<RemoteTrawlNetAbility>();

				// 2 - GlobalSyncManager
				GlobalSyncPrefab = MakeNewNetworkObject(2, nameof(GlobalSyncPrefab));
				GlobalSyncPrefab.AddComponent<TimeSyncManager>();
				GlobalSyncPrefab.AddComponent<CFBSpawnManager>();
				GlobalSyncPrefab.AddComponent<WorldEventSyncManager>();
				spawnPrefabs.Add(GlobalSyncPrefab);

				// 3 - IndexedHarvestPOI
				IndexedHarvestPOIPrefab = MakeNewNetworkObject(3, nameof(IndexedHarvestPOIPrefab));
				IndexedHarvestPOIPrefab.AddComponent<IndexedNetworkHarvestPOI>();
				spawnPrefabs.Add(IndexedHarvestPOIPrefab);

				// 4 - BaitHarvestPOI
				BaitHarvestPOIPrefab = MakeNewNetworkObject(4, nameof(BaitHarvestPOIPrefab));
				BaitHarvestPOIPrefab.AddComponent<NetworkBaitHarvestPOI>();
				spawnPrefabs.Add(BaitHarvestPOIPrefab);

				// 5 - PlacedHarvestPOIPrefab
				PlacedHarvestPOIPrefab = MakeNewNetworkObject(5, nameof(PlacedHarvestPOIPrefab));
				PlacedHarvestPOIPrefab.AddComponent<NetworkPlacedHarvestPOI>();
				spawnPrefabs.Add(PlacedHarvestPOIPrefab);

				gameObject.SetActive(true);

				CFBCore.Instance.PlayerLoaded.AddListener(OnPlayerLoaded);
				CFBCore.Instance.SwitchSceneRequested.AddListener(OnSwitchSceneRequested);

				PlayerManager.PlayerJoined += OnPlayerJoined;
				PlayerManager.PlayerLeft += OnPlayerLeft;

				base.Awake();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		private void OnSwitchSceneRequested(string scene)
		{
			if (SceneManager.GetActiveScene().name == Scenes.Game)
			{
				_flagSkipGoingToTitle = true;
				if (NetworkServer.activeHost)
				{
					StopHost();
				}
				else if (NetworkClient.isConnected)
				{
					StopClient();
				}
				_flagSkipGoingToTitle = false;
			}
		}

		private void OnPlayerJoined(bool isOwned, uint netID)
		{
			if (!isOwned)
			{
				NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, "A player has joined the game!", DredgeColorTypeEnum.POSITIVE);
			}
		}

		private void OnPlayerLeft(bool isOwned, uint netID)
		{
			if (!isOwned)
			{
				NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, "A player has left the game.", DredgeColorTypeEnum.NEGATIVE);
			}
		}

		public void SetConnection(bool isHost, string address, TransportType transportType)
		{
			networkAddress = address.Trim();
			_isHost = isHost;
			_isConnected = true;

			TransportType = transportType;

			switch (transportType)
			{
				case TransportType.KCP:
					transport = _kcpTransport;
					break;
				case TransportType.EPIC:
					transport = _epicTransport;
					break;
				default:
					throw new Exception($"Unsupported transport {transportType}");
			}
			// Have to set this when changing transport after mirror initializes
			Transport.active = transport;
		}

		private void OnPlayerLoaded()
		{
			if (_isConnected)
			{
				GameManager.Instance.Player.gameObject.AddComponent<NetworkHarvestPOIManager>();
				if (_isHost)
				{
					StartHost();
					NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, "Now hosting", DredgeColorTypeEnum.POSITIVE);
				}
				else
				{
					StartClient();
					NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, $"You've joined the server", DredgeColorTypeEnum.POSITIVE);
				}
			}
			else
			{
				NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, "Something went wrong when connecting to the server", DredgeColorTypeEnum.NEGATIVE);
			}
		}

		private GameObject MakeNewNetworkObject(uint assetId, string name)
		{
			var assetBundle = AssetBundle.LoadFromFile(Path.Combine(CFBCore.GetModFolder(), "Assets", "qsb_empty"));
			var template = assetBundle.LoadAsset<GameObject>("Assets/Prefabs/Empty.prefab");
			assetBundle.Unload(false);

			template.name = name;
			var netID = template.AddComponent<NetworkIdentity>();
			netID.SetValue("_assetId", assetId);

			return template;
		}

		public override void OnStartHost()
		{
			base.OnStartHost();

			GlobalSyncPrefab.SpawnWithServerAuthority();
		}

		public override void OnStartClient()
		{
			base.OnStartClient();

			CFBCore.LogInfo($"Trying to connect to [{networkAddress}] on [{transport}]");
		}

		public override void OnClientError(TransportError error, string reason)
		{
			base.OnClientError(error, reason);
			CFBCore.LogError($"TRANSPORT ERROR: [{error}] - [{reason}] on [{transport}] [{networkAddress}]");
		}

		public override void OnStopClient()
		{
			CFBCore.LogInfo("Stop client");

			_isConnected = false;

			base.OnStopClient();

			if (!_flagSkipGoingToTitle && SceneManager.GetActiveScene().name == Scenes.Game)
			{
				// GameManager.Instance.Loader.LoadTitleFromGame();
			}

			// Temporary fix (that is to say, permanent)
			CFBCore.RestartGame();
		}
	}
}
