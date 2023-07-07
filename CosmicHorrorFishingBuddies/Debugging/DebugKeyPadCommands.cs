using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Debugging
{
	public class DebugKeyPadCommands : MonoBehaviour
	{
		private List<(KeyCode key, Action action)> _commands;

		public void Awake()
		{
			_commands = new()
			{
				(KeyCode.Keypad0, LogAllWorldEventData),
				(KeyCode.Keypad1, TriggerPassiveEvent)
			};
		}

		public void Update()
		{
			foreach (var (key, action) in _commands)
			{
				if (Input.GetKeyDown(key)) action.Invoke();
			}
		}

		private void TriggerPassiveEvent()
		{
			WorldEventData selectedWorldEventData = null;
			var list = new List<WorldEventData>();
			for (int i = 0; i < GameManager.Instance.DataLoader.allWorldEvents.Count; i++)
			{
				var worldEvent = GameManager.Instance.DataLoader.allWorldEvents[i];
				if (worldEvent.allowInPassiveMode)
				{
					list.Add(worldEvent);
				}
			}
			if (list.Count > 0)
			{
				selectedWorldEventData = list.PickRandom();
			}
			GameManager.Instance.WorldEventManager.DoEvent(selectedWorldEventData);
			NotificationHelper.ShowNotification(NotificationType.NONE, $"World Event: {selectedWorldEventData.name}");
		}

		private void LogAllWorldEventData()
		{
			foreach (var worldEvent in GameManager.Instance.DataLoader.allWorldEvents)
			{
				CFBCore.LogInfo($"{worldEvent.name} {worldEvent.allowInPassiveMode} {worldEvent.GetType().Name}");
			}
		}
	}
}
