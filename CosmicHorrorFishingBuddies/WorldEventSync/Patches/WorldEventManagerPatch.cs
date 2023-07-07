using CosmicHorrorFishingBuddies.PlayerSync;
using HarmonyLib;

namespace CosmicHorrorFishingBuddies.WorldEventSync.Patches
{
	[HarmonyPatch(typeof(WorldEventManager))]
	public static class WorldEventManagerPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch(nameof(WorldEventManager.RollForEvent))]
		public static bool WorldEventManager_RollForEvent()
		{
			// Each player can be triggering world events, so we reduce the likelihood of any occuring by 1 / number of players
			return UnityEngine.Random.value < 1f / PlayerManager.Players.Count;
		}

		[HarmonyPrefix]
		[HarmonyPatch(nameof(WorldEventManager.DoEvent))]
		public static void WorldEventManager_DoEvent(WorldEventData worldEventData)
		{
			WorldEventSyncManager.Instance.DoEvent(worldEventData);
		}
	}
}
