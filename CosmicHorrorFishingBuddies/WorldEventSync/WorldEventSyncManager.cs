using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.WorldEventSync
{
	internal class WorldEventSyncManager : NetworkBehaviour
	{
		public static WorldEventSyncManager Instance { get; private set; }

		public bool IsRpc { get; private set; }

		public void Awake()
		{
			Instance = this;
		}

		public void DoEvent(WorldEventData worldEvent)
		{
			if (!IsRpc)
			{
				CmdDoEvent(worldEvent.name, NetworkPlayer.LocalPlayer.netIdentity.netId);
			}
		}

		[Command(requiresAuthority = false)]
		public void CmdDoEvent(string eventName, uint sender) => RpcDoEvent(eventName, sender);

		[ClientRpc]
		private void RpcDoEvent(string eventName, uint sender)
		{
			IsRpc = true;
			if (NetworkPlayer.LocalPlayer.netIdentity.netId != sender)
			{
				CFBCore.LogInfo($"{sender} told {NetworkPlayer.LocalPlayer.netIdentity.netId} to world event");
				try
				{
					var worldEvent = GameManager.Instance.DataLoader.allWorldEvents.First(x => x.name == eventName);
					var remotePlayer = PlayerManager.Players[sender].gameObject;
					DoRemoteEvent(worldEvent, remotePlayer);
				}
				catch (Exception ex)
				{
					CFBCore.LogError($"Couldn't trigger WorldEvent {ex}");
				}
			}
			IsRpc = false;
		}

		private void DoRemoteEvent(WorldEventData worldEventData, GameObject remotePlayer)
		{
			// Mostly copied from WorldEventManager.DoEvent, but with the remote player in place of the local player
			switch (worldEventData.eventType)
			{
				case WorldEventType.SPAWN_PREFAB:
					{
						Vector3 position = remotePlayer.transform.position;
						Vector3 vector = new Vector3(position.x, position.y, position.z);
						vector += remotePlayer.transform.right * worldEventData.playerSpawnOffset.x;
						vector += remotePlayer.transform.forward * worldEventData.playerSpawnOffset.z;
						vector.y = worldEventData.playerSpawnOffset.y;
						GameObject gameObject = GameObject.Instantiate<GameObject>(worldEventData.prefab, vector, Quaternion.identity);
						GameManager.Instance.WorldEventManager.currentEvent = gameObject.GetComponent<WorldEvent>();
						break;
					}
				case WorldEventType.EYE_PARTICLES:
					GameManager.Instance.WorldEventManager.currentEvent = GameManager.Instance.WorldEventManager.eyeParticleWorldEvent;
					break;
				case WorldEventType.FOG_GHOST:
					{
						float minDistance = float.PositiveInfinity;
						float thisDistance = 0f;
						List<WorldEvent> list = new List<WorldEvent>();
						if (GameManager.Instance.WorldEventManager.staticWorldEvents.TryGetValue(worldEventData.eventType, out list))
						{
							list.ForEach(delegate (WorldEvent worldEvent)
							{
								thisDistance = Vector3.Distance(remotePlayer.transform.position, worldEvent.transform.position);
								if (thisDistance < minDistance)
								{
									minDistance = thisDistance;
									GameManager.Instance.WorldEventManager.currentEvent = worldEvent;
								}
							});
						}
						break;
					}
				case WorldEventType.PLAYER_CHILD:
					{
						GameObject gameObject2 = GameObject.Instantiate<GameObject>(worldEventData.prefab, remotePlayer.transform);
						GameManager.Instance.WorldEventManager.currentEvent = gameObject2.GetComponent<WorldEvent>();
						break;
					}
			}
			if (GameManager.Instance.WorldEventManager.currentEvent == null)
			{
				return;
			}
			GameManager.Instance.WorldEventManager.currentEvent.worldEventData = worldEventData;
			WorldEvent worldEvent2 = GameManager.Instance.WorldEventManager.currentEvent;
			worldEvent2.OnEventFinished = (Action<WorldEvent>)Delegate.Combine(worldEvent2.OnEventFinished, new Action<WorldEvent>(GameManager.Instance.WorldEventManager.OnEventFinished));
			GameManager.Instance.WorldEventManager.currentEvent.Activate();
			GameManager.Instance.WorldEventManager.foghornHoldTime = 0f;
			GameManager.Instance.WorldEventManager.foghornBlastCount = 0;
			GameManager.Instance.WorldEventManager.AddWorldEventToHistory(worldEventData, GameManager.Instance.WorldEventManager.timeOfLastRoll);
		}
	}
}
