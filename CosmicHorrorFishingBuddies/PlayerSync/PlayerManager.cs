using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal static class PlayerManager
	{
		public static uint LocalNetID => NetworkClient.connection.identity.netId;

		public static readonly List<NetworkPlayer> Players = new();
		public static UnityEvent<bool> PlayerJoined = new();
		public static UnityEvent<bool> PlayerLeft = new();
	}
}
