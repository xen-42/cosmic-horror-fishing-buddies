using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Extensions
{
	internal static class MirrorExtensions
	{
		public static GameObject SpawnWithServerAuthority(this GameObject prefab, bool active = true)
		{
			var obj = prefab.InstantiateInactive();
			obj.SetActive(active);
			NetworkServer.Spawn(obj, NetworkServer.localConnection);
			return obj;
		}
	}
}
