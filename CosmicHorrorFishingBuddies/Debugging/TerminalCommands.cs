using CommandTerminal;
using CosmicHorrorFishingBuddies.Util;
using UnityEngine;

namespace CosmicHorrorFishingBuddies.Debugging
{
	internal class TerminalCommands : MonoBehaviour
	{
		public void Start()
		{
			AddTerminalCommands();
		}

		public void OnDestroy()
		{
			RemoveTerminalCommands();
		}

		private void AddTerminalCommands()
		{
			Terminal.Shell.AddCommand("remote.flicker", DebugTriggerFlickerEvent, 0, 0, "Triggers the flicker lights world event");
			Terminal.Shell.AddCommand("remote.mainfest", DebugManifest, 0, 0, "Triggers the manifest ability");
		}

		private void RemoveTerminalCommands()
		{
			Terminal.Shell.RemoveCommand("remote.flicker");
			Terminal.Shell.RemoveCommand("remote.mainfest");
		}

		private void DebugTriggerFlickerEvent(CommandArg[] args) => EventHelper.GetWorldEvent<FlickerLightsWorldEvent>().Activate();
		private void DebugManifest(CommandArg[] args) => GameManager.Instance.PlayerAbilities.abilityMap["teleport"].Activate();
	}
}
