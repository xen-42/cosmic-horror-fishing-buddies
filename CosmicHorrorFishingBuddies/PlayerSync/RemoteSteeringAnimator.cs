using Mirror;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.PlayerSync
{
	[RequireComponent(typeof(NetworkPlayer))]
	internal class RemoteSteeringAnimator : NetworkBehaviour
	{
		public Transform[] rudders;
		public Transform[] propellers;

		public float rudderMaxTurnDegrees = 50f;
		public Vector3 propellorMaxTurnSpeed = new(0f, 0f, 50f);

		private float _moveValX;
		private float _moveValY;
		private float _lerpedMoveValY;

		private NetworkPlayer _player;

		[Command]
		public void SetSteeringParameters(float moveValX, float moveValY) => RpcSetSteeringParameters(moveValX, moveValY);

		[ClientRpc(includeOwner = false)]
		public void RpcSetSteeringParameters(float moveValX, float moveValY)
		{
			_moveValX = moveValX;
			_moveValY = moveValY;
		}

		public void Start()
		{
			_player = GetComponent<NetworkPlayer>();
			_player.remotePlayerBoatGraphics.RefreshBoatModel.AddListener(OnRefreshBoatModel);
		}

		public void OnDestroy()
		{
			_player.remotePlayerBoatGraphics.RefreshBoatModel.RemoveListener(OnRefreshBoatModel);
		}

		private void OnRefreshBoatModel()
		{
			var currentSteering = _player.remotePlayerBoatGraphics.CurrentBoatModelProxy.GetComponentInChildren<SteeringAnimator>(true);

			rudders = currentSteering.rudders;
			propellers = currentSteering.propellers;

			rudderMaxTurnDegrees = currentSteering.rudderMaxTurnDegrees;
			propellorMaxTurnSpeed = currentSteering.propellorMaxTurnSpeed;
		}

		public void Update()
		{
			_lerpedMoveValY = Mathf.Lerp(_lerpedMoveValY, _moveValY, Time.deltaTime * 4f);
			if (_moveValY < 0f)
			{
				_moveValX *= -1f;
			}
			for (int i = 0; i < rudders.Length; i++)
			{
				Vector3 euler = new(0f, rudderMaxTurnDegrees * -_moveValX, 0f);
				rudders[i].localRotation = Quaternion.Lerp(rudders[i].localRotation, Quaternion.Euler(euler), Time.deltaTime * 10f);
			}
			for (int j = 0; j < propellers.Length; j++)
			{
				propellers[j].transform.Rotate(propellorMaxTurnSpeed * _lerpedMoveValY * Time.deltaTime, Space.Self);
			}
		}
	}
}
