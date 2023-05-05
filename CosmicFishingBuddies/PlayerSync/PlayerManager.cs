using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace CosmicFishingBuddies.PlayerSync
{
	internal static class PlayerManager
	{
		public static readonly List<NetworkPlayer> Players = new();
		public static UnityEvent RemotePlayerJoined = new();
		public static UnityEvent RemotePlayerLeft = new();
	}
}
