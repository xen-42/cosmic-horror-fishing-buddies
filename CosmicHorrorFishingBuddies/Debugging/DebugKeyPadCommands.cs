using CosmicHorrorFishingBuddies.Core;
using CosmicHorrorFishingBuddies.PlayerSync;
using CosmicHorrorFishingBuddies.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Debugging
{
	public class DebugKeyPadCommands : MonoBehaviour
	{
		private List<(KeyCode key, Action action)> _commands;

		private int _dockIndex;

		public void Awake()
		{
			_commands = new()
			{
				(KeyCode.Keypad0, LogAllWorldEventData),
				(KeyCode.Keypad1, TriggerPassiveEvent),
				(KeyCode.Keypad2, CycleNextDock)
			};
		}

		public void Update()
		{
			foreach (var (key, action) in _commands)
			{
				if (Input.GetKeyDown(key))
				{

					NotificationHelper.ShowNotificationWithColour(NotificationType.NONE, $"Invoked debug action {action.Method.Name}", DredgeColorTypeEnum.POSITIVE);
					try
					{
						action.Invoke();
					}
					catch (Exception e)
					{
						CFBCore.LogError($"Failed to invoke debug command {e}");
					}
				}
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

		private void CycleNextDock()
		{
			var docks = GameObject.FindObjectsOfType<DockPOI>(true).Select(x => x?.dockSlots[0]?.gameObject).Where(x => x != null);
			if (_dockIndex >= docks.Count()) _dockIndex -= docks.Count();

			CFBCore.LogInfo($"Teleporting to dock {_dockIndex} out of {docks.Count()}");

			TeleportPlayer.To(docks.ElementAt(_dockIndex++));

			// Keep Sanity up
			GameManager.Instance.Player.Sanity.ChangeSanity(100);
		}
	}
}