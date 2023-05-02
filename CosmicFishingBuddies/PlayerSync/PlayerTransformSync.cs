using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CosmicFishingBuddies.BaseSync;
using UnityEngine;

namespace CosmicFishingBuddies.PlayerSync
{
	internal class PlayerTransformSync : TransformSync
	{
		public static PlayerTransformSync LocalInstance { get; private set; }

		public static GameObject PlayerPrefab { get; private set; }

		private static void CreatePrefab()
		{
			try
			{
				PlayerPrefab = new GameObject("PlayerPrefab");
				var boatModel = GameManager.Instance.Player.transform.Find("Boat1");
				boatModel.transform.parent = PlayerPrefab.transform;
				boatModel.transform.localPosition = Vector3.zero;

				GameObject.DontDestroyOnLoad(PlayerPrefab);
			}
			catch (Exception e)
			{
				CFBCore.LogError($"Failed to make player prefab - multiplayer will not function {e}");
			}
		}

		protected override Transform InitLocalTransform()
		{
			CFBCore.Log($"Creating local {nameof(PlayerTransformSync)}");

			LocalInstance = this;
			return GameManager.Instance.Player.transform;
		}

		protected override Transform InitRemoteTransform()
		{
			CFBCore.Log($"Creating remote {nameof(PlayerTransformSync)}");

			if (PlayerPrefab == null) CreatePrefab();
			return Instantiate(PlayerPrefab).transform;
		}
	}
}
