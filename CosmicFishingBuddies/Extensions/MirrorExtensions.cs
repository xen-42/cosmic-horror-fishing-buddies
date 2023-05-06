using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.Extensions
{
	internal static class MirrorExtensions
	{
		public static GameObject SpawnWithServerAuthority(this GameObject prefab)
		{
			var obj = GameObject.Instantiate(prefab);
			NetworkServer.Spawn(obj, NetworkServer.localConnection);
			return obj;
		}
	}
}
