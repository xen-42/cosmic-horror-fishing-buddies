using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.PlayerSync;
using CosmicFishingBuddies.TimeSync;
using CosmicFishingBuddies.Util;
using kcp2k;
using Mirror;
using Mirror.FizzySteam;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CosmicFishingBuddies
{
	internal class CFBNetworkManager : NetworkManager
	{
		public static CFBNetworkManager Instance { get; private set; }

		private bool _isConnecting;
		private bool _isHost;

		private KcpTransport _kcpTransport;

		public enum TransportType
		{
			KCP,
			EPIC,
			STEAM
		}

		public static GameObject TimeSyncManagerPrefab { get; private set; }

		public override void Awake()
		{
			try
			{
				Instance = this;

				gameObject.SetActive(false);

				_kcpTransport = gameObject.AddComponent<KcpTransport>();

				// gameObject.AddComponent<NetworkManagerHUD>();

				playerPrefab = MakeNewNetworkObject(1, "PlayerPrefab");
				playerPrefab.AddComponent<PlayerTransformSync>();
				playerPrefab.AddComponent<NetworkTransform>().syncDirection = SyncDirection.ClientToServer;

				var networkPlayer = playerPrefab.AddComponent<NetworkPlayer>();
				networkPlayer.syncDirection = SyncDirection.ClientToServer;

				// REMOTE PLAYER ABILITIES
				// Foghorn
				networkPlayer.foghornEndSource = playerPrefab.AddComponent<AudioSource>();
				networkPlayer.foghornEndSource.spatialBlend = 1;
				networkPlayer.foghornEndSource.minDistance = 15;
				networkPlayer.foghornMidSource = playerPrefab.AddComponent<AudioSource>();
				networkPlayer.foghornMidSource.spatialBlend = 1;
				networkPlayer.foghornMidSource.minDistance = 15;

				// PlaySFX
				networkPlayer.oneShotSource = playerPrefab.AddComponent<AudioSource>();
				networkPlayer.oneShotSource.spatialBlend = 1;
				networkPlayer.oneShotSource.minDistance = 5;
				networkPlayer.oneShotSource.maxDistance = 20;

				// Engine audio
				// All NetworkBehaviours have to be on root object to share its netid
				networkPlayer.remotePlayerEngineAudio = playerPrefab.AddComponent<RemotePlayerEngineAudio>();

				var engineAudioObj = new GameObject("RemotePlayerEngineAudio");
				engineAudioObj.transform.parent = playerPrefab.transform;
				engineAudioObj.transform.localPosition = Vector3.zero;
				networkPlayer.remotePlayerEngineAudio.engineSource = engineAudioObj.AddComponent<AudioSource>();
				networkPlayer.remotePlayerEngineAudio.engineSource.spatialBlend = 1;
				networkPlayer.remotePlayerEngineAudio.engineSource.minDistance = 10;
				networkPlayer.remotePlayerEngineAudio.engineSource.maxDistance = 40;
				networkPlayer.remotePlayerEngineAudio.engineSource.loop = true;
				networkPlayer.remotePlayerEngineAudio.engineSource.playOnAwake = true;

				// 2 - TimeSyncManager
				TimeSyncManagerPrefab = MakeNewNetworkObject(2, nameof(TimeSyncManagerPrefab));
				TimeSyncManagerPrefab.AddComponent<TimeSyncManager>();
				spawnPrefabs.Add(TimeSyncManagerPrefab);

				gameObject.SetActive(true);

				CFBCore.Instance.PlayerLoaded.AddListener(OnPlayerLoaded);
				SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;

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

		public void SetConnection(bool isHost, string address, TransportType transportType)
		{
			networkAddress = address;
			_isHost = isHost;
			_isConnecting = true;

			switch(transportType)
			{
				case TransportType.KCP:
					transport = _kcpTransport;
					break;
				case TransportType.STEAM:
					var fizzy = gameObject.GetAddComponent<FizzySteamworks>();
					transport = fizzy;
					FizzyLogger.LogEvent += (string msg, FizzyMessageType sev) => CFBCore.LogInfo($"[FIZZYSTEAMWORKS] {msg}");
					break;
				default:
					throw new Exception($"Unsupported transport {transportType}");
			}
		}

		private void OnPlayerLoaded()
		{
			if (_isConnecting)
			{
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

		public override void OnServerReady(NetworkConnectionToClient conn)
		{
			try
			{
				CFBCore.LogInfo($"Server ready {conn.connectionId}");

				base.OnServerReady(conn);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnServerConnect(NetworkConnectionToClient conn)
		{
			try
			{
				CFBCore.LogInfo($"[{conn.connectionId}] connecting to server");

				base.OnServerConnect(conn);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStartServer()
		{
			try
			{
				CFBCore.LogInfo("Server start");

				base.OnStartServer();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStopServer()
		{
			try
			{
				CFBCore.LogInfo("Server stop");

				base.OnStopServer();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStartHost()
		{
			try
			{
				CFBCore.LogInfo($"Now hosting. NetworkServer active [{NetworkServer.active}] NetworkClient active [{NetworkClient.active}]");

				TimeSyncManagerPrefab.SpawnWithServerAuthority();

				base.OnStartHost();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStopHost()
		{
			try
			{
				CFBCore.LogInfo("Stopped hosting");

				base.OnStopHost();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientConnect()
		{
			try
			{
				CFBCore.LogInfo("Client connected");

				base.OnClientConnect();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientDisconnect()
		{
			try
			{
				CFBCore.LogInfo("Client disconnected");

				base.OnClientDisconnect();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientError(TransportError error, string reason)
		{
			try
			{
				CFBCore.LogError($"TRANSPORT ERROR: {error} - {reason}");

				base.OnClientError(error, reason);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStartClient()
		{
			try
			{
				CFBCore.LogInfo($"Started client. NetworkServer active [{NetworkServer.active}] NetworkClient active [{NetworkClient.active}]");

				base.OnStartClient();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientNotReady()
		{
			try
			{
				CFBCore.LogInfo("Client is not ready");

				base.OnClientNotReady();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStopClient()
		{
			try
			{
				CFBCore.LogInfo("Stop client");

				base.OnStopClient();
			}
			catch(Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}
	}
}
