using CosmicHorrorFishingBuddies.Extensions;
using CosmicHorrorFishingBuddies.HarvestPOISync;
using CosmicHorrorFishingBuddies.PlayerSync;
using CosmicHorrorFishingBuddies.PlayerSync.AbilitySync;
using CosmicHorrorFishingBuddies.TimeSync;
using CosmicHorrorFishingBuddies.Util;
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
                networkPlayer.remoteFoghornAbility.foghornEndSource = remoteFoghornObj.AddComponent<AudioSource>();
                networkPlayer.remoteFoghornAbility.foghornEndSource.spatialBlend = 1;
                networkPlayer.remoteFoghornAbility.foghornEndSource.minDistance = 15;
                networkPlayer.remoteFoghornAbility.foghornMidSource = remoteFoghornObj.AddComponent<AudioSource>();
                networkPlayer.remoteFoghornAbility.foghornMidSource.spatialBlend = 1;
                networkPlayer.remoteFoghornAbility.foghornMidSource.minDistance = 15;

                // PlaySFX
                networkPlayer.oneShotSource = playerPrefab.AddComponent<AudioSource>();
                networkPlayer.oneShotSource.spatialBlend = 1;
                networkPlayer.oneShotSource.minDistance = 5;
                networkPlayer.oneShotSource.maxDistance = 20;

                // Engine audio
                // All NetworkBehaviours have to be on root object to share its netid
                networkPlayer.remotePlayerEngineAudio = playerPrefab.AddComponent<RemotePlayerEngineAudio>();

                var engineAudioObj = new GameObject(nameof(RemotePlayerEngineAudio));
                engineAudioObj.transform.parent = playerPrefab.transform;
                engineAudioObj.transform.localPosition = Vector3.zero;
                networkPlayer.remotePlayerEngineAudio.engineSource = engineAudioObj.AddComponent<AudioSource>();
                networkPlayer.remotePlayerEngineAudio.engineSource.spatialBlend = 1;
                networkPlayer.remotePlayerEngineAudio.engineSource.minDistance = 10;
                networkPlayer.remotePlayerEngineAudio.engineSource.maxDistance = 40;
                networkPlayer.remotePlayerEngineAudio.engineSource.loop = true;
                networkPlayer.remotePlayerEngineAudio.engineSource.playOnAwake = true;

				// Boat graphics
				networkPlayer.remotePlayerBoatGraphics = playerPrefab.AddComponent<RemoteBoatGraphics>();

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
                SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

                PlayerManager.PlayerJoined.AddListener(OnPlayerJoined);
                PlayerManager.PlayerLeft.AddListener(OnPlayerLeft);

                base.Awake();
            }
            catch (Exception e)
            {
                CFBCore.LogError($"{e}");
            }
        }

        private void SceneManager_activeSceneChanged(Scene prev, Scene current)
        {
            if (current.name != Scenes.Game)
            {
                if (NetworkServer.activeHost)
                {
                    StopHost();
                }
                if (NetworkClient.isConnected)
                {
                    StopClient();
                }
            }
        }

        private void OnPlayerJoined(bool isOwned)
        {
			if (!isOwned)
			{
				NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, "A player has joined the game!", DredgeColorTypeEnum.POSITIVE);
			}
        }

        private void OnPlayerLeft(bool isOwned)
        {
			if (!isOwned)
			{
				NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, "A player has left the game.", DredgeColorTypeEnum.NEGATIVE);
			}
        }

        public void SetConnection(bool isHost, string address, TransportType transportType)
        {
            networkAddress = address;
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
        }

        private void OnPlayerLoaded()
        {
            if (_isConnected)
            {
				GameManager.Instance.Player.gameObject.AddComponent<NetworkHarvestPOIManager>();
                if (_isHost)
                {
                    StartHost();
                }
                else
                {
                    StartClient();
                }
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

        public override void OnClientError(TransportError error, string reason)
        {
			base.OnClientError(error, reason);
			CFBCore.LogError($"TRANSPORT ERROR: {error} - {reason}");
		}

        public override void OnStopClient()
        {
			CFBCore.LogInfo("Stop client");

			if (SceneManager.GetActiveScene().name == Scenes.Game)
			{
				GameManager.Instance.Loader.LoadTitleFromGame();
			}

			_isConnected = false;

			base.OnStopClient();
		}
    }
}
