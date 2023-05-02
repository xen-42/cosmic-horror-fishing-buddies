using CosmicFishingBuddies.Extensions;
using CosmicFishingBuddies.PlayerSync;
using kcp2k;
using Mirror;
using Mirror.SimpleWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				gameObject.AddComponent<NetworkManagerHUD>();
				transport = kcpTransport;

				playerPrefab = new GameObject("PlayerPrefab");
				var netID = playerPrefab.AddComponent<NetworkIdentity>();
				netID.SetValue("_assetId", (uint)1);
				playerPrefab.AddComponent<PlayerTransformSync>();

				gameObject.SetActive(true);

				base.Awake();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnServerReady(NetworkConnectionToClient conn)
		{
			CFBCore.Log($"Server ready {conn.identity}");

			try
			{
				base.OnServerReady(conn);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnServerConnect(NetworkConnectionToClient conn)
		{
			CFBCore.Log($"Server connect {conn.identity}");

			try
			{
				base.OnServerConnect(conn);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStartServer()
		{
			CFBCore.Log("Server start");

			try
			{
				base.OnStartServer();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStopServer()
		{
			CFBCore.Log("Server stop");

			try
			{
				base.OnStopServer();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStartHost()
		{
			CFBCore.Log("Now hosting");

			try
			{
				base.OnStartHost();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStopHost()
		{
			CFBCore.Log("Stopped hosting");

			try
			{
				base.OnStopHost();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientConnect()
		{
			CFBCore.Log("Client connected");

			try
			{
				base.OnClientConnect();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientDisconnect()
		{
			CFBCore.Log("Client disconnected");

			try
			{
				base.OnClientDisconnect();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientError(TransportError error, string reason)
		{
			CFBCore.LogError($"TRANSPORT ERROR: {error} - {reason}");

			try
			{
				base.OnClientError(error, reason);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStartClient()
		{
			CFBCore.Log("Started client");

			try
			{
				base.OnStartClient();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnClientNotReady()
		{
			CFBCore.Log("Client is not ready");

			try
			{
				base.OnClientNotReady();
			}
			catch (Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}

		public override void OnStopClient()
		{
			CFBCore.Log("Stop client");

			try
			{
				base.OnStopClient();
			}
			catch(Exception e)
			{
				CFBCore.LogError($"{e}");
			}
		}


	}
}
