using Mirror;
using System;
using System.Collections.Generic;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	internal static class PlayerManager
	{
		public static uint LocalNetID => NetworkClient.connection?.identity?.netId ?? uint.MaxValue;

		public static readonly Dictionary<uint, NetworkPlayer> Players = new();
		public static Action<bool, uint> PlayerJoined;
		public static Action<bool, uint> PlayerLeft;
	}
}
