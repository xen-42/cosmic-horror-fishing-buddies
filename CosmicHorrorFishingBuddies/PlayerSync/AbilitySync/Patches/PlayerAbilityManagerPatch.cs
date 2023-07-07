using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch(typeof(PlayerAbilityManager))]
	internal static class PlayerAbilityManagerPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(nameof(PlayerAbilityManager.GetTimeSinceLastCast))]
		public static void PlayerAbilityManager_GetTimeSinceLastCast(AbilityData abilityData, ref float __result)
		{
			// This means the last cast happened in the future - impossible
			if (__result < 0)
			{
				// Reset it
				GameManager.Instance.SaveData.abilityHistory[abilityData.name] = float.NegativeInfinity;
				__result = float.PositiveInfinity;
			}
		}
	}
}
