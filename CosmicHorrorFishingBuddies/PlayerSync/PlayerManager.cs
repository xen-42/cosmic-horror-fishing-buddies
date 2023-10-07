using Mirror;
using System.Collections.Generic;
using UnityEngine.Events;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal static class PlayerManager
	{
		public static uint LocalNetID => NetworkClient.connection?.identity?.netId ?? uint.MaxValue;

		public static readonly Dictionary<uint, NetworkPlayer> Players = new();
		public static UnityEvent<bool> PlayerJoined = new();
		public static UnityEvent<bool> PlayerLeft = new();
	}
}
