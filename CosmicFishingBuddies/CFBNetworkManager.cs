using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.PlayerSync;
using kcp2k;
using Mirror;
using System;
using System.IO;
using UnityEngine;

namespace CosmicFishingBuddies
{
	internal class CFBNetworkManager : NetworkManager
	{
		public override void Awake()
		{
			try
			{
				gameObject.SetActive(false);

				var kcpTransport = gameObject.AddComponent<KcpTransport>();
				transport = kcpTransport;

				gameObject.AddComponent<NetworkManagerHUD>();

				playerPrefab = MakeNewNetworkObject(1, "PlayerPrefab");
				playerPrefab.AddComponent<PlayerTransformSync>();
				playerPrefab.AddComponent<NetworkTransform>().syncDirection = SyncDirection.ClientToServer;

				gameObject.SetActive(true);

				base.Awake();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
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
