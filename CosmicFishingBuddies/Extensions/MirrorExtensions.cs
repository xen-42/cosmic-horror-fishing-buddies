using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.Extensions
{
	internal static class MirrorExtensions
	{
		public static void SpawnWithServerAuthority(this GameObject prefab)
		{
			NetworkServer.Spawn(GameObject.Instantiate(prefab), NetworkServer.localConnection);
		}
	}
}
