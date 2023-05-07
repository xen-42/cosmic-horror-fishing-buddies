using CosmicFishingBuddies.Core;
using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.BaseSync
{
    internal abstract class TransformSync : NetworkBehaviour
	{
		protected abstract Transform InitLocalTransform();
		protected abstract Transform InitRemoteTransform();

		private Transform _syncedTransform;

		private void Start()
		{
			CFBCore.LogInfo($"Start TransformSync: {netId}");

			_syncedTransform = isOwned ? InitLocalTransform() : InitRemoteTransform();

			var networkTransform = gameObject.GetComponent<NetworkTransform>();
			networkTransform.target = transform;
		}

		private void Update()
		{
			if (isOwned)
			{
				transform.position = GameManager.Instance.Player.transform.position;
				transform.rotation = GameManager.Instance.Player.transform.rotation;
			}
		}
	}
}
