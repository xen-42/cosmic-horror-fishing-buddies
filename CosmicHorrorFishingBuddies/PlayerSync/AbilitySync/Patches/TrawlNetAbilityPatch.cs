using HarmonyLib;

namespace CosmicHorrorFishingBuddies.PlayerSync.AbilitySync.Patches
{
	[HarmonyPatch]
	internal static class TrawlNetAbilityPatch
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(TrawlNetAbility), nameof(TrawlNetAbility.Activate))]
		public static void TrawlNetAbility_Activate()
		{
			//NetworkPlayer.LocalPlayer?.CmdPlayOneShot(AudioSync.AudioEnum.TRAWL, 1f, 1f);
		}
	}
}
