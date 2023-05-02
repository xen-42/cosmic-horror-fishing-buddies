using CosmicFishingBuddies.PlayerSync;
using kcp2k;
using Mirror;
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
			gameObject.SetActive(false);

			var kcpTransport = gameObject.AddComponent<KcpTransport>();
			gameObject.AddComponent<NetworkManagerHUD>();
			transport = kcpTransport;

			playerPrefab = new GameObject("PlayerPrefab");
			playerPrefab.AddComponent<NetworkIdentity>();
			playerPrefab.AddComponent<PlayerTransformSync>();

			gameObject.SetActive(true);
		}

		public override void OnStartHost()
		{
			base.OnStartHost();

			CFBCore.Log("Now hosting");
		}

		public override void OnClientConnect()
		{
			base.OnClientConnect();

			CFBCore.Log("Client connected");
		}

		public override void OnStartClient()
		{
			base.OnStartClient();

			CFBCore.Log("Started client");
		}
	}
}
