using Mirror;
using UnityEngine;

namespace CosmicFishingBuddies.BaseSync
{
	internal abstract class TransformSync : NetworkBehaviour
	{
		private const float SmoothTime = 0.1f;
		private Vector3 _positionSmoothVelocity;
		private Quaternion _rotationSmoothVelocity;

		protected abstract Transform InitLocalTransform();
		protected abstract Transform InitRemoteTransform();

		private Transform _syncedTransform;

		private void Start()
		{
			DontDestroyOnLoad(this);
			CFBCore.Log($"Start TransformSync: {netId}");

			_syncedTransform = isOwned ? InitLocalTransform() : InitRemoteTransform();
		}

		private void Update()
		{
			if (_syncedTransform == null)
			{
				return;
			}

			if (isOwned)
			{
				transform.position = _syncedTransform.position;
				transform.rotation = _syncedTransform.rotation;
			}
			else
			{
				_syncedTransform.localPosition = Vector3.SmoothDamp(_syncedTransform.localPosition, transform.position, ref _positionSmoothVelocity, SmoothTime);
				_syncedTransform.localRotation = QuaternionHelper.SmoothDamp(_syncedTransform.localRotation, transform.rotation, ref _rotationSmoothVelocity, Time.deltaTime);
			}
		}
	}
}
