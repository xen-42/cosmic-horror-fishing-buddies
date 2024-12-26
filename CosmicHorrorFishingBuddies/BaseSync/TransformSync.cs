using CosmicHorrorFishingBuddies.Core;
using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.BaseSync
{
    internal abstract class TransformSync : NetworkBehaviour
	{
		protected abstract Transform InitLocalTransform();
		protected abstract Transform InitRemoteTransform();

		private Transform _syncedTransform;

		private NetworkTransform _networkTransform;

		public void Awake()
		{
			_networkTransform = gameObject.GetComponent<NetworkTransform>();
			_networkTransform.target = transform;
		}

		public void Start()
		{
			CFBCore.LogInfo($"Start TransformSync: {netId}");

			_syncedTransform = isOwned ? InitLocalTransform() : InitRemoteTransform();

			_networkTransform.transform.position = Vector3.zero;
			_networkTransform.transform.rotation = Quaternion.identity;

			_networkTransform.target = _syncedTransform;
		}
	}
}
